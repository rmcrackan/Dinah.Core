using System.Text;

#nullable enable
namespace Dinah.Core.IO;

/// <summary>
/// Safe file persistence helpers: write via temp file then replace, and verified backups.
/// </summary>
public static class AtomicFileWriter
{
	/// <summary>
	/// Write text by creating a temp file in the same directory, flushing it, then replacing the destination.
	/// If <paramref name="path"/> already exists and a later step fails, the original file is left intact.
	/// </summary>
	public static void WriteAllText(string path, string contents)
		=> WriteAllText(path, contents, validateTempFile: null);

	/// <summary>
	/// Same as <see cref="WriteAllText(string, string)"/>, then runs <paramref name="validateTempFile"/> on the
	/// temp path before replacing. If validation throws, the destination is not modified.
	/// </summary>
	public static void WriteAllText(string path, string contents, Action<string>? validateTempFile)
	{
		ArgumentNullException.ThrowIfNull(contents);
		WriteAllBytes(path, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(contents), validateTempFile);
	}

	/// <summary>
	/// Write bytes by creating a temp file in the same directory, flushing it, then replacing the destination.
	/// </summary>
	public static void WriteAllBytes(string path, byte[] bytes)
		=> WriteAllBytes(path, bytes, validateTempFile: null);

	/// <summary>
	/// Same as <see cref="WriteAllBytes(string, byte[])"/>, with optional validation of the temp file before replace.
	/// </summary>
	public static void WriteAllBytes(string path, byte[] bytes, Action<string>? validateTempFile)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(path, nameof(path));
		ArgumentNullException.ThrowIfNull(bytes);

		var fullPath = Path.GetFullPath(path);
		var directory = Path.GetDirectoryName(fullPath);
		if (!string.IsNullOrEmpty(directory))
			Directory.CreateDirectory(directory);

		var tempPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
		var tempCommitted = false;
		try
		{
			using (var stream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
			{
				stream.Write(bytes, 0, bytes.Length);
				stream.Flush(flushToDisk: true);
			}

			validateTempFile?.Invoke(tempPath);

			ReplaceDestination(tempPath, fullPath);
			tempCommitted = true;
		}
		finally
		{
			if (!tempCommitted && File.Exists(tempPath))
			{
				try { File.Delete(tempPath); }
				catch { /* best-effort temp cleanup; original destination remains */ }
			}
		}
	}

	/// <summary>
	/// Copy an existing file to a uniquely named backup in the same directory.
	/// Verifies the backup is readable and byte-identical to the source before returning its path.
	/// </summary>
	/// <param name="path">Source file that must already exist.</param>
	/// <param name="backupExtension">Extension for the backup file (default <c>bak</c>).</param>
	/// <returns>Full path of the created backup.</returns>
	public static string CreateBackup(string path, string backupExtension = "bak")
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(path, nameof(path));
		ArgumentValidator.EnsureNotNullOrWhiteSpace(backupExtension, nameof(backupExtension));

		var fullPath = Path.GetFullPath(path);
		if (!File.Exists(fullPath))
			throw new FileNotFoundException("Cannot back up a missing file.", fullPath);

		var extension = backupExtension.Trim().TrimStart('.');
		if (string.IsNullOrWhiteSpace(extension))
			throw new ArgumentException("Backup extension is required.", nameof(backupExtension));

		string backupPath;
		do
		{
			backupPath = $"{fullPath}.{DateTime.UtcNow:yyyyMMddTHHmmssfff}.{Guid.NewGuid():N}.{extension}";
		}
		while (File.Exists(backupPath));

		File.Copy(fullPath, backupPath, overwrite: false);

		try
		{
			var original = File.ReadAllBytes(fullPath);
			var backup = File.ReadAllBytes(backupPath);
			if (original.Length != backup.Length || !original.AsSpan().SequenceEqual(backup))
				throw new IOException($"Backup verification failed for '{fullPath}'.");
		}
		catch
		{
			try { File.Delete(backupPath); }
			catch { /* best-effort cleanup of bad backup */ }
			throw;
		}

		return backupPath;
	}

	private static void ReplaceDestination(string tempPath, string destinationPath)
	{
		// Prefer Move(overwrite) over File.Replace: Replace can fail on Windows with
		// "Unable to remove the file to be replaced" when the destination is briefly locked.
		File.Move(tempPath, destinationPath, overwrite: true);
	}
}

using System.Security.Cryptography;
using System.Text;
using Dinah.Core.IO;

#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Portable secret store that persists raw secret bytes as files under a directory.
/// Suitable for Docker secret mounts and cross-machine key export/import.
/// Not OS-bound: anyone with filesystem access to the directory can read the secrets.
/// </summary>
public sealed class FileOsSecretStore : IOsSecretStore
{
	private readonly string _directory;

	public string Name => "File";
	public bool IsAvailable { get; }
	public string? UnavailableReason { get; }

	/// <summary>Directory that holds secret files. Created if missing when the store is available.</summary>
	public string DirectoryPath => _directory;

	public FileOsSecretStore(string directoryPath)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(directoryPath, nameof(directoryPath));

		try
		{
			_directory = Path.GetFullPath(directoryPath);
			Directory.CreateDirectory(_directory);
			IsAvailable = true;
			UnavailableReason = null;
		}
		catch (Exception ex)
		{
			_directory = directoryPath;
			IsAvailable = false;
			UnavailableReason = "File secret store directory is unavailable: " + SafeMessage(ex);
		}
	}

	public void Set(string key, ReadOnlySpan<byte> value)
	{
		EnsureAvailable();
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		if (value.Length == 0)
			throw new ArgumentException("Secret value must not be empty.", nameof(value));

		AtomicFileWriter.WriteAllBytes(GetPath(key), value.ToArray());
	}

	public bool TryGet(string key, out byte[] value)
	{
		EnsureAvailable();
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));

		var path = GetPath(key);
		if (!File.Exists(path))
		{
			value = [];
			return false;
		}

		value = File.ReadAllBytes(path);
		if (value.Length == 0)
		{
			value = [];
			return false;
		}

		return true;
	}

	public void Delete(string key)
	{
		EnsureAvailable();
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));

		var path = GetPath(key);
		if (File.Exists(path))
			File.Delete(path);
	}

	private void EnsureAvailable()
	{
		if (IsAvailable)
			return;

		throw new OsSecretStoreUnavailableException(Name, UnavailableReason ?? "File secret store is unavailable.");
	}

	private string GetPath(string key)
	{
		var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key))).ToLowerInvariant();
		return Path.Combine(_directory, hash + ".secret");
	}

	private static string SafeMessage(Exception ex)
		=> string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message;
}

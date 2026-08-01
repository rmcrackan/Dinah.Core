using System.Security.Cryptography;
using Dinah.Core.IO;

#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Copy an existing AES-GCM master key between secret stores or files without minting a new key.
/// Use this to move a desktop OS-bound key into a portable file/env store for Docker.
/// </summary>
public static class MasterKeyPortability
{
	/// <summary>
	/// Read <paramref name="masterKeyName"/> from <paramref name="source"/> and write it to <paramref name="destination"/>.
	/// Throws if the key is missing - never creates a new key.
	/// </summary>
	public static void Copy(
		IOsSecretStore source,
		IOsSecretStore destination,
		string masterKeyName = AesGcmSecretProtector.DefaultMasterKeyName)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(destination);
		ArgumentValidator.EnsureNotNullOrWhiteSpace(masterKeyName, nameof(masterKeyName));

		EnsureAvailable(source);
		EnsureAvailable(destination);

		var key = ReadExistingMasterKey(source, masterKeyName);
		try
		{
			destination.Set(masterKeyName, key);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(key);
		}
	}

	/// <summary>
	/// Export <paramref name="masterKeyName"/> from <paramref name="source"/> as raw bytes to <paramref name="filePath"/>.
	/// Throws if the key is missing - never creates a new key.
	/// </summary>
	public static void ExportToFile(
		IOsSecretStore source,
		string filePath,
		string masterKeyName = AesGcmSecretProtector.DefaultMasterKeyName)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentValidator.EnsureNotNullOrWhiteSpace(filePath, nameof(filePath));
		ArgumentValidator.EnsureNotNullOrWhiteSpace(masterKeyName, nameof(masterKeyName));

		EnsureAvailable(source);

		var key = ReadExistingMasterKey(source, masterKeyName);
		try
		{
			var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
			if (!string.IsNullOrEmpty(directory))
				Directory.CreateDirectory(directory);

			AtomicFileWriter.WriteAllBytes(filePath, key);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(key);
		}
	}

	/// <summary>
	/// Import raw master-key bytes from <paramref name="filePath"/> into <paramref name="destination"/> as <paramref name="masterKeyName"/>.
	/// </summary>
	public static void ImportFromFile(
		IOsSecretStore destination,
		string filePath,
		string masterKeyName = AesGcmSecretProtector.DefaultMasterKeyName)
	{
		ArgumentNullException.ThrowIfNull(destination);
		ArgumentValidator.EnsureNotNullOrWhiteSpace(filePath, nameof(filePath));
		ArgumentValidator.EnsureNotNullOrWhiteSpace(masterKeyName, nameof(masterKeyName));

		EnsureAvailable(destination);

		if (!File.Exists(filePath))
			throw new SecretProtectionException("Master key file was not found.");

		var key = File.ReadAllBytes(filePath);
		try
		{
			ValidateMasterKey(key);
			destination.Set(masterKeyName, key);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(key);
		}
	}

	private static byte[] ReadExistingMasterKey(IOsSecretStore source, string masterKeyName)
	{
		if (!source.TryGet(masterKeyName, out var key))
			throw new SecretProtectionException("Master key was not found in the source secret store.");

		try
		{
			ValidateMasterKey(key);
			return key;
		}
		catch
		{
			CryptographicOperations.ZeroMemory(key);
			throw;
		}
	}

	private static void ValidateMasterKey(byte[] key)
	{
		if (key.Length != AesGcmSecretProtector.KeySizeBytes)
			throw new SecretProtectionException("Master key has an unexpected length.");
	}

	private static void EnsureAvailable(IOsSecretStore store)
	{
		if (store.IsAvailable)
			return;

		throw new OsSecretStoreUnavailableException(
			store.Name,
			store.UnavailableReason ?? $"{store.Name} is unavailable.");
	}
}

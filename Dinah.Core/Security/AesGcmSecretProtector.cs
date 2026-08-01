using System.Security.Cryptography;
using System.Text;

#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Authenticated encryption (AES-GCM) using a master key kept in an <see cref="IOsSecretStore"/>.
/// Payload format: <c>v1.{nonce}.{ciphertext}.{tag}</c> with Base64URL segments (no padding).
/// </summary>
public sealed class AesGcmSecretProtector
{
	public const string PayloadVersion = "v1";
	public const string DefaultMasterKeyName = "aes-gcm-master-key-v1";
	public const int KeySizeBytes = 32;
	public const int NonceSizeBytes = 12;
	public const int TagSizeBytes = 16;

	private readonly IOsSecretStore _secretStore;
	private readonly string _masterKeyName;
	private Lock KeyLock { get; } = new();
	private byte[]? _cachedKey;

	public AesGcmSecretProtector(IOsSecretStore secretStore, string masterKeyName = DefaultMasterKeyName)
	{
		_secretStore = secretStore ?? throw new ArgumentNullException(nameof(secretStore));
		ArgumentValidator.EnsureNotNullOrWhiteSpace(masterKeyName, nameof(masterKeyName));
		_masterKeyName = masterKeyName;

		if (!AesGcm.IsSupported)
			throw new SecretProtectionException("AES-GCM is not supported on this platform.");
	}

	public string Protect(string plaintext, string? associatedData = null)
	{
		ArgumentNullException.ThrowIfNull(plaintext);
		EnsureStoreAvailable();

		var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
		var nonce = new byte[NonceSizeBytes];
		var ciphertext = new byte[plaintextBytes.Length];
		var tag = new byte[TagSizeBytes];
		RandomNumberGenerator.Fill(nonce);

		var key = GetOrCreateMasterKey();
		try
		{
			var aad = ToAad(associatedData);
			using var aes = new AesGcm(key, TagSizeBytes);
			aes.Encrypt(nonce, plaintextBytes, ciphertext, tag, aad);
		}
		catch (CryptographicException ex)
		{
			throw new SecretProtectionException("Encryption failed.", ex);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(plaintextBytes);
		}

		return string.Join('.',
			PayloadVersion,
			Base64Url.Encode(nonce),
			Base64Url.Encode(ciphertext),
			Base64Url.Encode(tag));
	}

	public string Unprotect(string payload, string? associatedData = null)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(payload, nameof(payload));
		EnsureStoreAvailable();

		var parts = payload.Split('.');
		if (parts.Length != 4 || parts[0] != PayloadVersion)
			throw new SecretProtectionException("Ciphertext payload is malformed or uses an unsupported version.");

		byte[] nonce;
		byte[] ciphertext;
		byte[] tag;
		try
		{
			nonce = Base64Url.Decode(parts[1]);
			ciphertext = Base64Url.Decode(parts[2]);
			tag = Base64Url.Decode(parts[3]);
		}
		catch (FormatException ex)
		{
			throw new SecretProtectionException("Ciphertext payload is malformed.", ex);
		}

		if (nonce.Length != NonceSizeBytes || tag.Length != TagSizeBytes)
			throw new SecretProtectionException("Ciphertext payload is malformed.");

		var plaintextBytes = new byte[ciphertext.Length];
		var key = GetExistingMasterKey();
		try
		{
			var aad = ToAad(associatedData);
			using var aes = new AesGcm(key, TagSizeBytes);
			aes.Decrypt(nonce, ciphertext, tag, plaintextBytes, aad);
			return Encoding.UTF8.GetString(plaintextBytes);
		}
		catch (CryptographicException ex)
		{
			throw new SecretProtectionException("Decryption failed. The ciphertext may be corrupt, tampered, or bound to different associated data.", ex);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(plaintextBytes);
		}
	}

	private void EnsureStoreAvailable()
	{
		if (_secretStore.IsAvailable)
			return;

		throw new OsSecretStoreUnavailableException(
			_secretStore.Name,
			_secretStore.UnavailableReason ?? $"{_secretStore.Name} is unavailable.");
	}

	private byte[] GetExistingMasterKey()
	{
		lock (KeyLock)
		{
			if (_cachedKey is not null)
				return _cachedKey;

			if (!_secretStore.TryGet(_masterKeyName, out var existing))
				throw new SecretProtectionException("Master key was not found in the secret store.");

			if (existing.Length != KeySizeBytes)
				throw new SecretProtectionException("Stored master key has an unexpected length.");

			_cachedKey = existing;
			return _cachedKey;
		}
	}

	private byte[] GetOrCreateMasterKey()
	{
		lock (KeyLock)
		{
			if (_cachedKey is not null)
				return _cachedKey;

			if (_secretStore.TryGet(_masterKeyName, out var existing))
			{
				if (existing.Length != KeySizeBytes)
					throw new SecretProtectionException("Stored master key has an unexpected length.");

				_cachedKey = existing;
				return _cachedKey;
			}

			var key = new byte[KeySizeBytes];
			RandomNumberGenerator.Fill(key);
			_secretStore.Set(_masterKeyName, key);
			_cachedKey = key;
			return _cachedKey;
		}
	}

	private static byte[]? ToAad(string? associatedData)
		=> associatedData is null ? null : Encoding.UTF8.GetBytes(associatedData);
}

file static class Base64Url
{
	public static string Encode(ReadOnlySpan<byte> data)
		=> Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');

	public static byte[] Decode(string value)
	{
		var padded = value.Replace('-', '+').Replace('_', '/');
		switch (padded.Length % 4)
		{
			case 2: padded += "=="; break;
			case 3: padded += "="; break;
		}
		return Convert.FromBase64String(padded);
	}
}

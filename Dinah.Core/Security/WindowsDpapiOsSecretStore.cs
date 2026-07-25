using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Dinah.Core.IO;

#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Windows OS-bound secret store using DPAPI (CurrentUser). Protected blobs are written under LocalApplicationData.
/// Encrypted material is not portable across machines or Windows users.
/// </summary>
public sealed class WindowsDpapiOsSecretStore : IOsSecretStore
{
	private readonly string _directory;
	private readonly byte[] _entropy;

	public string Name => "Windows DPAPI";
	public bool IsAvailable { get; }
	public string? UnavailableReason { get; }

	public WindowsDpapiOsSecretStore(string applicationName)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(applicationName, nameof(applicationName));

		if (!OperatingSystem.IsWindows())
		{
			IsAvailable = false;
			UnavailableReason = "Windows DPAPI is only available on Windows.";
			_directory = string.Empty;
			_entropy = [];
			return;
		}

		var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		if (string.IsNullOrWhiteSpace(root))
		{
			IsAvailable = false;
			UnavailableReason = "LocalApplicationData is unavailable; cannot persist DPAPI-protected secrets.";
			_directory = string.Empty;
			_entropy = [];
			return;
		}

		_directory = Path.Combine(root, SanitizePathSegment(applicationName), "os-secrets");
		_entropy = SHA256.HashData(Encoding.UTF8.GetBytes("Dinah.Core.Security.WindowsDpapi:" + applicationName));

		try
		{
			Directory.CreateDirectory(_directory);
			ProbeDpapi();
			IsAvailable = true;
			UnavailableReason = null;
		}
		catch (Exception ex)
		{
			IsAvailable = false;
			UnavailableReason = "Windows DPAPI is unavailable: " + SafeMessage(ex);
		}
	}

	public void Set(string key, ReadOnlySpan<byte> value)
	{
		EnsureAvailable();
		if (!OperatingSystem.IsWindows())
			throw new OsSecretStoreUnavailableException(Name, "Windows DPAPI is only available on Windows.");

		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		if (value.Length == 0)
			throw new ArgumentException("Secret value must not be empty.", nameof(value));

		var protectedBytes = Protect(value.ToArray());
		AtomicFileWriter.WriteAllBytes(GetPath(key), protectedBytes);
	}

	public bool TryGet(string key, out byte[] value)
	{
		EnsureAvailable();
		if (!OperatingSystem.IsWindows())
			throw new OsSecretStoreUnavailableException(Name, "Windows DPAPI is only available on Windows.");

		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));

		var path = GetPath(key);
		if (!File.Exists(path))
		{
			value = [];
			return false;
		}

		try
		{
			var protectedBytes = File.ReadAllBytes(path);
			value = Unprotect(protectedBytes);
			return true;
		}
		catch (CryptographicException ex)
		{
			throw new OsSecretStoreUnavailableException(Name, "Failed to unprotect a DPAPI secret for this Windows user.", ex);
		}
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

		throw new OsSecretStoreUnavailableException(Name, UnavailableReason ?? "Windows DPAPI secret store is unavailable.");
	}

	[SupportedOSPlatform("windows")]
	private void ProbeDpapi()
	{
		var probe = RandomNumberGenerator.GetBytes(16);
		var protectedBytes = Protect(probe);
		var roundTrip = Unprotect(protectedBytes);
		if (!CryptographicOperations.FixedTimeEquals(probe, roundTrip))
			throw new CryptographicException("Windows DPAPI probe failed integrity check.");
	}

	[SupportedOSPlatform("windows")]
	private byte[] Protect(byte[] value)
		=> ProtectedData.Protect(value, _entropy, DataProtectionScope.CurrentUser);

	[SupportedOSPlatform("windows")]
	private byte[] Unprotect(byte[] protectedBytes)
		=> ProtectedData.Unprotect(protectedBytes, _entropy, DataProtectionScope.CurrentUser);

	private string GetPath(string key)
	{
		var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key))).ToLowerInvariant();
		return Path.Combine(_directory, hash + ".dpapi");
	}

	private static string SanitizePathSegment(string value)
	{
		var invalid = Path.GetInvalidFileNameChars();
		var chars = value.Trim().Select(c => invalid.Contains(c) ? '_' : c).ToArray();
		var sanitized = new string(chars);
		return string.IsNullOrWhiteSpace(sanitized) ? "app" : sanitized;
	}

	private static string SafeMessage(Exception ex)
		=> string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message;
}

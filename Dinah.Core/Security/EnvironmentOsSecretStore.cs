#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Portable secret store backed by process environment variables.
/// Values are stored as standard Base64 (not Base64URL). Suitable for Docker/K8s env injection.
/// Not OS-bound: anyone who can read the process environment can read the secrets.
/// </summary>
public sealed class EnvironmentOsSecretStore : IOsSecretStore
{
	private readonly string _environmentVariablePrefix;

	public string Name => "Environment";
	public bool IsAvailable => true;
	public string? UnavailableReason => null;

	/// <summary>
	/// Create a store that maps each secret key to an environment variable named
	/// <c>{environmentVariablePrefix}{SANITIZED_KEY}</c>.
	/// </summary>
	/// <param name="environmentVariablePrefix">
	/// Prefix for env var names (e.g. <c>LIBATION_SECRET_</c>). Must be non-empty.
	/// </param>
	public EnvironmentOsSecretStore(string environmentVariablePrefix)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(environmentVariablePrefix, nameof(environmentVariablePrefix));
		_environmentVariablePrefix = environmentVariablePrefix;
	}

	public void Set(string key, ReadOnlySpan<byte> value)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		if (value.Length == 0)
			throw new ArgumentException("Secret value must not be empty.", nameof(value));

		Environment.SetEnvironmentVariable(GetEnvironmentVariableName(key), Convert.ToBase64String(value));
	}

	public bool TryGet(string key, out byte[] value)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));

		var encoded = Environment.GetEnvironmentVariable(GetEnvironmentVariableName(key));
		if (string.IsNullOrEmpty(encoded))
		{
			value = [];
			return false;
		}

		try
		{
			value = Convert.FromBase64String(encoded);
		}
		catch (FormatException ex)
		{
			throw new OsSecretStoreUnavailableException(Name, "Stored secret was not valid base64.", ex);
		}

		if (value.Length == 0)
		{
			value = [];
			return false;
		}

		return true;
	}

	public void Delete(string key)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		Environment.SetEnvironmentVariable(GetEnvironmentVariableName(key), null);
	}

	/// <summary>Resolve the environment variable name used for <paramref name="key"/>.</summary>
	public string GetEnvironmentVariableName(string key)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		return _environmentVariablePrefix + SanitizeKey(key);
	}

	private static string SanitizeKey(string key)
	{
		var chars = key.Trim().Select(c => char.IsAsciiLetterOrDigit(c) ? char.ToUpperInvariant(c) : '_').ToArray();
		var sanitized = new string(chars);
		return string.IsNullOrWhiteSpace(sanitized) ? "KEY" : sanitized;
	}
}

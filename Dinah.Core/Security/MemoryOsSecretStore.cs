#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// In-memory secret store for tests. Not for production persistence.
/// </summary>
public sealed class MemoryOsSecretStore : IOsSecretStore
{
	private readonly Dictionary<string, byte[]> _secrets = new(StringComparer.Ordinal);

	public string Name => "Memory";
	public bool IsAvailable => true;
	public string? UnavailableReason => null;

	public void Set(string key, ReadOnlySpan<byte> value)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		if (value.Length == 0)
			throw new ArgumentException("Secret value must not be empty.", nameof(value));

		_secrets[key] = value.ToArray();
	}

	public bool TryGet(string key, out byte[] value)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		if (_secrets.TryGetValue(key, out var stored))
		{
			value = stored.ToArray();
			return true;
		}

		value = [];
		return false;
	}

	public void Delete(string key)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		_secrets.Remove(key);
	}
}

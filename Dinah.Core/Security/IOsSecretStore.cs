#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// OS-bound secret storage for small secrets such as encryption master keys.
/// Implementations must use a real platform secret store and must not fall back to plaintext.
/// </summary>
public interface IOsSecretStore
{
	/// <summary>Human-readable store name for errors and diagnostics (never includes secret values).</summary>
	string Name { get; }

	/// <summary>True when the store was probed successfully and may be used.</summary>
	bool IsAvailable { get; }

	/// <summary>When <see cref="IsAvailable"/> is false, a safe explanation with no secret material.</summary>
	string? UnavailableReason { get; }

	/// <summary>Store or replace a secret value for <paramref name="key"/>.</summary>
	void Set(string key, ReadOnlySpan<byte> value);

	/// <summary>Try to read a previously stored secret.</summary>
	bool TryGet(string key, out byte[] value);

	/// <summary>Delete a secret if present. No-op when missing.</summary>
	void Delete(string key);
}

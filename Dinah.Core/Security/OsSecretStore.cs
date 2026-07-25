#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Factory for the platform OS secret store. Fail-closed: never returns a plaintext-backed store.
/// </summary>
public static class OsSecretStore
{
	/// <summary>
	/// Create the default OS-bound secret store for <paramref name="applicationName"/>.
	/// Callers must check <see cref="IOsSecretStore.IsAvailable"/> before encrypting.
	/// </summary>
	public static IOsSecretStore Create(string applicationName)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(applicationName, nameof(applicationName));

		if (OperatingSystem.IsWindows())
			return new WindowsDpapiOsSecretStore(applicationName);

		if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
			return CredentialManagerOsSecretStore.Create(applicationName);

		return new UnavailableOsSecretStore(
			"Unsupported OS",
			$"No OS secret store is implemented for '{System.Runtime.InteropServices.RuntimeInformation.OSDescription}'.");
	}
}

file sealed class UnavailableOsSecretStore : IOsSecretStore
{
	public UnavailableOsSecretStore(string name, string reason)
	{
		Name = name;
		UnavailableReason = reason;
	}

	public string Name { get; }
	public bool IsAvailable => false;
	public string? UnavailableReason { get; }

	public void Set(string key, ReadOnlySpan<byte> value)
		=> throw new OsSecretStoreUnavailableException(Name, UnavailableReason!);

	public bool TryGet(string key, out byte[] value)
		=> throw new OsSecretStoreUnavailableException(Name, UnavailableReason!);

	public void Delete(string key)
		=> throw new OsSecretStoreUnavailableException(Name, UnavailableReason!);
}

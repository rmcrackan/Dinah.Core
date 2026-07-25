using GitCredentialManager;

#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// macOS Keychain / Linux Secret Service store via Git Credential Manager backends.
/// Only allowlisted secure backends are used; plaintext/cache/none are rejected.
/// </summary>
public sealed class CredentialManagerOsSecretStore : IOsSecretStore
{
	private const string ServicePrefix = "Dinah.Core.OsSecretStore";

	private readonly ICredentialStore? _store;
	private readonly string _service;

	public string Name { get; }
	public bool IsAvailable { get; }
	public string? UnavailableReason { get; }

	private CredentialManagerOsSecretStore(
		string name,
		string applicationName,
		ICredentialStore? store,
		bool isAvailable,
		string? unavailableReason)
	{
		Name = name;
		_service = ServicePrefix + ":" + applicationName;
		_store = store;
		IsAvailable = isAvailable;
		UnavailableReason = unavailableReason;
	}

	/// <summary>
	/// Create a store for the current OS. On unsupported platforms, returns an unavailable instance.
	/// </summary>
	public static CredentialManagerOsSecretStore Create(string applicationName)
	{
		ArgumentValidator.EnsureNotNullOrWhiteSpace(applicationName, nameof(applicationName));

		string requiredBackend;
		string name;
		if (OperatingSystem.IsMacOS())
		{
			requiredBackend = "keychain";
			name = "macOS Keychain";
		}
		else if (OperatingSystem.IsLinux())
		{
			requiredBackend = "secretservice";
			name = "Linux Secret Service";
		}
		else
		{
			return new CredentialManagerOsSecretStore(
				"Credential Manager",
				applicationName,
				store: null,
				isAvailable: false,
				unavailableReason: "CredentialManagerOsSecretStore is only used on macOS and Linux.");
		}

		var previous = Environment.GetEnvironmentVariable("GCM_CREDENTIAL_STORE");
		try
		{
			// Force a secure backend so git/user plaintext or cache settings cannot win.
			Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", requiredBackend);

			var context = CredentialManager.CreateContext(applicationName);
			var configured = context.Settings.CredentialBackingStore?.Trim().ToLowerInvariant();
			if (!string.IsNullOrEmpty(configured) && configured != requiredBackend)
			{
				return new CredentialManagerOsSecretStore(
					name,
					applicationName,
					store: null,
					isAvailable: false,
					unavailableReason: $"{name} refused insecure or unexpected backing store '{configured}'.");
			}

			if (IsInsecureBackend(configured))
			{
				return new CredentialManagerOsSecretStore(
					name,
					applicationName,
					store: null,
					isAvailable: false,
					unavailableReason: $"{name} refused insecure backing store '{configured}'.");
			}

			var store = context.CredentialStore;
			// Force backend initialization; throws when Secret Service / Keychain is unavailable.
			_ = store.GetAccounts(ServicePrefix + ":probe:" + applicationName);

			return new CredentialManagerOsSecretStore(
				name,
				applicationName,
				store,
				isAvailable: true,
				unavailableReason: null);
		}
		catch (Exception ex)
		{
			return new CredentialManagerOsSecretStore(
				name,
				applicationName,
				store: null,
				isAvailable: false,
				unavailableReason: $"{name} is unavailable: {SafeMessage(ex)}");
		}
		finally
		{
			Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", previous);
		}
	}

	public void Set(string key, ReadOnlySpan<byte> value)
	{
		var store = EnsureStore();
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		if (value.Length == 0)
			throw new ArgumentException("Secret value must not be empty.", nameof(value));

		store.AddOrUpdate(_service, key, Encode(value));
	}

	public bool TryGet(string key, out byte[] value)
	{
		var store = EnsureStore();
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));

		var credential = store.Get(_service, key);
		if (credential?.Password is null)
		{
			value = [];
			return false;
		}

		try
		{
			value = Decode(credential.Password);
			return true;
		}
		catch (FormatException ex)
		{
			throw new OsSecretStoreUnavailableException(Name, "Stored secret was not valid base64.", ex);
		}
	}

	public void Delete(string key)
	{
		var store = EnsureStore();
		ArgumentValidator.EnsureNotNullOrWhiteSpace(key, nameof(key));
		store.Remove(_service, key);
	}

	private ICredentialStore EnsureStore()
	{
		if (IsAvailable && _store is not null)
			return _store;

		throw new OsSecretStoreUnavailableException(Name, UnavailableReason ?? $"{Name} is unavailable.");
	}

	private static bool IsInsecureBackend(string? backend)
		=> backend is "plaintext" or "cache" or "none";

	private static string Encode(ReadOnlySpan<byte> value)
		=> Convert.ToBase64String(value);

	private static byte[] Decode(string value)
		=> Convert.FromBase64String(value);

	private static string SafeMessage(Exception ex)
		=> string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message;
}

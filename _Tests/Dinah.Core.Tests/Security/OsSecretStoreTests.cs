using System.Security.Cryptography;
using Dinah.Core.Security;

#nullable enable
namespace OsSecretStoreTests;

[TestClass]
public class MemoryStore
{
	[TestMethod]
	public void set_get_delete()
	{
		var store = new MemoryOsSecretStore();
		store.IsAvailable.ShouldBeTrue();

		store.Set("k", "value"u8);
		store.TryGet("k", out var value).ShouldBeTrue();
		value.ShouldBe("value"u8.ToArray());

		store.Delete("k");
		store.TryGet("k", out _).ShouldBeFalse();
	}
}

[TestClass]
public class Factory
{
	[TestMethod]
	public void create_returns_platform_store()
	{
		var store = OsSecretStore.Create("Dinah.Core.Tests");
		store.ShouldNotBeNull();
		store.Name.ShouldNotBeNullOrWhiteSpace();

		if (OperatingSystem.IsWindows())
		{
			store.ShouldBeOfType<WindowsDpapiOsSecretStore>();
			store.IsAvailable.ShouldBeTrue();
		}
	}
}

[TestClass]
public class WindowsDpapi
{
	[TestMethod]
	public void roundtrip_on_windows()
	{
		if (!OperatingSystem.IsWindows())
			Assert.Inconclusive("Windows-only test");

		var appName = "Dinah.Core.Tests." + Guid.NewGuid().ToString("N");
		var store = new WindowsDpapiOsSecretStore(appName);
		store.IsAvailable.ShouldBeTrue();

		var key = "master-" + Guid.NewGuid().ToString("N");
		var secret = RandomNumberGenerator.GetBytes(32);

		try
		{
			store.Set(key, secret);
			store.TryGet(key, out var loaded).ShouldBeTrue();
			loaded.ShouldBe(secret);

			// New instance must still decrypt for the same Windows user.
			var store2 = new WindowsDpapiOsSecretStore(appName);
			store2.TryGet(key, out var loaded2).ShouldBeTrue();
			loaded2.ShouldBe(secret);
		}
		finally
		{
			try { store.Delete(key); } catch { /* cleanup */ }
		}
	}

	[TestMethod]
	public void unavailable_off_windows()
	{
		if (OperatingSystem.IsWindows())
			Assert.Inconclusive("Non-Windows test");

		var store = new WindowsDpapiOsSecretStore("x");
		store.IsAvailable.ShouldBeFalse();
		Should.Throw<OsSecretStoreUnavailableException>(() => store.Set("k", "v"u8));
	}
}

[TestClass]
public class CredentialManagerStore
{
	[TestMethod]
	public void create_on_windows_is_marked_unavailable_for_this_type()
	{
		if (!OperatingSystem.IsWindows())
			Assert.Inconclusive("Windows-only test");

		var store = CredentialManagerOsSecretStore.Create("Dinah.Core.Tests");
		store.IsAvailable.ShouldBeFalse();
		store.UnavailableReason.ShouldNotBeNullOrWhiteSpace();
	}

	[TestMethod]
	public void create_on_unix_does_not_use_insecure_backends()
	{
		if (OperatingSystem.IsWindows())
			Assert.Inconclusive("Unix-only test");

		var store = CredentialManagerOsSecretStore.Create("Dinah.Core.Tests." + Guid.NewGuid().ToString("N"));
		// May be available (desktop + keychain/secretservice) or unavailable (headless) - never plaintext.
		if (store.IsAvailable)
		{
			var key = "probe-" + Guid.NewGuid().ToString("N");
			var secret = "unit-test-secret"u8.ToArray();
			try
			{
				store.Set(key, secret);
				store.TryGet(key, out var loaded).ShouldBeTrue();
				loaded.ShouldBe(secret);
			}
			finally
			{
				try { store.Delete(key); } catch { /* cleanup */ }
			}
		}
		else
		{
			store.UnavailableReason.ShouldNotBeNullOrWhiteSpace();
			store.UnavailableReason!.ToLowerInvariant().ShouldNotContain("plaintext");
			Should.Throw<OsSecretStoreUnavailableException>(() => store.Set("k", "v"u8));
		}
	}
}

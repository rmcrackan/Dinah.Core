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
public class FileStore
{
	[TestMethod]
	public void set_get_delete()
	{
		var dir = Path.Combine(Path.GetTempPath(), "Dinah.Core.Tests.FileOsSecretStore." + Guid.NewGuid().ToString("N"));
		try
		{
			var store = new FileOsSecretStore(dir);
			store.IsAvailable.ShouldBeTrue();

			store.Set("aes-gcm-master-key-v1", "value"u8);
			store.TryGet("aes-gcm-master-key-v1", out var value).ShouldBeTrue();
			value.ShouldBe("value"u8.ToArray());

			Directory.EnumerateFiles(dir, "*.secret").Count().ShouldBe(1);

			store.Delete("aes-gcm-master-key-v1");
			store.TryGet("aes-gcm-master-key-v1", out _).ShouldBeFalse();
		}
		finally
		{
			if (Directory.Exists(dir))
				Directory.Delete(dir, recursive: true);
		}
	}

	[TestMethod]
	public void rejects_empty_value()
	{
		var dir = Path.Combine(Path.GetTempPath(), "Dinah.Core.Tests.FileOsSecretStore." + Guid.NewGuid().ToString("N"));
		try
		{
			var store = new FileOsSecretStore(dir);
			Should.Throw<ArgumentException>(() => store.Set("k", ReadOnlySpan<byte>.Empty));
		}
		finally
		{
			if (Directory.Exists(dir))
				Directory.Delete(dir, recursive: true);
		}
	}
}

[TestClass]
public class EnvironmentStore
{
	[TestMethod]
	public void set_get_delete()
	{
		var prefix = "DINAH_CORE_TEST_SECRET_" + Guid.NewGuid().ToString("N") + "_";
		var store = new EnvironmentOsSecretStore(prefix);
		var envName = store.GetEnvironmentVariableName("aes-gcm-master-key-v1");
		envName.ShouldBe(prefix + "AES_GCM_MASTER_KEY_V1");

		try
		{
			store.Set("aes-gcm-master-key-v1", "value"u8);
			store.TryGet("aes-gcm-master-key-v1", out var value).ShouldBeTrue();
			value.ShouldBe("value"u8.ToArray());

			Environment.GetEnvironmentVariable(envName).ShouldBe(Convert.ToBase64String("value"u8.ToArray()));

			store.Delete("aes-gcm-master-key-v1");
			store.TryGet("aes-gcm-master-key-v1", out _).ShouldBeFalse();
		}
		finally
		{
			Environment.SetEnvironmentVariable(envName, null);
		}
	}

	[TestMethod]
	public void rejects_empty_value()
	{
		var store = new EnvironmentOsSecretStore("DINAH_CORE_TEST_EMPTY_");
		Should.Throw<ArgumentException>(() => store.Set("k", ReadOnlySpan<byte>.Empty));
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

	/// <summary>
	/// Runs everywhere, because <see cref="CredentialManagerOsSecretStore.Create"/> bounds how long it waits for
	/// the backend. It used to hang here instead of reporting anything.
	/// </summary>
	[TestMethod]
	public void create_on_unix_reports_availability_and_never_plaintext()
	{
		if (OperatingSystem.IsWindows())
			Assert.Inconclusive("Unix-only test");

		var store = CredentialManagerOsSecretStore.Create("Dinah.Core.Tests." + Guid.NewGuid().ToString("N"));

		// available on a desktop with an answering keychain, unavailable on a headless box - never plaintext
		if (store.IsAvailable)
			store.Name.ShouldNotBeNullOrWhiteSpace();
		else
		{
			store.UnavailableReason.ShouldNotBeNullOrWhiteSpace();
			store.UnavailableReason!.ToLowerInvariant().ShouldNotContain("plaintext");
			Should.Throw<OsSecretStoreUnavailableException>(() => store.Set("k", "v"u8));
		}
	}

	/// <summary>
	/// Storing a secret is opt-in, and unlike the availability check it cannot be bounded. Writing a new item to
	/// a locked keyring asks its owner to unlock it, and waiting on a person is legitimate - a timeout here would
	/// break the desktop case it exists for. So this hangs on a machine whose keyring nobody can unlock, which is
	/// why it only runs when asked for.
	/// <para>
	/// To run it: <c>DINAH_TEST_OS_SECRET_STORE=1 dotnet test</c> (PowerShell:
	/// <c>$env:DINAH_TEST_OS_SECRET_STORE = '1'; dotnet test</c>), on a machine where you can answer the prompt.
	/// </para>
	/// </summary>
	[TestMethod]
	public void a_secret_round_trips_through_the_real_store_on_unix()
	{
		if (OperatingSystem.IsWindows())
			Assert.Inconclusive("Unix-only test");

		SkipUnlessWritingToTheRealStoreWasRequested();

		var store = CredentialManagerOsSecretStore.Create("Dinah.Core.Tests." + Guid.NewGuid().ToString("N"));
		if (!store.IsAvailable)
			Assert.Inconclusive($"No usable OS secret store here: {store.UnavailableReason}");

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

	private const string WriteOptInVariable = "DINAH_TEST_OS_SECRET_STORE";

	private static void SkipUnlessWritingToTheRealStoreWasRequested()
	{
		var value = Environment.GetEnvironmentVariable(WriteOptInVariable);
		if (value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
			return;

		Assert.Inconclusive(
			$"""
			Skipped: this test writes to the real OS secret store, which asks a locked keyring's owner to unlock
			it and waits indefinitely for an answer. To run it, on a machine where you can answer that prompt:
			    bash/zsh:   {WriteOptInVariable}=1 dotnet test
			    PowerShell: $env:{WriteOptInVariable} = '1'; dotnet test
			""");
	}
}

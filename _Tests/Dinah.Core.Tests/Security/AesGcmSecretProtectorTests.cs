using Dinah.Core.Security;

#nullable enable
namespace AesGcmSecretProtectorTests;

[TestClass]
public class ProtectUnprotect
{
	[TestMethod]
	public void roundtrip_with_memory_store()
	{
		var store = new MemoryOsSecretStore();
		var protector = new AesGcmSecretProtector(store);

		var payload = protector.Protect("secret-token-value", associatedData: "account|RefreshToken");
		payload.ShouldStartWith("v1.");
		payload.ShouldNotContain("secret-token-value");

		protector.Unprotect(payload, associatedData: "account|RefreshToken").ShouldBe("secret-token-value");
	}

	[TestMethod]
	public void encryption_reuses_same_master_key()
	{
		var store = new MemoryOsSecretStore();
		var protector = new AesGcmSecretProtector(store);

		_ = protector.Protect("a");
		_ = protector.Protect("b");

		store.TryGet("aes-gcm-master-key-v1", out var key).ShouldBeTrue();
		key.Length.ShouldBe(AesGcmSecretProtector.KeySizeBytes);
	}

	[TestMethod]
	public void wrong_aad_fails_without_returning_plaintext()
	{
		var protector = new AesGcmSecretProtector(new MemoryOsSecretStore());
		var payload = protector.Protect("token-value-xyz", associatedData: "aad-a");

		var ex = Should.Throw<SecretProtectionException>(() => protector.Unprotect(payload, associatedData: "aad-b"));
		ex.Message.ShouldNotContain("token-value-xyz");
		ex.ToString().ShouldNotContain("token-value-xyz");
	}

	[TestMethod]
	public void tampered_ciphertext_fails()
	{
		var protector = new AesGcmSecretProtector(new MemoryOsSecretStore());
		var payload = protector.Protect("token-value-xyz");
		var parts = payload.Split('.');
		var ciphertext = Convert.FromBase64String(Pad(parts[2]));
		ciphertext[0] ^= 0xFF;
		var tampered = $"{parts[0]}.{parts[1]}.{Unpad(Convert.ToBase64String(ciphertext))}.{parts[3]}";

		var ex = Should.Throw<SecretProtectionException>(() => protector.Unprotect(tampered));
		ex.Message.ShouldNotContain("token-value-xyz");
	}

	[TestMethod]
	public void malformed_payload_fails_safely()
	{
		var protector = new AesGcmSecretProtector(new MemoryOsSecretStore());
		Should.Throw<SecretProtectionException>(() => protector.Unprotect("not-a-payload"));
		Should.Throw<SecretProtectionException>(() => protector.Unprotect("v2.a.b.c"));
	}

	[TestMethod]
	public void unavailable_store_fails_closed()
	{
		var store = new UnavailableTestStore();
		var protector = new AesGcmSecretProtector(store);

		var ex = Should.Throw<OsSecretStoreUnavailableException>(() => protector.Protect("x"));
		ex.StoreName.ShouldBe("UnavailableTest");
		ex.Message.ShouldContain("no desktop session");
		ex.Message.ShouldNotContain("x");
	}

	static string Pad(string base64Url)
	{
		var padded = base64Url.Replace('-', '+').Replace('_', '/');
		return (padded.Length % 4) switch
		{
			2 => padded + "==",
			3 => padded + "=",
			_ => padded
		};
	}

	static string Unpad(string base64)
		=> base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
}

file sealed class UnavailableTestStore : IOsSecretStore
{
	public string Name => "UnavailableTest";
	public bool IsAvailable => false;
	public string? UnavailableReason => "no desktop session";

	public void Set(string key, ReadOnlySpan<byte> value)
		=> throw new OsSecretStoreUnavailableException(Name, UnavailableReason!);

	public bool TryGet(string key, out byte[] value)
		=> throw new OsSecretStoreUnavailableException(Name, UnavailableReason!);

	public void Delete(string key)
		=> throw new OsSecretStoreUnavailableException(Name, UnavailableReason!);
}

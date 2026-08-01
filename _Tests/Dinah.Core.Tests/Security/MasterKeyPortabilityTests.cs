using Dinah.Core.Security;

#nullable enable
namespace MasterKeyPortabilityTests;

[TestClass]
public class CopyExportImport
{
	[TestMethod]
	public void copy_between_stores_preserves_decrypt()
	{
		var source = new MemoryOsSecretStore();
		var protector = new AesGcmSecretProtector(source);
		var payload = protector.Protect("secret-token-value", associatedData: "aad");

		var destination = new MemoryOsSecretStore();
		MasterKeyPortability.Copy(source, destination);

		var imported = new AesGcmSecretProtector(destination);
		imported.Unprotect(payload, associatedData: "aad").ShouldBe("secret-token-value");
	}

	[TestMethod]
	public void export_import_file_preserves_decrypt()
	{
		var source = new MemoryOsSecretStore();
		var protector = new AesGcmSecretProtector(source);
		var payload = protector.Protect("secret-token-value", associatedData: "aad");

		var path = Path.Combine(Path.GetTempPath(), "Dinah.Core.Tests.MasterKey." + Guid.NewGuid().ToString("N") + ".key");
		try
		{
			MasterKeyPortability.ExportToFile(source, path);

			var destination = new MemoryOsSecretStore();
			MasterKeyPortability.ImportFromFile(destination, path);

			var imported = new AesGcmSecretProtector(destination);
			imported.Unprotect(payload, associatedData: "aad").ShouldBe("secret-token-value");
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}

	[TestMethod]
	public void export_to_file_store_directory_preserves_decrypt()
	{
		var source = new MemoryOsSecretStore();
		var protector = new AesGcmSecretProtector(source);
		var payload = protector.Protect("secret-token-value", associatedData: "aad");

		var dir = Path.Combine(Path.GetTempPath(), "Dinah.Core.Tests.MasterKeyDir." + Guid.NewGuid().ToString("N"));
		try
		{
			var fileStore = new FileOsSecretStore(dir);
			MasterKeyPortability.Copy(source, fileStore);

			var imported = new AesGcmSecretProtector(fileStore);
			imported.Unprotect(payload, associatedData: "aad").ShouldBe("secret-token-value");
		}
		finally
		{
			if (Directory.Exists(dir))
				Directory.Delete(dir, recursive: true);
		}
	}

	[TestMethod]
	public void copy_to_environment_store_preserves_decrypt()
	{
		var source = new MemoryOsSecretStore();
		var protector = new AesGcmSecretProtector(source);
		var payload = protector.Protect("secret-token-value", associatedData: "aad");

		var prefix = "DINAH_CORE_TEST_MASTER_" + Guid.NewGuid().ToString("N") + "_";
		var envStore = new EnvironmentOsSecretStore(prefix);
		var envName = envStore.GetEnvironmentVariableName(AesGcmSecretProtector.DefaultMasterKeyName);
		try
		{
			MasterKeyPortability.Copy(source, envStore);

			var imported = new AesGcmSecretProtector(envStore);
			imported.Unprotect(payload, associatedData: "aad").ShouldBe("secret-token-value");
		}
		finally
		{
			Environment.SetEnvironmentVariable(envName, null);
		}
	}

	[TestMethod]
	public void copy_fails_closed_when_master_key_missing()
	{
		var source = new MemoryOsSecretStore();
		var destination = new MemoryOsSecretStore();

		var ex = Should.Throw<SecretProtectionException>(() => MasterKeyPortability.Copy(source, destination));
		ex.Message.ShouldContain("was not found");
		destination.TryGet(AesGcmSecretProtector.DefaultMasterKeyName, out _).ShouldBeFalse();
	}

	[TestMethod]
	public void export_fails_closed_when_master_key_missing()
	{
		var source = new MemoryOsSecretStore();
		var path = Path.Combine(Path.GetTempPath(), "Dinah.Core.Tests.MasterKeyMissing." + Guid.NewGuid().ToString("N") + ".key");

		try
		{
			Should.Throw<SecretProtectionException>(() => MasterKeyPortability.ExportToFile(source, path));
			File.Exists(path).ShouldBeFalse();
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}

	[TestMethod]
	public void import_rejects_wrong_length_key_file()
	{
		var path = Path.Combine(Path.GetTempPath(), "Dinah.Core.Tests.MasterKeyBad." + Guid.NewGuid().ToString("N") + ".key");
		var destination = new MemoryOsSecretStore();
		try
		{
			File.WriteAllBytes(path, new byte[8]);
			Should.Throw<SecretProtectionException>(() => MasterKeyPortability.ImportFromFile(destination, path));
			destination.TryGet(AesGcmSecretProtector.DefaultMasterKeyName, out _).ShouldBeFalse();
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}

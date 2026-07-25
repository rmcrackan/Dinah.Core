using Dinah.Core.IO;

#nullable enable
namespace AtomicFileWriterTests;

[TestClass]
public class WriteAllText
{
	string? _dir;

	[TestInitialize]
	public void Init()
	{
		_dir = Path.Combine(Path.GetTempPath(), "dinah-atomic-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_dir);
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (_dir is not null && Directory.Exists(_dir))
			Directory.Delete(_dir, recursive: true);
	}

	string PathInDir(string name) => Path.Combine(_dir!, name);

	[TestMethod]
	public void creates_new_file()
	{
		var path = PathInDir("new.json");
		File.Exists(path).ShouldBeFalse();

		AtomicFileWriter.WriteAllText(path, "{\"a\":1}");

		File.ReadAllText(path).ShouldBe("{\"a\":1}");
		Directory.GetFiles(_dir!, "*.tmp").ShouldBeEmpty();
	}

	[TestMethod]
	public void replaces_existing_file()
	{
		var path = PathInDir("existing.json");
		File.WriteAllText(path, "original");

		AtomicFileWriter.WriteAllText(path, "replacement");

		File.ReadAllText(path).ShouldBe("replacement");
		Directory.GetFiles(_dir!, "*.tmp").ShouldBeEmpty();
	}

	[TestMethod]
	public void failed_validation_preserves_original_and_cleans_temp()
	{
		var path = PathInDir("validate.json");
		File.WriteAllText(path, "keep-me");

		Should.Throw<InvalidOperationException>(() =>
			AtomicFileWriter.WriteAllText(path, "do-not-write", _ => throw new InvalidOperationException("bad temp")));

		File.ReadAllText(path).ShouldBe("keep-me");
		Directory.GetFiles(_dir!, "*.tmp").ShouldBeEmpty();
	}

	[TestMethod]
	public void validation_sees_temp_contents_before_replace()
	{
		var path = PathInDir("peek.json");
		File.WriteAllText(path, "old");
		string? seen = null;

		AtomicFileWriter.WriteAllText(path, "new-contents", temp =>
		{
			seen = File.ReadAllText(temp);
			File.Exists(path).ShouldBeTrue();
			File.ReadAllText(path).ShouldBe("old");
		});

		seen.ShouldBe("new-contents");
		File.ReadAllText(path).ShouldBe("new-contents");
	}

	[TestMethod]
	public void creates_missing_directory()
	{
		var path = PathInDir(Path.Combine("sub", "nested", "file.txt"));
		AtomicFileWriter.WriteAllText(path, "nested");
		File.ReadAllText(path).ShouldBe("nested");
	}
}

[TestClass]
public class CreateBackup
{
	string? _dir;

	[TestInitialize]
	public void Init()
	{
		_dir = Path.Combine(Path.GetTempPath(), "dinah-bak-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_dir);
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (_dir is not null && Directory.Exists(_dir))
			Directory.Delete(_dir, recursive: true);
	}

	[TestMethod]
	public void missing_source_throws()
	{
		var path = Path.Combine(_dir!, "missing.json");
		Should.Throw<FileNotFoundException>(() => AtomicFileWriter.CreateBackup(path));
	}

	[TestMethod]
	public void backup_matches_source_and_leaves_source_intact()
	{
		var path = Path.Combine(_dir!, "AccountsSettings.json");
		const string contents = "{\n  \"Accounts\": []\n}";
		File.WriteAllText(path, contents);

		var backupPath = AtomicFileWriter.CreateBackup(path);

		File.Exists(backupPath).ShouldBeTrue();
		backupPath.ShouldNotBe(path);
		Path.GetDirectoryName(backupPath).ShouldBe(Path.GetDirectoryName(path));
		File.ReadAllText(backupPath).ShouldBe(contents);
		File.ReadAllText(path).ShouldBe(contents);
		backupPath.ShouldEndWith(".bak");
	}

	[TestMethod]
	public void successive_backups_use_unique_paths()
	{
		var path = Path.Combine(_dir!, "file.json");
		File.WriteAllText(path, "v1");

		var bak1 = AtomicFileWriter.CreateBackup(path);
		var bak2 = AtomicFileWriter.CreateBackup(path);

		bak1.ShouldNotBe(bak2);
		File.Exists(bak1).ShouldBeTrue();
		File.Exists(bak2).ShouldBeTrue();
	}
}

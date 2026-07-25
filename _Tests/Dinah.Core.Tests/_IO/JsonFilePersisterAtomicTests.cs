using Dinah.Core.IO;

#nullable enable
namespace JsonFilePersisterTests;

file class TestSettings : IUpdatable
{
	public event EventHandler? Updated;

	public string Name
	{
		get => field;
		set
		{
			if (field == value)
				return;
			field = value;
			Updated?.Invoke(this, EventArgs.Empty);
		}
	} = "";
}

file class TestPersister : JsonFilePersister<TestSettings>
{
	public TestPersister(TestSettings target, string path) : base(target, path) { }
	public TestPersister(string path) : base(path) { }
}

[TestClass]
public class AtomicPersistence
{
	string? _dir;

	[TestInitialize]
	public void Init()
	{
		_dir = Path.Combine(Path.GetTempPath(), "dinah-persister-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_dir);
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (_dir is not null && Directory.Exists(_dir))
			Directory.Delete(_dir, recursive: true);
	}

	[TestMethod]
	public void create_and_update_writes_readable_json_without_tmp_leftovers()
	{
		var path = Path.Combine(_dir!, "settings.json");

		using (var p = new TestPersister(new TestSettings { Name = "one" }, path))
		{
			File.Exists(path).ShouldBeTrue();
			p.Target.Name = "two";
		}

		var json = File.ReadAllText(path);
		JObject.Parse(json)["Name"]!.Value<string>().ShouldBe("two");
		Directory.GetFiles(_dir!, "*.tmp").ShouldBeEmpty();
		Directory.GetFiles(_dir!, "*.bak").ShouldBeEmpty();
	}

	[TestMethod]
	public void load_existing_roundtrips()
	{
		var path = Path.Combine(_dir!, "settings.json");
		using (new TestPersister(new TestSettings { Name = "persist-me" }, path)) { }

		using var loaded = new TestPersister(path);
		loaded.Target.Name.ShouldBe("persist-me");
	}
}

[TestClass]
public class JsonHelperUpdateJsonFile
{
	string? _dir;

	[TestInitialize]
	public void Init()
	{
		_dir = Path.Combine(Path.GetTempPath(), "dinah-jsonhelper-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_dir);
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (_dir is not null && Directory.Exists(_dir))
			Directory.Delete(_dir, recursive: true);
	}

	[TestMethod]
	public void update_replaces_atomically_and_preserves_on_failed_write_validation_path()
	{
		var path = Path.Combine(_dir!, "doc.json");
		File.WriteAllText(path, """{"x":1}""");

		JsonHelper.UpdateJsonFile(path, jo => jo["x"] = 2);

		JObject.Parse(File.ReadAllText(path))["x"]!.Value<int>().ShouldBe(2);
		Directory.GetFiles(_dir!, "*.tmp").ShouldBeEmpty();
	}
}

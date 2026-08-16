using System.Reflection;
using Dinah.Core.Security;
using Serilog;
using Serilog.Core;
using Serilog.Events;

#nullable enable
namespace SecretStringTests;

[TestClass]
public class ToStringRedaction
{
	[TestMethod]
	public void keeps_the_length_and_nothing_else()
	{
		var value = "activation-bytes";

		var text = new SecretString(value).ToString();

		text.ShouldBe($"[REDACTED length={value.Length}]");
		text.ShouldNotContain(value);
	}

	[TestMethod]
	public void null_is_distinguishable_from_empty()
	{
		new SecretString(null).ToString().ShouldBe("[REDACTED <null>]");
		new SecretString("").ToString().ShouldBe("[REDACTED <empty>]");
	}

	[TestMethod]
	public void interpolation_cannot_leak()
	{
		SecretString secret = "Atnr|foo";

		$"token={secret}".ShouldBe("token=[REDACTED length=8]");
	}
}

[TestClass]
public class RedactFormat
{
	/// <summary>
	/// The labelled form is what a type wrapping a secret uses for its own ToString, so it has to produce
	/// exactly what those hand-written overrides produce today.
	/// </summary>
	[TestMethod]
	public void labelled_form_matches_a_hand_written_token_override()
	{
		SecretString.Redact("RefreshToken", "Atnr|foo").ShouldBe("RefreshToken [REDACTED length=8]");
		SecretString.Redact("PrivateKey", null).ShouldBe("PrivateKey [REDACTED <null>]");
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow("")]
	[DataRow("   ")]
	public void no_label_leaves_the_bare_redaction(string? label)
		=> SecretString.Redact(label, "abc").ShouldBe("[REDACTED length=3]");
}

[TestClass]
public class Reveal
{
	[TestMethod]
	public void round_trips_the_value() => new SecretString("abc").Reveal().ShouldBe("abc");

	[TestMethod]
	public void keeps_null_as_null() => new SecretString(null).Reveal().ShouldBeNull();

	[TestMethod]
	public void a_string_converts_implicitly()
	{
		SecretString secret = "abc";

		secret.Reveal().ShouldBe("abc");
	}

	[TestMethod]
	[DataRow(null, false)]
	[DataRow("", false)]
	[DataRow(" ", true)]
	[DataRow("abc", true)]
	public void has_value_reports_whether_anything_is_held(string? value, bool expected)
		=> new SecretString(value).HasValue.ShouldBe(expected);
}

[TestClass]
public class Equality
{
	[TestMethod]
	public void same_value_is_equal()
	{
		new SecretString("abc").Equals(new SecretString("abc")).ShouldBeTrue();
		(new SecretString("abc") == new SecretString("abc")).ShouldBeTrue();
		new SecretString("abc").GetHashCode().ShouldBe(new SecretString("abc").GetHashCode());
	}

	[TestMethod]
	public void different_value_is_not_equal()
	{
		(new SecretString("abc") != new SecretString("abd")).ShouldBeTrue();
		(new SecretString(null) == new SecretString("")).ShouldBeFalse();
	}

	[TestMethod]
	public void null_equals_null() => (new SecretString(null) == new SecretString(null)).ShouldBeTrue();
}

[TestClass]
public class Json
{
	private class Model
	{
		public SecretString Secret { get; set; }
	}

	[TestMethod]
	public void persists_as_a_bare_string()
		=> JsonConvert.SerializeObject(new Model { Secret = "abc" }).ShouldBe(@"{""Secret"":""abc""}");

	[TestMethod]
	public void reads_back_what_it_wrote()
	{
		var json = JsonConvert.SerializeObject(new Model { Secret = "abc" });

		JsonConvert.DeserializeObject<Model>(json)!.Secret.Reveal().ShouldBe("abc");
	}

	[TestMethod]
	public void a_string_written_before_the_property_became_a_secret_still_loads()
		=> JsonConvert.DeserializeObject<Model>(@"{""Secret"":""abc""}")!.Secret.Reveal().ShouldBe("abc");

	[TestMethod]
	public void null_survives_a_round_trip()
	{
		var json = JsonConvert.SerializeObject(new Model());
		json.ShouldBe(@"{""Secret"":null}");

		JsonConvert.DeserializeObject<Model>(json)!.Secret.Reveal().ShouldBeNull();
	}
}

/// <summary>
/// The reason the type is safe in a log: the loggers that leak secrets do it by reflecting over public
/// members, and there is no public member holding the value.
/// </summary>
[TestClass]
public class ReflectionSurface
{
	[TestMethod]
	public void no_public_property_exposes_the_value()
		=> typeof(SecretString)
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Select(p => p.Name)
			.ShouldBe([nameof(SecretString.HasValue)]);

	[TestMethod]
	public void no_public_field_exposes_the_value()
		=> typeof(SecretString)
			.GetFields(BindingFlags.Public | BindingFlags.Instance)
			.ShouldBeEmpty();
}

[TestClass]
public class SerilogLogging
{
	private class CollectingSink : ILogEventSink
	{
		public List<LogEvent> Events { get; } = [];
		public void Emit(LogEvent logEvent) => Events.Add(logEvent);
	}

	[TestMethod]
	public void neither_plain_nor_destructured_logging_writes_the_value()
	{
		var value = "activation-bytes";
		SecretString secret = value;
		var sink = new CollectingSink();
		using var logger = new LoggerConfiguration().WriteTo.Sink(sink).CreateLogger();

		logger.Information("plain={Plain} destructured={@Destructured}", secret, secret);

		var logEvent = sink.Events.ShouldHaveSingleItem();
		var written = logEvent.RenderMessage()
			+ string.Join("|", logEvent.Properties.Select(p => $"{p.Key}={p.Value}"));

		written.ShouldNotContain(value);
		written.ShouldContain($"[REDACTED length={value.Length}]");
	}
}

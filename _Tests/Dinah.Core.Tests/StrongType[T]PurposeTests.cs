using Dinah.Core.Security;

#nullable enable
namespace StrongTypePurposeTests;

// two strong types over the same primitive: the situation StrongType exists for
class AdpTokenLike : StrongType<string>
{
	public AdpTokenLike(string value) : base(value) { }
}

class RefreshTokenLike : StrongType<string>
{
	public RefreshTokenLike(string value) : base(value) { }

	protected override void ValidateInput(string? value)
	{
		if (value is null || !value.StartsWith("Atnr|"))
			throw new ArgumentException("Improperly formatted refresh token", nameof(value));
	}
}

/// <summary>
/// The first reason for StrongType: two values of the same primitive type can be swapped, and nothing notices.
/// Not the compiler, not a test, not a log - the call simply does the wrong thing with the right-looking data.
/// </summary>
[TestClass]
public class prevents_mixing_up_two_strings
{
	[TestMethod]
	public void primitives_accept_arguments_in_the_wrong_order()
	{
		static string register(string adpToken, string refreshToken) => $"adp={adpToken} refresh={refreshToken}";

		register("adp-value", "Atnr|refresh-value").ShouldBe("adp=adp-value refresh=Atnr|refresh-value");

		// the bug this type exists to prevent: swapped, still compiles, still runs, still silent
		register("Atnr|refresh-value", "adp-value").ShouldBe("adp=Atnr|refresh-value refresh=adp-value");
	}

	[TestMethod]
	public void strong_types_only_accept_the_argument_they_name()
	{
		static string register(AdpTokenLike adpToken, RefreshTokenLike refreshToken)
			=> $"adp={adpToken.Value} refresh={refreshToken.Value}";

		register(new AdpTokenLike("adp-value"), new RefreshTokenLike("Atnr|refresh-value"))
			.ShouldBe("adp=adp-value refresh=Atnr|refresh-value");

		// neither of these can be written at all, which is the whole point:
		//   register(new RefreshTokenLike("Atnr|refresh-value"), new AdpTokenLike("adp-value"));
		//   register("adp-value", "Atnr|refresh-value");
		// StrongType's implicit operator produces a StrongType<string>, never a derived type, so a bare string
		// cannot stand in for either of them
		typeof(AdpTokenLike).IsAssignableFrom(typeof(RefreshTokenLike)).ShouldBeFalse();
		typeof(AdpTokenLike).IsAssignableFrom(typeof(string)).ShouldBeFalse();
	}
}

/// <summary>
/// The second reason: validation runs in the constructor, so an instance that exists is an instance that is
/// well-formed. Every consumer downstream is free to skip re-checking, which is what stops format checks from
/// being copy-pasted along every call path.
/// </summary>
[TestClass]
public class guarantees_a_validated_value
{
	[TestMethod]
	public void an_invalid_value_cannot_be_constructed()
	{
		Assert.Throws<ArgumentException>(() => new RefreshTokenLike("no-prefix"));
		Assert.Throws<ArgumentException>(() => new RefreshTokenLike(null!));
	}

	[TestMethod]
	public void so_a_consumer_does_not_have_to_re_check()
	{
		// no prefix check here: holding the type is the proof
		static string use(RefreshTokenLike token) => token.Value;

		use(new RefreshTokenLike("Atnr|ok")).ShouldBe("Atnr|ok");
	}
}

/// <summary>
/// The third reason, and its sharp edge. A strong type compares equal to its own bare value, which is what lets
/// it stand in for the primitive. In source that looks symmetric, but only because the implicit operator
/// converts the strong type back to a string before an overload is chosen. Once both sides are typed as
/// <see cref="object"/> - dictionary keys, LINQ over object collections - no conversion is available and
/// <see cref="string"/> knows nothing about StrongType, so equality goes one way only.
/// </summary>
[TestClass]
public class compares_equal_to_its_bare_value
{
	[TestMethod]
	public void reads_symmetric_in_source()
	{
		var token = new AdpTokenLike("adp-value");

		(token == "adp-value").ShouldBeTrue();
		("adp-value" == token).ShouldBeTrue();
		token.Equals("adp-value").ShouldBeTrue();

		// true only because the implicit operator turns token back into a string, so the compiler picks
		// string.Equals(string) rather than string.Equals(object)
		"adp-value".Equals(token).ShouldBeTrue();
	}

	[TestMethod]
	public void but_not_once_both_sides_are_object()
	{
		object token = new AdpTokenLike("adp-value");
		object value = "adp-value";

		token.Equals(value).ShouldBeTrue();
		value.Equals(token).ShouldBeFalse();
	}

	[TestMethod]
	public void which_decides_which_way_a_dictionary_lookup_has_to_go()
	{
		var token = new AdpTokenLike("adp-value");
		token.GetHashCode().ShouldBe("adp-value".GetHashCode());

		// works: the stored key is the strong type, and a dictionary asks the stored key
		var keyedByStrongType = new Dictionary<object, string> { [token] = "found" };
		keyedByStrongType.ContainsKey("adp-value").ShouldBeTrue();

		// does not: the stored key is a string, so string.Equals(object) is what gets asked
		var keyedByValue = new Dictionary<object, string> { ["adp-value"] = "found" };
		keyedByValue.ContainsKey(token).ShouldBeFalse();
	}
}

// the composition AudibleApi's token types use: StrongType says what the value is and validates it,
// SecretString says who may see it
class SecretToken : StrongType<SecretString>
{
	public SecretToken(SecretString value) : base(value) { }

	protected override void ValidateInput(SecretString value)
	{
		if (value.Reveal() is not string raw || !raw.StartsWith("Atnr|"))
			throw new ArgumentException("Improperly formatted token", nameof(value));
	}
}

/// <summary>
/// StrongType answers what a value means and whether it is well-formed; <see cref="SecretString"/> answers who
/// may see it. They are orthogonal, so a type that needs both composes them rather than choosing, and the
/// redaction comes with the wrapper instead of being hand-written per type.
/// </summary>
[TestClass]
public class composes_with_SecretString
{
	[TestMethod]
	public void validation_still_runs_on_the_secret()
	{
		Assert.Throws<ArgumentException>(() => new SecretToken("no-prefix"));
		new SecretToken("Atnr|ok").Value.Reveal().ShouldBe("Atnr|ok");
	}

	[TestMethod]
	public void the_inherited_ToString_redacts_without_being_overridden()
	{
		var secret = "Atnr|ok";

		var token = new SecretToken(secret);

		token.ToString().ShouldBe($"[REDACTED length={secret.Length}]");
		token.ToString().ShouldNotContain(secret);
	}
}

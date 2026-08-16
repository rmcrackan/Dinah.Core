using Newtonsoft.Json;

#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// A string that must not reach a log. <see cref="Reveal"/> is the only way to the value: no public property
/// or field holds it, so the reflection-based paths that produce logs find nothing to write. That covers
/// Serilog's structured destructuring and Serilog.Exceptions' walk over the public properties of a logged
/// exception, which is how secrets hanging off an object graph reach a log file nobody meant to publish.
/// <para>
/// Everything else - interpolation, <see cref="string.Format(string, object?)"/>, <see cref="ToString"/> -
/// gets <see cref="Redact(string?)"/> instead, which shares only what costs nothing: whether the value is
/// null, and how long it is. Enough to answer "is this even set?" from a log attached to a public issue.
/// </para>
/// </summary>
[JsonConverter(typeof(SecretStringJsonConverter))]
public readonly struct SecretString : IEquatable<SecretString>
{
	private readonly string? value;

	public SecretString(string? value) => this.value = value;

	/// <summary>The secret itself. Each call site is a place to check that the value cannot reach a log.</summary>
	public string? Reveal() => value;

	/// <summary>Whether a non-empty secret is held. Safe to log.</summary>
	public bool HasValue => !string.IsNullOrEmpty(value);

	/// <summary>Safe to log: shape only, never content.</summary>
	public override string ToString() => Redact(value);

	/// <summary>
	/// The canonical redaction: <c>[REDACTED &lt;null&gt;]</c> or <c>[REDACTED length=N]</c>, where an empty
	/// value is length 0.
	/// </summary>
	public static string Redact(string? value)
		=> value is null
			? "[REDACTED <null>]"
			: $"[REDACTED length={value.Length}]";

	/// <summary>
	/// <see cref="Redact(string?)"/> behind a name, for a <see cref="object.ToString"/> override on a type
	/// that wraps a secret: <c>RefreshToken [REDACTED length=8]</c>.
	/// </summary>
	public static string Redact(string? label, string? value)
		=> string.IsNullOrWhiteSpace(label) ? Redact(value) : $"{label} {Redact(value)}";

	public bool Equals(SecretString other) => string.Equals(value, other.value, StringComparison.Ordinal);

	public override bool Equals(object? obj) => obj is SecretString other && Equals(other);

	public override int GetHashCode() => value?.GetHashCode(StringComparison.Ordinal) ?? 0;

	public static bool operator ==(SecretString a, SecretString b) => a.Equals(b);

	public static bool operator !=(SecretString a, SecretString b) => !a.Equals(b);

	public static implicit operator SecretString(string? value) => new(value);
}

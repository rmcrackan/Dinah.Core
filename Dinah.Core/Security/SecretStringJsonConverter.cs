using Newtonsoft.Json;

#nullable enable
namespace Dinah.Core.Security;

/// <summary>
/// Persists a <see cref="SecretString"/> as the bare string it wraps, so a settings file keeps the format it
/// had before the property became a secret. Applied by <see cref="SecretString"/> itself, so no consumer has
/// to remember it - without it the struct would serialize as <c>{}</c> and the stored secret would be lost.
/// </summary>
public class SecretStringJsonConverter : JsonConverter<SecretString>
{
	public override void WriteJson(JsonWriter writer, SecretString value, JsonSerializer serializer)
		=> writer.WriteValue(value.Reveal());

	public override SecretString ReadJson(
		JsonReader reader,
		Type objectType,
		SecretString existingValue,
		bool hasExistingValue,
		JsonSerializer serializer)
		=> new(reader.TokenType is JsonToken.Null ? null : reader.Value?.ToString());
}

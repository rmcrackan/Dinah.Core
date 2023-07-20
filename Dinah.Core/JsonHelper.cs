using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable
namespace Dinah.Core
{
	public static class JsonHelper
	{
		public static T FromJson<T>(string json, string? jsonPath = null, JsonSerializerSettings? jsonSerializerSettings = null)
		{
			T? instance;

			if (jsonPath is null)
				instance = JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
			else
			{
				var serializer = JsonSerializer.Create(jsonSerializerSettings);
                var token = JObject.Parse(json).SelectToken(jsonPath);
				instance = token is null ? default : token.ToObject<T>(serializer);
			}

			if (instance is null)
				throw new FormatException("Could not deserialize json: " + json);

			return instance;
		}

        public static T? GetValue<T>(this JObject jObject, string propertyName)
        {
			if (jObject?.GetValue(propertyName)?.Value<object>() is not object obj)
				return default;

			if (obj.GetType().IsAssignableTo(typeof(T))) return (T)obj;
			if (obj is JObject jObject2) return jObject2.ToObject<T>();
            if (obj is JValue jValue)
			{
				if (typeof(T).IsAssignableTo(typeof(Enum)))
				{
					return
						Enum.TryParse(typeof(T), jValue.Value<string>(), out var enumVal)
						? (T)enumVal
						: Enum.GetValues(typeof(T)).Cast<T>().First();
				}
				return jValue.Value<T>();
			}
			throw new InvalidCastException($"{obj.GetType()} is not convertible to {typeof(T)}");
		}

        /// <summary>
        /// Find a key containing all of these strings. Recursive search
        /// </summary>
        public static JProperty? Search(string jsonFilePath, IEnumerable<string> searchPieces)
        {
            // SingleOrDefault: don't accidentally update more than intended
            var contents = File.ReadAllText(jsonFilePath);
            var result = JObject.Parse(contents)
                .AllTokens()
                .Where(a => a is JProperty)
                .Cast<JProperty>()
                .SingleOrDefault(prop => searchPieces.All(piece => prop.Name.ContainsInsensitive(piece)));
            return result;
        }

        /// <summary>
        /// Especially convenient when used with linq
        /// </summary>
        public static IEnumerable<JToken> AllTokens(this JObject obj)
        {
            var toSearch = new Stack<JToken>(obj.Children());
            while (toSearch.Count > 0)
            {
                var inspected = toSearch.Pop();
                yield return inspected;

                foreach (var child in inspected)
                    toSearch.Push(child);
            }
        }

        public static void UpdateJsonFile(string jsonFilePath, Action<JObject> updateAction)
        {
            var contents = File.ReadAllText(jsonFilePath);
            var jObj = JObject.Parse(contents);
            updateAction(jObj);
            File.WriteAllText(jsonFilePath, jObj.ToString(Formatting.Indented));
        }
    }
}

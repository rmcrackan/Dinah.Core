using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dinah.Core
{
	public static class JsonHelper
	{
		public static T FromJson<T>(string json, string jsonPath = null, JsonSerializerSettings jsonSerializerSettings = null)
		{
			T instance;

			if (jsonPath is null)
				instance = JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
			else
			{
				var serializer = JsonSerializer.Create(jsonSerializerSettings);
				instance = JObject.Parse(json)
					.SelectToken(jsonPath)
					.ToObject<T>(serializer);
			}

			if (instance is null)
				throw new FormatException("Could not deserialize json: " + json);

			return instance;
		}

        /// <summary>
        /// Find a key containing all of these strings. Recursive search
        /// </summary>
        public static JProperty Search(string jsonFilePath, IEnumerable<string> searchPieces)
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

using System;
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
	}
}

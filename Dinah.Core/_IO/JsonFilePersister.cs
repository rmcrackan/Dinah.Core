﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable
namespace Dinah.Core.IO
{
	/// <summary>
	/// Persist settings to json file.
	/// 
	/// <list type="bullet">
	///     <item><description>If not using the optional JSONPath: Create file if it does not exist. Overwrite existing with identity tokens</description></item>
	///     <item><description>If using the optional JSONPath: the object of the path must be valid and existing in the file.</description></item>
	/// </list>
	/// 
	/// <example>Impetus for being IDisposable: 'using' disposes/unsubscribes persistence. can still use object
	/// <code>
	/// foo()
	///   using var persister = new Persister()
	///     // PERSISTS
	///     persister.Target.Prop = true
	///     return persister.Target
	/// bar()
	///   // DOES NOT PERSIST
	///   foo().Prop = false
	/// </code>
	/// </example>
	/// </summary>
	public abstract class JsonFilePersister<T> : IDisposable
		where T : IUpdatable
	{
		public T Target { get; }
		public string Path { get; }
		public string? JsonPath { get; }

		/// <summary>uses path. create file if doesn't yet exist</summary>
		protected JsonFilePersister(T target, string path, string? jsonPath = null)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Target.Updated += saveFile;

			validatePath(path);

			Path = path;

			if (!string.IsNullOrWhiteSpace(jsonPath))
				JsonPath = jsonPath.Trim();

			saveFile(this, EventArgs.Empty);
		}

		/// <summary>load from existing file</summary>
		protected JsonFilePersister(string path, string? jsonPath = null)
		{
			validatePath(path);

			Path = path;

			if (!string.IsNullOrWhiteSpace(jsonPath))
				JsonPath = jsonPath.Trim();

			Target = loadFromFile();
			Target.Updated += saveFile;
		}

		private T loadFromFile()
		{
			var json = File.ReadAllText(Path);
			var target = JsonHelper.FromJson<T>(json, JsonPath, GetSerializerSettings());

			if (target is null)
				throw new FormatException("File was not in a format able to be imported");

			return target;
		}

		protected virtual JsonSerializerSettings? GetSerializerSettings() => null;

		private void validatePath(string path)
		{
			if (path is null)
				throw new ArgumentNullException(nameof(path));
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentException("Path cannot be blank", nameof(path));
		}


		private bool pendingUpdate = false;

		public bool IsInTransaction { get; private set; }

		public void BeginTransation() => IsInTransaction = true;

		public void CommitTransation()
		{
			IsInTransaction = false;
			if (pendingUpdate)
				saveFile(this, EventArgs.Empty);
			pendingUpdate = false;
		}

		// currently not needed. might add later:
		// - RollbackTransation/AbondonTransation
		// - reload Target

		/// <summary>Called when save begins</summary>
		protected virtual void OnSaving() { }
		/// <summary>Called when save begins</summary>
		protected virtual void OnSaved() { }

		private object _locker { get; } = new object();
		private void saveFile(object? _, EventArgs __)
		{
			if (IsInTransaction)
			{
				pendingUpdate = true;
				return;
			}

			lock (_locker)
            {
                try
                {
					OnSaving();

					if (JsonPath is null)
					{
						File.WriteAllText(Path, JsonConvert.SerializeObject(Target, Formatting.Indented, GetSerializerSettings()));
						return;
					}

					// path must
					// - exist
					// - have valid jsonPath match

					var contents = File.ReadAllText(Path);
					var allToken = JObject.Parse(contents);
					var pathToken = allToken.SelectToken(JsonPath);

					if (pathToken is null)
						throw new JsonSerializationException($"No match found at JSONPath: {JsonPath}");

					// load existing identity into JObject
					var serializer = JsonSerializer.Create(GetSerializerSettings());
					var idJObj = JObject.FromObject(Target, serializer);

					// replace. this propgates to 'allToken'
					pathToken.Replace(idJObj);

					var allSer = JsonConvert.SerializeObject(allToken, Formatting.Indented, GetSerializerSettings());
					File.WriteAllText(Path, allSer);
				}
                finally
                {
					OnSaved();
				}
			}
		}

		private void _dispose()
			=> Target.Updated -= saveFile;

		#region IDisposable pattern
		// from: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
		// To detect redundant calls
		private bool _disposed = false;

		~JsonFilePersister() => Dispose(false);

		// Public implementation of Dispose pattern callable by consumers.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Protected implementation of Dispose pattern.
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				// dispose managed state (managed objects).
				_dispose();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
			// TODO: set large fields to null.

			_disposed = true;
		}
		#endregion
	}
}

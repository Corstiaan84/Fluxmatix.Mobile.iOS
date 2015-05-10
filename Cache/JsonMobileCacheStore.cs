using System;
using Fluxmatix.Mobile.Cache;
using Foundation;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Fluxmatix.Mobile.iOS.Cache
{
	public class JsonMobileCacheStore : IMobileCacheStore
	{
		private CustomCreationConverter<CacheItem<object>> _deserializer;
		public JsonMobileCacheStore ()
		{
		}

		public void SetCustomCacheItemDeserializer (CustomCreationConverter<CacheItem<object>> deserializer)
		{
			_deserializer = deserializer;
		}

		private string GetCacheDirectory() {
			var library = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User) [0];
			var cacheFolder = Path.Combine (library.Path, "FluxmatixMobileiOS");
			if(Directory.Exists(cacheFolder) == false) {
				Directory.CreateDirectory (cacheFolder);
				var filename = GetJsonFilePath ();
				File.WriteAllText (filename, "");  
			}
			return cacheFolder;
		}

		private string GetJsonFilePath() {
			var folder = GetCacheDirectory ();
			var filename = Path.Combine (folder, "cache.json");
			return filename;
		}

		private List<CacheItem<object>> GetCacheItems() {
			var filename = GetJsonFilePath ();
			var json = File.ReadAllText(filename);
			if (_deserializer != null) {
				return JsonConvert.DeserializeObject<List<CacheItem<object>>> (json, _deserializer);
			} else {
				return JsonConvert.DeserializeObject<List<CacheItem<object>>> (json);
			}
		}

		private void WriteCacheItems(List<CacheItem<object>> items) {
			var filename = GetJsonFilePath ();
			var json = JsonConvert.SerializeObject (items);
			File.WriteAllText (filename, json);
		}

		#region IMobileCacheStore implementation

		public string Get (string key)
		{
			//throw new NotImplementedException ();
			return (string)Get<string> (key);
		}

		public void Put (string key, string value)
		{
			Put<string> (key, value);
		}

		public T Get<T> (string key)
		{
			var items = GetCacheItems ();
			if (items == null)
				throw new CacheKeyNotFoundException ();
			var item = items.Find (q => q.Key == key);
			if (item == null)
				throw new CacheKeyNotFoundException ();
			var result = JsonConvert.DeserializeObject<T> (item.Value.ToString ());
			return result;
		}

		public void Put<T> (string key, T value)
		{
			var items = GetCacheItems (); 
			if (items == null)
				items = new List<CacheItem<object>> ();
			if(items.Find(q => q.Key == key) != null) {
				var item = items.Find (q => q.Key == key);
				items.Remove (item);
			}
			items.Add(new CacheItem<object> () { Key = key, Value = value });
			WriteCacheItems (items);
		}

		public bool ContainsKey (string key)
		{
			var result = false;
			var items = GetCacheItems ();
			if(items != null) {
				if(items.Find(q => q.Key == key) != null) {
					result = true;
				}
			}
			return result;
		}

		public void Clear ()
		{
			var items = new List<CacheItem<object>> ();
			WriteCacheItems (items);
		}

		#endregion
	}
}


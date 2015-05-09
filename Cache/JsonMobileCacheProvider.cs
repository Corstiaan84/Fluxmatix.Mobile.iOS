using System;
using Fluxmatix.Mobile.Cache;
using Foundation;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Fluxmatix.Mobile.iOS.Cache
{
	public class JsonMobileCacheProvider : IMobileCacheStore
	{
		public JsonMobileCacheProvider ()
		{
			GetCacheDirectory ();
		}

		private string GetCacheDirectory() {
			var library = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User) [0];
			var cacheFolder = Path.Combine (library.AbsoluteString, "Fluxmatix.Mobile.iOS.Cache");
			if(Directory.Exists(cacheFolder) == false) {
				Directory.CreateDirectory (cacheFolder);
				File.WriteAllText (GetJsonFilePath (), "");
			}
			return cacheFolder;
		}

		private string GetJsonFilePath() {
			var folder = GetCacheDirectory ();
			var filename = Path.Combine (folder, "cache.json");
			return filename;
		}

		private void WriteJsonFile(string content) {

		}

		private List<CacheItem<object>> GetCacheItems() {
			var filename = GetJsonFilePath ();
			var json = File.ReadAllText(filename);
			return JsonConvert.DeserializeObject<List<CacheItem<object>>> (json);
		}

		private void WriteCacheItems(List<CacheItem<object>> items) {
			var filename = GetJsonFilePath ();
			File.WriteAllText (filename, JsonConvert.SerializeObject(items));
		}

		#region IMobileCacheStore implementation

		public string Get (string key)
		{
			throw new NotImplementedException ();
		}

		public void Put (string key, string value)
		{
			throw new NotImplementedException ();
		}

		public object Get<T> (string key)
		{
			var items = GetCacheItems ();
			if (items == null)
				throw new CacheKeyNotFoundException ();
			var item = items.Find (q => q.Key == key);
			if (item == null)
				throw new CacheKeyNotFoundException ();
			return JObject.Parse (item.Value.ToString()).ToObject<T>();
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
			if (items == null)
				result = false;
			if(items.Find(q => q.Key == key) != null) {
				result = true;
			}
			return result;
		}

		public void Clear ()
		{
			var items = GetCacheItems ();
			items.Clear ();
			WriteCacheItems (items);
		}

		#endregion
	}
}


using System;
using Fluxmatix.Mobile.Cache;
using PerpetualEngine.Storage;

namespace Fluxmatix.Mobile.iOS.Cache
{
	public class SimpleStorageMobileCacheStore : IMobileCacheStore
	{
		private SimpleStorage _storage;

		public SimpleStorageMobileCacheStore ()
		{
			_storage = SimpleStorage.EditGroup ("Fluxmatix.Mobile.iOS.Cache");
		}

		#region IMobileCacheStore implementation

		public string Get (string key)
		{
			return _storage.Get (key);
		}

		public void Put (string key, string value)
		{
			_storage.Put (key, value);
		}

		public object Get<T> (string key)
		{
			return _storage.Get<T> (key);
		}

		public void Put<T> (string key, T value)
		{
			_storage.Put<T> (key, value);
		}

		public bool ContainsKey (string key)
		{
			return _storage.HasKey (key);
		}

		#endregion
	}
}


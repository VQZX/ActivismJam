using System;
using System.IO;
using UnityEngine;

namespace Flusk.DataSaver
{
	/// <summary>
	/// Base class that loads data from TextAssets (though is unable to save)
	/// </summary>
	public abstract class TextAssetLoader<T> : IDataSaver<T> where T: IDataStore
	{
		protected readonly TextAsset asset;

		/// <summary>
		/// Instantiate a json saver
		/// </summary>
		/// <param name="asset">The text asset to load from</param>
		protected TextAssetLoader(TextAsset asset)
		{
			this.asset = asset;
		}

		public void Save(T data)
		{
			throw new InvalidOperationException("TextAssetLoader is read only");
		}
		public abstract bool Load(out T data);

		public void Delete()
		{
			throw new InvalidOperationException("TextAssetLoader is read only");
		}

		protected virtual StreamReader GetReadStream()
		{
			return new StreamReader(new MemoryStream(asset.bytes));
		}
	}
}

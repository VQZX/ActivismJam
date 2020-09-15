using System.IO;
using LitJson;
using UnityEngine;

namespace Flusk.DataSaver
{
	public class LitJsonTextAssetLoader<T> : TextAssetLoader<T>  where T: IDataStore
	{
		public LitJsonTextAssetLoader(TextAsset asset) : base(asset) {}

		/// <summary>
		/// Load the specified data store
		/// </summary>
		/// <param name="data">Data.</param>
		public override bool Load(out T data)
		{
			if (asset == null)
			{
				data = default(T);
				return false;
			}

			using (StreamReader reader = GetReadStream())
			{
				data = JsonMapper.ToObject<T>(reader);
			}

			return true;
		}
	}
}

using System.IO;
using UnityEngine;

namespace Flusk.DataSaver
{
	public class JsonSaver<T> : FileSaver<T> where T: IDataStore
	{
		public JsonSaver(string filename) : base(filename) {}

		/// <summary>
		/// Save the specified data store
		/// </summary>
		/// <param name="data">Data.</param>
		public override void Save(T data)
		{
			string json = JsonUtility.ToJson(data);

			using (StreamWriter writer = GetWriteStream())
			{
				writer.Write(json);
			}
		}

		/// <summary>
		/// Load the specified data store
		/// </summary>
		/// <param name="data">Data.</param>
		public override bool Load(out T data)
		{
			data = default(T);
			if (!File.Exists(filename))
			{
				return false;
			}

			using (StreamReader reader = GetReadStream())
			{
				JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), data);
			}

			return true;
		}
	}
}

using System;
using System.IO;
using LitJson;

namespace Flusk.DataSaver
{
	public class LitJsonSaver<T> : FileSaver<T> where T: IDataStore
	{
		public LitJsonSaver(string filename) : base(filename) {}

		/// <summary>
		/// Save the specified data store
		/// </summary>
		/// <param name="data">Data.</param>
		public override void Save(T data)
		{
			using (StreamWriter writer = GetWriteStream())
			{
				var jsonWriter = new JsonWriter(writer);
#if UNITY_EDITOR
				jsonWriter.PrettyPrint = true;
#endif

				JsonMapper.ToJson(data, jsonWriter);
			}
		}

		/// <summary>
		/// Load the specified data store
		/// </summary>
		/// <param name="data">Data.</param>
		public override bool Load(out T data)
		{
			if (!File.Exists(filename))
			{
				data = default(T);
				return false;
			}

			using (var reader = GetReadStream())
			{
				JsonMapper.RegisterExporter<double>((obj, writer) => writer.Write(Convert.ToDouble(obj)));
				JsonMapper.RegisterImporter<double, double>(Convert.ToDouble);
				data = JsonMapper.ToObject<T>(reader);
			}

			return true;
		}
	}
}

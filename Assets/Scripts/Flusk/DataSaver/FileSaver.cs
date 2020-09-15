using System.IO;
using Flusk.Management.Platform;
using UnityEngine;

namespace Flusk.DataSaver
{
	public abstract class FileSaver<T> : IDataSaver<T> where T: IDataStore
	{
		protected readonly string filename;

		/// <summary>
		/// Instantiate a json saver
		/// </summary>
		/// <param name="filename">The filename (including extension) to use.</param>
		/// <remarks><paramref name="filename"/> is relative to <see cref="Application.persistentDataPath"/></remarks>
		protected FileSaver(string filename)
		{
			this.filename = FileSaver<T>.GetFinalSaveFilename(filename);
		}

		public abstract void Save(T data);
		public abstract bool Load(out T data);

		public void Delete()
		{
			File.Delete(filename);
		}

		public static string GetFinalSaveFilename(string baseFilename)
		{
			return string.Format("{0}/{1}", PlatformHelper.PersistentDataPath(), baseFilename);
		}

		protected virtual StreamWriter GetWriteStream()
		{
			return new StreamWriter(new FileStream(filename, FileMode.Create));
		}

		protected virtual StreamReader GetReadStream()
		{
			return new StreamReader(new FileStream(filename, FileMode.Open));
		}
	}
}

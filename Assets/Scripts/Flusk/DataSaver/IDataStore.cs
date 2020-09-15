namespace Flusk.DataSaver
{
	public interface IDataStore
	{
		void PreSave();
		void PostLoad();
	}
}

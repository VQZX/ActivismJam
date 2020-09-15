namespace Flusk.Systems
{
	/// <summary>
	/// Interface for persistent manager that does some initialization work that should occur after
	/// all <see cref="IPreInitializationWorker"/> components have done their work
	/// </summary>
	public interface IPostInitializationWorker
	{
		void PostInitialize();
	}
}

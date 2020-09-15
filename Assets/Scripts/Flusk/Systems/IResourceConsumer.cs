namespace Flusk.Systems
{
	/// <summary>
	/// Interface for a persistent manager that requires some dynamically-loaded resources
	/// </summary>
	public interface IResourceConsumer
	{
		/// <summary>
		/// Called when this consumer should load its persistent resources
		/// </summary>
		void LoadDependencies();
	}
}

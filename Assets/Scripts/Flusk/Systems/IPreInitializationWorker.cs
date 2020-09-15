using System;

namespace Flusk.Systems
{
	/// <summary>
	/// Interface for persistent manager that does some initialization work that should defer the rest of
	/// the initialization process
	/// </summary>
	public interface IPreInitializationWorker
	{
		bool Initialized { get; }
		event Action<IPreInitializationWorker> InitializationComplete;
	}
}

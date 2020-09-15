#if !UNITY_EDITOR && UNITY_ANDROID
#if SPLIT_APK
using TFBGames.Plugins.DynamicPlugins;
#else
using TFBGames.Management.App;
#endif
#endif
using Flusk.Utility;
using UnityEngine;

namespace Flusk.Systems
{
	public class Initializer : PrefabFactory
	{
		private const string MainPrefabPath = "Main";

		private int deferredInitializers;
		private int initializations;

		public static bool Initialized { get; protected set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), JetBrains.Annotations.UsedImplicitly]
		private static void Main()
		{
#if USE_INITIALIZER
#if !UNITY_EDITOR && UNITY_ANDROID
#if SPLIT_APK
			// Wait for SplitAPK plugin to tell us we're ready
			SplitAPK.OBBCompleteCB += PerformInitialization;
#else
			// Wait instead for permissions to be granted
			AndroidStoragePermissionChecker.PermissionGrantedCB += PerformInitialization;
#endif
#else
			PerformInitialization(); // Just do it immediately
#endif
#endif
		}


		/// <summary>
		/// Do initialization. This potentially waits on OBBs being available
		/// </summary>
		private static void PerformInitialization()
		{
			// Unregister delegates
#if UNITY_ANDROID && !UNITY_EDITOR
#if SPLIT_APK
			SplitAPK.OBBCompleteCB -= PerformInitialization;
#else
			AndroidStoragePermissionChecker.PermissionGrantedCB -= PerformInitialization;
#endif
#endif

			Debug.Log("[INIT] Initializer creating Main prefab");
			Initializer mainPrefab = Resources.Load<Initializer>(MainPrefabPath);

			if (mainPrefab != null)
			{
				Initializer instantiatedObject = Object.Instantiate(mainPrefab);
				instantiatedObject.Init();
			}
			else
			{
				Debug.Log("[INIT] Failed to load Main prefab.");
			}
		}

		/// <summary>
		/// Intentionally not calling base
		/// </summary>
		protected override void Awake() {}


		/// <summary>
		/// Perform immediate initialization
		/// </summary>
		protected virtual void Init()
		{
			CreatePrefabs();

			foreach (GameObject createdManager in createdObjects.Values)
			{
				foreach (MonoBehaviour managementBehaviour in createdManager.GetComponentsInChildren<MonoBehaviour>(true))
				{
					var initializer = managementBehaviour as IPreInitializationWorker;
					if ((initializer != null) &&
						!initializer.Initialized)
					{
						Debug.LogFormat("[INIT] Deferring loading on object {0}", initializer);
						deferredInitializers++;
						initializer.InitializationComplete += OnInitializationComplete;
					}
				}
			}

			if (deferredInitializers == 0)
			{
				DeferredInit();
			}
		}

		/// <summary>
		/// Called by IInitializationWorkers
		/// </summary>
		private void OnInitializationComplete(IPreInitializationWorker completedWorker)
		{
			Debug.LogFormat("[INIT] Initializer {0} now ready", completedWorker);

			completedWorker.InitializationComplete -= OnInitializationComplete;

			initializations++;
			if (initializations == deferredInitializers)
			{
				DeferredInit();
			}
			Debug.Assert(initializations <= deferredInitializers);
		}


		/// <summary>
		/// Perform some deferred initialization that requires manager components to initialize some asynchronous tasks
		/// or tasks that otherwise cannot be performed immediately on instantiation
		/// </summary>
		protected virtual void DeferredInit()
		{
			// Tell loaded managers can do post-initialization
			foreach (GameObject createdManager in createdObjects.Values)
			{
				foreach (MonoBehaviour managementBehaviour in createdManager.GetComponentsInChildren<MonoBehaviour>(true))
				{
					var initializer = managementBehaviour as IPostInitializationWorker;
					if (initializer != null)
					{
						initializer.PostInitialize();
					}
				}
			}

			// Tell loaded managers that they can initialize any dynamic data
			foreach (GameObject createdManager in createdObjects.Values)
			{
				foreach (MonoBehaviour managementBehaviour in createdManager.GetComponentsInChildren<MonoBehaviour>(true))
				{
					var consumer = managementBehaviour as IResourceConsumer;
					if (consumer != null)
					{
						consumer.LoadDependencies();
					}
				}
			}

			Initialized = true;
		}
	}
}

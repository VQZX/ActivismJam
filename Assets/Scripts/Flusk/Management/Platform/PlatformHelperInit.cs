using System;
using Flusk.Systems;
using UnityEngine;

namespace Flusk.Management.Platform
{
	public class PlatformHelperInit : MonoBehaviour, IPreInitializationWorker
	{
		[SerializeField]
		protected string defaultLocale = "en_US";

		[SerializeField]
		protected string defaultCountry = "US";

		[SerializeField]
		protected string androidBridgeClassName = "com.tfbgames.platformhelper.PlatformHelperBridge";

		public bool Initialized { get; private set; }
		public event Action<IPreInitializationWorker> InitializationComplete;

		[JetBrains.Annotations.UsedImplicitly]
		private void Awake()
		{
			// ReSharper disable once JoinDeclarationAndInitializer
			// Can't join here because of subsequent #ifs
			PlatformHelper ph = null;

			#if UNITY_EDITOR
			ph = gameObject.AddComponent<PlatformHelperEditor>();
			#elif UNITY_IOS
				ph = gameObject.AddComponent<PlatformHelperIOS>();
			#elif UNITY_ANDROID
				ph = gameObject.AddComponent<PlatformHelperAndroid>();
			#endif

			if(ph == null)
			{
				Debug.LogError("Unsupported platform for PlatformHelper");
				return;
			}

			ph.InitializationComplete += OnInitializationComplete;
			Initialized = ph.Initialized;

			//set up app-defined constants
			ph.DefaultLocale = defaultLocale;
			ph.DefaultCountry = defaultCountry;
			ph.AndroidBridgeClassName = androidBridgeClassName;

			// We've done everything we needed to do
			if (Initialized)
			{
				Destroy(this);
				OnInitializationComplete(ph);
			}
		}

		private void OnInitializationComplete(IPreInitializationWorker completedWorker)
		{
			Initialized = true;
			Destroy(this);

			if (InitializationComplete != null)
			{
				InitializationComplete(completedWorker);
			}
		}
	}
}

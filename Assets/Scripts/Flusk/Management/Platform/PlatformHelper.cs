using System;
using Flusk.Patterns;
using Flusk.Systems;
using UnityEditor;
using UnityEngine;

namespace Flusk.Management.Platform
{
	public abstract class PlatformHelper : PersistentSingleton<PlatformHelper>, IPreInitializationWorker
	{
		//supported permission types to check / request
		public enum PermissionType
		{
			ReadExternalData,
			WriteExternalData,
			RecordAudio,
			AccessCameraRoll,
			NumPermissions
		}

		//results of a permission request
		public enum PermissionRequestResult
		{
			Granted,
			Denied,
			DeniedNeverAsk
		}

		public event Action<IPreInitializationWorker> InitializationComplete;

		public static event Action<bool> HeadphonesConnectedEvent;

		//app-defined constant values
		public string DefaultLocale { get; set; }
		public string DefaultCountry { get; set; }
		public string AndroidBridgeClassName { get; set; }

		public float OsVersion { get; private set; }
		public float OsVersionMajor { get; private set; }

		//public static method accessors
		public static int DiskSpaceFreeMegabytes { get { return Instance._GetDiskSpaceFreeMegabytes(); } }
		public static string Locale { get { return Instance._GetLocale(); } }
		public static string CountryCode { get { return Instance._GetCountryCode(); } }
		public static string BuildNumber { get { return Instance._GetBuildNumber(); } }
		public static string VersionString { get { return Instance._GetVersionString(); } }

		public static bool IsMusicPlaying { get { return Instance._GetIsMusicPlaying(); } }
		public static bool AreHeadphonesConnected { get { return Instance._GetAreHeadphonesConnected(); } }
		public static bool IsMicrophoneAvailable { get { return Instance._IsMicrophoneAvailable(); } }
		public static bool GuidedMode { get { return Instance._IsInGuidedAccessMode(); } }
		public static float DeviceVolume { get { return Instance._GetDeviceVolume(); } }

		public bool Initialized { get; private set; }

		/// <summary>
		/// Initialize this platform helper. Call base last
		/// </summary>
		protected virtual void Initialize()
		{
			if (Initialized)
			{
				return;
			}

			Initialized = true;

			if (InitializationComplete != null)
			{
				InitializationComplete(this);
			}
			
			// Process our version number
			if (!string.IsNullOrEmpty(VersionString))
			{
				string[] split = VersionString.Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries);
				if (split.Length >= 2)
				{
					OsVersionMajor = float.Parse(split[0]);
					OsVersion = OsVersionMajor;
					if (split.Length > 1)
					{
						OsVersion += float.Parse(split[1]) * 0.1f;
					}
				}
			}
			
			Debug.LogFormat("[PH] OS version string {0}, Maj: {1:00.00}, Full {2:00.00}", VersionString, OsVersionMajor, OsVersion);
		}


		protected override void Awake()
		{
			base.Awake();

			this.name = "PlatformHelper";
		}


		//called from native code via SendMessage to flag that the headphone connectivity has changed
		public void OnHeadphonesConnected(string connected)
		{
			Debug.Log("OnHeadphonesConnected called with: " + connected);
			bool isConnected = connected.Equals("true");

			//call any registered events that want to be notified about this
			if(HeadphonesConnectedEvent != null)
			{
				HeadphonesConnectedEvent(isConnected);
			}
		}


		//public static access functions
		public static void LaunchApp(string appUrlSchema)
		{
			Instance._LaunchApp(appUrlSchema);
		}

		public static void LaunchAppConfirm(string title, string msg, string cancel, string confirm, string url)
		{
			Instance._LaunchAppConfirm(title, msg, cancel, confirm, url);
		}

		public static void OpenWebView(string url)
		{
			Instance._OpenWebView(url);
		}

		public static void OpenWebViewConfirm(string title, string msg, string cancel, string confirm, string url)
		{
			Instance._OpenWebViewConfirm(title, msg, cancel, confirm, url);
		}

		public static void OpenStoreView(string productId)
		{
			Instance._OpenStoreView(productId);
		}

		public static void OpenStoreViewConfirm(string title, string msg, string cancel, string confirm, string productId)
		{
			Instance._OpenStoreViewConfirm(title, msg, cancel, confirm, productId);
		}

		public static bool CanLaunchApp(string appUrlSchema)
		{
			return Instance._CanLaunchApp(appUrlSchema);
		}

		public static void ShowSystemAlert(string title, string desc, string oktext, GameObject obj, string function)
		{
			Instance._ShowSystemAlert(title, desc, oktext, obj, function);
		}

		public static void SetClipboardString(string textToCopy)
		{
			Instance._SetClipboardString(textToCopy);
		}

		public static string GetClipboardString()
		{
			return Instance._GetClipboardString();
		}

		public static bool CheckPermission(PermissionType type)
		{
			return Instance._CheckPermission(type);
		}

		public static void RequestPermission(PermissionType type, GameObject obj, string function)
		{
			Instance._RequestPermission(type, obj, function);
		}

		public static void OpenAppSettings()
		{
			Instance._OpenAppSettings();
		}

		//NOTE: preferExternal is only currently useful on Android devices
		public static string PersistentDataPath(bool preferExternal = false)
		{
			#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				return Application.persistentDataPath;
			}
			#endif
			return Instance._PersistentDataPath(preferExternal);
		}


		//abstract virtual methods to override by child classes
		protected virtual int _GetDiskSpaceFreeMegabytes() { return -1; }
		protected virtual string _GetLocale() { return DefaultLocale; }
		protected virtual string _GetCountryCode() { return DefaultCountry; }
		protected virtual string _GetBuildNumber() { return "0"; }
		protected virtual bool _GetIsMusicPlaying() { return false; }
		protected virtual bool _GetAreHeadphonesConnected() { return false; }
		protected virtual bool _IsMicrophoneAvailable()
		{
		//forced to shield this as Unity will automatically add microphone/recording permissions and hardware to AndroidManifest
		//if it detects any occurances of the 'Microphone' class in your code. PlatformHelperAndroid overrides this function anyways...
		#if UNITY_ANDROID
			return false;
		#else
			return (Microphone.devices.Length > 0) ? true : false;
		#endif
		}
		protected virtual float _GetDeviceVolume() { return 1.0f; }
		protected virtual void _SetClipboardString(string textToCopy) { }
		protected virtual string _GetClipboardString() { return null; }
		protected virtual void _ShowSystemAlert(string title, string desc, string oktext, GameObject obj, string function) { }
		protected virtual bool _CanLaunchApp(string appUrlSchema) { return false; }
		protected virtual bool _IsInGuidedAccessMode() { return false; }
		protected virtual void _OpenStoreView(string productId) { }
		protected virtual void _OpenStoreViewConfirm(string title, string msg, string cancel, string confirm, string productId) { }
		protected virtual bool _CheckPermission(PermissionType type) { return true; }
		protected virtual void _RequestPermission(PermissionType type, GameObject obj, string function) { obj.SendMessage(function, PermissionRequestResult.Granted.ToString());  }
		protected virtual void _OpenAppSettings() { }
		protected virtual string _GetVersionString() { return ""; }
		protected virtual string _PersistentDataPath(bool preferExternal) { return Application.persistentDataPath; }


		protected virtual void _LaunchApp(string url)
		{
			Application.OpenURL(url);
		}

		protected virtual void _LaunchAppConfirm(string title, string msg, string cancel, string confirm, string url)
		{
			Application.OpenURL(url);
		}

		protected virtual void _OpenWebView(string url)
		{
			Application.OpenURL(url);
		}

		protected virtual void _OpenWebViewConfirm(string title, string msg, string cancel, string confirm, string url)
		{
			Application.OpenURL(url);
		}
	}
}

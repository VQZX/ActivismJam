#if UNITY_ANDROID
using UnityEngine;
using System;
using TFBGames.Build;
#endif
#if UNITY_ANDROID && PRIME31
using Prime31;
#endif

namespace Flusk.Management.Platform
{
	public class PlatformHelperAndroid : PlatformHelper
	{
#if UNITY_ANDROID
		private static readonly string[] PermissionLUT = {	"android.permission.READ_EXTERNAL_STORAGE",
															"android.permission.WRITE_EXTERNAL_STORAGE",
															"android.permission.RECORD_AUDIO",
															"android.permission.WRITE_EXTERNAL_STORAGE"};

		private const string AndroidActivityName = "com.unity3d.player.UnityPlayer";


		private AndroidJavaObject bridge = null;
		public AndroidJavaObject AndroidActivity { get; private set; }

		private string curUrl = null;
		private string curConfirm = null;
		private GameObject curDismissGameObject = null;
		private string curDismissMethod = null;
		private string cachedVersionString = "";
		private int defaultDialogTheme;

		public void CopyFile(string source, string destination)
		{
			if (bridge != null)
			{
				string funcName = "copyAsset";
#if SPLIT_APK
				//handle separate case for OBB source copies
				funcName = "copyAssetFromOBB";
#endif
				string error = bridge.CallStatic<string>(funcName, source, destination);
				if (!String.IsNullOrEmpty(error))
				{
					Debug.LogErrorFormat("Android CopyFile error: {0}", error);
				}
			}
		}


		protected override void Initialize()
		{
			if (Initialized)
			{
				return;
			}

			bridge = new AndroidJavaClass(AndroidBridgeClassName);
			if(bridge != null)
			{
				AndroidJavaClass activityClass = new AndroidJavaClass(AndroidActivityName);
				if(activityClass != null)
				{
					AndroidActivity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
					if(AndroidActivity != null)
					{
						bridge.CallStatic("setContext", AndroidActivity);
					}
					else
					{
						Debug.LogError("---NULL OBJECT 'currentActivity'");
					}
					activityClass.Dispose();
				}
				else
				{
					Debug.LogError("---CANNOT LOCATE ANDROID ACTIVITY CLASS: " + AndroidActivityName);
				}
			}
			else
			{
				Debug.LogError("---CANNOT LOCATE ANDROID BRIDGE CLASS: " + AndroidBridgeClassName);
			}
			
			// Try get version
			using(var buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
			{
				cachedVersionString = buildVersion.GetStatic<string>("RELEASE");
			}
			
			// Try get default theme for dialogs
			using (var rattrClass = new AndroidJavaClass("android.R$attr"))
			{
				defaultDialogTheme = rattrClass.GetStatic<int>("alertDialogTheme");
			}

			base.Initialize();
		}


		// This is intentionally happening on Start rather than Awake.
		// Cannot get an AndroidJavaClass during Awake
		protected virtual void Start()
		{
			Initialize();
		}


		protected override void OnDestroy()
		{
			if(AndroidActivity != null)
			{
				AndroidActivity.Dispose();
			}
			if(bridge != null)
			{
				bridge.Dispose();
			}
		}


		protected override string _GetLocale()
		{
			if(bridge != null)
			{
				 return bridge.CallStatic<string>("getLocale");
			}
			return base._GetLocale();
		}

		protected override string _GetCountryCode()
		{
			if(bridge != null)
			{
				 return bridge.CallStatic<string>("getCountryCode");
			}
			return base._GetCountryCode();
		}

		protected override string _GetBuildNumber()
		{
			if(bridge != null)
			{
				return bridge.CallStatic<int>("getVersionCode").ToString();
			}
			return base._GetCountryCode();
		}

		protected override int _GetDiskSpaceFreeMegabytes()
		{
			if(bridge != null)
			{
				return bridge.CallStatic<int>("getDiskSpaceFreeMegabytes");
			}
			return base._GetDiskSpaceFreeMegabytes();
		}

		protected override bool _GetIsMusicPlaying()
		{
			Debug.LogError("---_GetIsMusicPlaying IS NOT FUNCTIONAL CURRENTLY ON ANDROID DUE TO UNITY ISSUE FROM v5");

			if(bridge != null)
			{
				return bridge.CallStatic<bool>("getIsMusicPlaying");
			}
			return base._GetIsMusicPlaying();
		}

		protected override bool _GetAreHeadphonesConnected()
		{
			if(bridge != null)
			{
				return bridge.CallStatic<bool>("getAreHeadphonesConnected");
			}
			return base._GetAreHeadphonesConnected();
		}

		protected override bool _IsMicrophoneAvailable()
		{
			if(bridge != null)
			{
				return bridge.CallStatic<bool>("getIsMicrophoneAvailable");
			}
			return base._IsMicrophoneAvailable();
		}

		protected override float _GetDeviceVolume()
		{
			if(bridge != null)
			{
				return bridge.CallStatic<float>("getVolume");
			}
			return base._GetDeviceVolume();
		}

		protected override void _SetClipboardString(string text)
		{
			if(bridge != null)
			{
				bridge.CallStatic("setClipboard", text);
				return;
			}
			base._SetClipboardString(text);
		}

		protected override string _GetClipboardString()
		{
			if(bridge != null)
			{
				return bridge.CallStatic<string>("getClipboard");
			}
			return base._GetClipboardString();
		}

		protected override bool _CanLaunchApp(string packageAddress)
		{
			if(bridge != null)
			{
				return bridge.CallStatic<bool>("canLaunchApp", packageAddress);
			}
			return base._CanLaunchApp(packageAddress);
		}

		protected override void _LaunchApp(string packageAddress)
		{
			if(bridge != null)
			{
				bridge.CallStatic("launchApp", packageAddress);
				return;
			}
			base._LaunchApp(packageAddress);
		}

		protected override void _OpenStoreView(string packageAddress)
		{
			if(bridge != null)
			{
				if(BuildConfig.Instance.Platform == TargetPlatform.GooglePlay)
				{
					bridge.CallStatic("openGooglePlayView", packageAddress);
				}
				else
				{
					bridge.CallStatic("openAmazonStoreView", packageAddress);
				}
				return;
			}
			base._OpenStoreView(packageAddress);
		}


		protected override bool _CheckPermission(PermissionType type)
		{
			if(bridge != null)
			{
				string permission = PermissionLUT[(int)type];
				return bridge.CallStatic<bool>("checkPermission", permission);
			}
			return base._CheckPermission(type);
		}


		protected override void _RequestPermission(PermissionType type, GameObject obj, string function)
		{
			if(bridge != null)
			{
				string permission = PermissionLUT[(int)type];
				bridge.CallStatic<bool>("requestPermission", permission, obj.name, function);
				return;
			}
			base._RequestPermission(type, obj, function);
		}


		protected override void _OpenAppSettings()
		{
			if(bridge != null)
			{
				bridge.CallStatic("openAppSettings");
				return;
			}
			base._OpenAppSettings();
		}


		protected override string _PersistentDataPath(bool preferExternal)
		{
			if(bridge != null)
			{
				return bridge.CallStatic<string>("persistentDataPath", preferExternal);
			}
			return base._PersistentDataPath(preferExternal);
		}


		protected override string _GetVersionString()
		{
			return cachedVersionString;
		}


		//LukeTODO: Custom version of these instead of P31...
		protected override void _ShowSystemAlert(string title, string desc, string oktext, GameObject obj, string function)
		{
			curDismissGameObject = obj;
			curDismissMethod = function;
			curConfirm = oktext;
#if PRIME31
			EtceteraAndroid.setAlertDialogTheme(defaultDialogTheme);
			EtceteraAndroidManager.alertButtonClickedEvent += OnAlertDismiss;
			EtceteraAndroidManager.alertCancelledEvent += OnAlertCancel;
			EtceteraAndroid.showAlert(title, desc, oktext);
#else
			throw new NotImplementedException();
#endif
		}

		protected override void _OpenWebView(string url)
		{
#if PRIME31
			EtceteraAndroid.showCustomWebView(url, true, false);
#else
			throw new NotImplementedException();
#endif
		}

		protected override void _OpenStoreViewConfirm(string title, string msg, string cancel, string confirm, string packageAddress)
		{
			curUrl = packageAddress;
			curConfirm = confirm;

#if PRIME31
			EtceteraAndroidManager.alertButtonClickedEvent += OnStoreViewSelect;
			EtceteraAndroidManager.alertCancelledEvent += OnWebViewCancel;
			EtceteraAndroid.showAlert(title, msg, confirm, cancel);
#else
			throw new NotImplementedException();
#endif
		}

		protected override void _LaunchAppConfirm(string title, string msg, string cancel, string confirm, string packageAddress)
		{
			curUrl = packageAddress;
			curConfirm = confirm;
#if PRIME31
			EtceteraAndroidManager.alertButtonClickedEvent += OnLaunchAppSelect;
			EtceteraAndroidManager.alertCancelledEvent += OnLaunchAppCancel;
			EtceteraAndroid.showAlert(title, msg, confirm, cancel);
#else
			throw new NotImplementedException();
#endif
		}

		protected override void _OpenWebViewConfirm(string title, string msg, string cancel, string confirm, string url)
		{
			curUrl = url;
			curConfirm = confirm;
#if PRIME31
			EtceteraAndroidManager.alertButtonClickedEvent += OnWebViewSelect;
			EtceteraAndroidManager.alertCancelledEvent += OnWebViewCancel;
			EtceteraAndroid.showAlert(title, msg, confirm, cancel);
#else
			throw new NotImplementedException();
#endif
		}



		private void OnAlertCancel()
		{
			OnAlertDismiss(null);
		}
		private void OnAlertDismiss(string button)
		{
#if PRIME31
			EtceteraAndroidManager.alertButtonClickedEvent -= OnAlertDismiss;
			EtceteraAndroidManager.alertCancelledEvent -= OnAlertCancel;
#endif
			if(curDismissGameObject)
			{
				curDismissGameObject.SendMessage(curDismissMethod, (button == curConfirm) ? "ok" : "cancel");
			}
			curDismissGameObject = null;
			curDismissMethod = null;
		}

		private void OnWebViewCancel()
		{
			OnWebViewSelect(null);
		}
		private void OnWebViewSelect(string button)
		{
#if PRIME31
			EtceteraAndroidManager.alertButtonClickedEvent -= OnWebViewSelect;
			EtceteraAndroidManager.alertCancelledEvent -= OnWebViewCancel;
#endif
			if(button == curConfirm)
			{
				_OpenWebView(curUrl);
			}
			curUrl = null;
		}

		private void OnLaunchAppCancel()
		{
			OnLaunchAppSelect(null);
		}
		private void OnLaunchAppSelect(string button)
		{
#if PRIME31
			EtceteraAndroidManager.alertButtonClickedEvent -= OnLaunchAppSelect;
			EtceteraAndroidManager.alertCancelledEvent -= OnLaunchAppCancel;
#endif
			if(button == curConfirm)
			{
				_LaunchApp(curUrl);
			}
			curUrl = null;
		}

		private void OnStoreViewCancel()
		{
			OnStoreViewSelect(null);
		}
		private void OnStoreViewSelect(string button)
		{
#if PRIME31
			EtceteraAndroidManager.alertButtonClickedEvent -= OnStoreViewSelect;
			EtceteraAndroidManager.alertCancelledEvent -= OnStoreViewCancel;
#endif
			if(button == curConfirm)
			{
				_OpenStoreView(curUrl);
			}
			curUrl = null;
		}
#endif
	}
}




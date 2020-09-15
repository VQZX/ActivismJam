#if UNITY_IPHONE
using UnityEngine;
using UnityEngine.iOS;
using System.Runtime.InteropServices;
#endif

namespace Flusk.Management.Platform
{
	public class PlatformHelperIOS : PlatformHelper
	{
		//defines the various OpenURL methods supported on iOS
		private enum OpenURLMethod
		{
			Default,
			WebView,
			StoreView
		}


#if UNITY_IPHONE

		//native iOS interop functions to bridge to
		[DllImport("__Internal")]
		private static extern void _initPlatformHelper();

		[DllImport("__Internal")]
		private static extern string _getLocale();

		[DllImport("__Internal")]
		private static extern string _getCountryCode();

		[DllImport("__Internal")]
		private static extern string _getBuildNumber();
		
		[DllImport("__Internal")]
		private static extern string _getOSVersion();

		[DllImport("__Internal")]
		private static extern int _getFreeDiskSpace();

		[DllImport("__Internal")]
		private static extern bool _getIsMusicPlaying();

		[DllImport("__Internal")]
		private static extern void _forceExternalSpeaker();

		[DllImport("__Internal")]
		private static extern bool _getAreHeadphonesConnected();

		[DllImport("__Internal")]
		private static extern bool _showSystemAlert(string title, string desc, string oktext);

		[DllImport("__Internal")]
		private static extern bool _isAppInstalled(string appname);
		
		[DllImport("__Internal")]
		private static extern bool _isInGuidedAccessMode();

		[DllImport("__Internal")]
		private static extern float _getDeviceVolume();

		[DllImport("__Internal")]
		private static extern void _setClipboardString(string textToCopy);

		[DllImport("__Internal")]
		private static extern string _getClipboardString();

		[DllImport("__Internal")]
		private static extern bool _checkPermission(int permission);

		[DllImport("__Internal")]
		private static extern void _requestPermission(int permission, string objname, string function);

		[DllImport("__Internal")]
		private static extern void _openAppSettings();

		[DllImport("__Internal")]
		private static extern void _openURLWithMethod(string url, int method);

		[DllImport("__Internal")]
		private static extern void _openURLWithMethodConfirm(string title, string msg, string cancel, string confirm, string url, int method);

		protected override void Initialize()
		{
			if (Initialized)
			{
				return;
			}

			_initPlatformHelper();

			base.Initialize();
		}

		public static void ForceExternalSpeaker()
		{
			_forceExternalSpeaker();
		}


		//overwritten methods for iOS platform functionality
		protected override void Awake()
		{
			base.Awake();

			Initialize();
		}

		protected override string _GetLocale()
		{
			return !Application.isEditor ? _getLocale() : base._GetLocale();
		}

		protected override string _GetCountryCode()
		{
			return !Application.isEditor ? _getCountryCode() : base._GetCountryCode();
		}

		protected override string _GetBuildNumber()
		{
			return !Application.isEditor ? _getBuildNumber() : base._GetBuildNumber();
		}

		protected override int _GetDiskSpaceFreeMegabytes()
		{
			return _getFreeDiskSpace();
		}

		protected override bool _GetIsMusicPlaying()
		{
			return _getIsMusicPlaying();
		}

		protected override bool _GetAreHeadphonesConnected()
		{
			return _getAreHeadphonesConnected();
		}

		protected override float _GetDeviceVolume()
		{
			return _getDeviceVolume();
		}

		protected override void _ShowSystemAlert(string title, string desc, string oktext, GameObject obj, string function)
		{
//LukeTODO: pass through obj / function to trigger native SendUnityMessage()
			_showSystemAlert(title, desc, oktext);
		}

		protected override bool _CanLaunchApp(string appURLSchema)
		{
			return _isAppInstalled(appURLSchema);
		}

		protected override bool _IsInGuidedAccessMode()
		{
			return _isInGuidedAccessMode();
		}

		protected override void _LaunchAppConfirm(string title, string msg, string cancel, string confirm, string url)
		{
			_openURLWithMethodConfirm(title, msg, cancel, confirm, url, (int)OpenURLMethod.Default);
		}

		protected override void _OpenWebView(string url)
		{
			int webViewMethod = (int)OpenURLMethod.WebView;

			_openURLWithMethod(url, webViewMethod);
		}

		protected override void _OpenWebViewConfirm(string title, string msg, string cancel, string confirm, string url)
		{
			int webViewMethod = (int)OpenURLMethod.WebView;

			_openURLWithMethodConfirm(title, msg, cancel, confirm, url, webViewMethod);
		}

		protected override void _OpenStoreView(string productID)
		{
			_openURLWithMethod(productID, (int)OpenURLMethod.StoreView);
		}

		protected override void _OpenStoreViewConfirm(string title, string msg, string cancel, string confirm, string productID)
		{
			_openURLWithMethodConfirm(title, msg, cancel, confirm, productID, (int)OpenURLMethod.StoreView);
		}

		protected override void _SetClipboardString(string textToCopy)
		{
			_setClipboardString(textToCopy);
		}

		protected override string _GetClipboardString()
		{
			return _getClipboardString();
		}

		protected override bool _CheckPermission(PermissionType type)
		{
			return _checkPermission((int)type);
		}

		protected override void _RequestPermission(PermissionType type, GameObject obj, string function)
		{
			_requestPermission((int)type, obj.name, function);
		}

		protected override void _OpenAppSettings()
		{
			_openAppSettings();
		}

		protected override string _GetVersionString()
		{
			return _getOSVersion();
		}
#endif
	}
}

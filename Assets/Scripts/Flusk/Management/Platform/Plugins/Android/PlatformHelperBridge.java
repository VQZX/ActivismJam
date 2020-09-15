
package com.tfbgames.platformhelper;

import java.io.File;
import java.util.Locale;

import android.support.v4.content.ContextCompat;
import android.support.v4.app.ActivityCompat;
import android.content.ClipboardManager;
import android.content.ClipData;
import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.Intent;
import android.media.AudioManager;
import android.os.Build;
import android.os.Environment;
import android.os.StatFs;
import android.util.Log;
import android.net.Uri;
import android.app.Activity;
import android.app.AlarmManager;
import android.app.PendingIntent;
import android.provider.Settings;
import android.content.res.AssetManager;
import java.io.InputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import com.android.vending.expansion.zipfile.APKExpansionSupport;

import com.unity3d.player.UnityPlayer;



public class PlatformHelperBridge
{
	private static Context mContext = null;
	private static ClipboardManager mClipboard = null;
	private static String _permissionMessageGameObject;
	private static String _permissionMessageMethod;
	
	
	public static void setContext(Context context)
	{
		mContext = context;

		//can only access CLIPBOARD_SERVICE via the UIThread, so get it once off now at the start
		((Activity)mContext).runOnUiThread(new Runnable(){
			@Override
			public void run(){
				mClipboard = (ClipboardManager)mContext.getSystemService(Context.CLIPBOARD_SERVICE); 
				Log.d("UI thread", "I am the UI thread");
			}
		});
	}

	
	public static String getLocale()
	{
		return Locale.getDefault().toString();
	}


	public static String getCountryCode()
	{
		return Locale.getDefault().getCountry();
	}
 
	
	public static int getDiskSpaceFreeMegabytes()
	{
		File path = Environment.getRootDirectory();
		StatFs stat = new StatFs( path.getPath() );
		int availBlocks = stat.getAvailableBlocks();
		int blockSize = stat.getBlockSize();
		long free_space_bytes = ((long)availBlocks * (long)blockSize);
		int free_space_megabytes = (int)((free_space_bytes / 1024) / 1024);        
		return free_space_megabytes;
	}
	

	public static boolean getIsMusicPlaying()
	{
		boolean returnValue = false;
		if(mContext != null)
		{
//NOTE: Unity seemed to have changed the way they play audio in v5, so this *always* returns true currently
// http://answers.unity3d.com/questions/984710/detect-if-music-is-playing-in-other-apps-on-androi.html            
			AudioManager audioManager = (AudioManager)mContext.getSystemService(Context.AUDIO_SERVICE);
			returnValue = audioManager.isMusicActive();
		}
		return returnValue;
	}

	
	public static boolean getAreHeadphonesConnected()
	{
		boolean returnValue = false;
		if(mContext != null)
		{
			//NOTE: From the API documentation: "(isWiredHeadsetOn) was deprecated in API level 14. Use
			// only to check if a headset is connected or not."
			AudioManager audioManager = (AudioManager)mContext.getSystemService(Context.AUDIO_SERVICE);
			returnValue = audioManager.isWiredHeadsetOn();
		}
		return returnValue;
	}


	public static boolean getIsMicrophoneAvailable()
	{
		boolean returnValue = false;
		if(mContext != null)
		{
			returnValue = mContext.getPackageManager().hasSystemFeature(PackageManager.FEATURE_MICROPHONE);
		}
		return returnValue;
	}


	public static float getVolume()
	{
		float returnValue = 1;
		if(mContext != null)
		{
			AudioManager audioManager = (AudioManager)mContext.getSystemService(Context.AUDIO_SERVICE);
			int currVolume = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
			int maxVolume = audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
			returnValue = currVolume / (float)maxVolume;

			Log.d("PlatformHelperBridge", "curVol: " + Integer.toString(currVolume));
			Log.d("PlatformHelperBridge", "maxVol: " + Integer.toString(maxVolume));
		}
		else
		{
			Log.e( "PlatformHelperBridge", "No context for getVolume" );
		}
		return returnValue;
	}


	public static void setClipboard(String text)
	{
		if(mClipboard != null)
		{
			ClipData clip = ClipData.newPlainText(null, text);
			mClipboard.setPrimaryClip(clip); 
		}
	}


	public static String getClipboard()
	{
		String ret = null;
		if(mClipboard != null)
		{
			ClipData clip = mClipboard.getPrimaryClip();
			if(clip != null)
			{
				ClipData.Item item = clip.getItemAt(0);
				if(item != null)
				{
					ret = item.getText().toString();
				}
			}
		}
		return ret;
	}


	public static boolean canLaunchApp(String packageName)
	 {
		boolean ret = false;
		if(mContext != null)
		{
			if(mContext.getPackageManager().getLaunchIntentForPackage(packageName) != null)
			{
				ret = true;
			}
		}
		return ret;
	}


	public static void launchApp(String packageName)
	 {
		if(mContext != null)
		{
			Intent intent = mContext.getPackageManager().getLaunchIntentForPackage(packageName);
			if(intent != null)
			{
				intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
				mContext.startActivity(intent);
			}
		}
	}


	public static void openGooglePlayView(String packageAddress)
	{
		if(mContext != null)
		{
			Intent intent = new Intent(Intent.ACTION_VIEW);
			try 
			{
				intent.setData(Uri.parse("market://details?id=" + packageAddress));
				mContext.startActivity(intent);
			} 
			catch (android.content.ActivityNotFoundException anfe) 
			{
				intent.setData(Uri.parse("https://play.google.com/store/apps/details?id=" + packageAddress));
				mContext.startActivity(intent);
			}
		}
	}


	public static void openAmazonStoreView(String packageAddress)
	{
		if(mContext != null)
		{
			Intent intent = new Intent(Intent.ACTION_VIEW);
			try 
			{
				intent.setData(Uri.parse("amzn://apps/android?p=" + packageAddress));
				mContext.startActivity(intent);
			} 
			catch (android.content.ActivityNotFoundException anfe) 
			{
				intent.setData(Uri.parse("http://www.amazon.com/gp/mas/dl/android?p=" + packageAddress));
				mContext.startActivity(intent);
			}
		}
	}


	public static boolean checkPermission(String permission)
	{
		if(mContext != null)
		{
			return (ContextCompat.checkSelfPermission(mContext, permission) == PackageManager.PERMISSION_GRANTED) ? true : false;
		}
		return false;
	}


	public static boolean requestPermission(String permission, String gameObject, String method)
	{
		if(mContext != null)
		{
			_permissionMessageGameObject = gameObject;
			_permissionMessageMethod = method;
			Log.d("PlatformHelperBridge", "requestPermission: " + permission);            
			ActivityCompat.requestPermissions((Activity)mContext, new String[]{permission}, 0);
			return true;
		}
		return false;
	}


	public static void openAppSettings()
	{
		if(mContext != null)
		{
			Intent intent = new Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS, Uri.parse("package:" + mContext.getPackageName()));
			if(intent != null)
			{
				intent.addCategory(Intent.CATEGORY_DEFAULT);
				intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
				mContext.startActivity(intent);
			}
		}
	}


	public static String persistentDataPath(boolean preferExternal)
	{
		if(mContext != null)
		{
			//if we prefer external storage, we need to have that permission first
			if(preferExternal && checkPermission("android.permission.WRITE_EXTERNAL_STORAGE"))
			{
				return mContext.getExternalFilesDir(null).getAbsolutePath();
			}
			return mContext.getFilesDir().getAbsolutePath();
		}
		return null;
	}


	public static String copyAsset(String source, String destination)
	{
		AssetManager assetManager = mContext.getAssets();
		InputStream in = null;
		FileOutputStream out = null;
		String newFileName = null;

		try 
		{
			Log.i("PlatformHelperBridge", "copyAsset " + source);
			in = assetManager.open(source);
			out = new FileOutputStream(destination);

			byte[] buffer = new byte[4096];
			int read;
			while ((read = in.read(buffer)) != -1)
			{
				out.write(buffer, 0, read);
			}
		}
		catch (Exception e) 
		{
			String error = e.toString();
			Log.e("PlatformHelperBridge", "Error copying file: " + error);
			return error;
		}
		finally
		{
			try
			{
				in.close();
				out.close();
			}
			catch (IOException e) 
			{
				String error = e.toString();
				Log.e("PlatformHelperBridge", "Could not close stream: " + error);
				return error;
			}
		}

		return null;
	}


	public static int getVersionCode()
	{
		PackageManager manager = mContext.getPackageManager();
		String packageName = mContext.getPackageName();

		try
		{
			return manager.getPackageInfo(packageName, 0).versionCode;
		}
		catch (PackageManager.NameNotFoundException e)
		{
			Log.e("PlatformHelperBridge", "Package manager failed to find package with our name.");
			return -1;
		}
	}


	public static String copyAssetFromOBB(String source, String destination)
	{
		int versionCode = getVersionCode();
		InputStream in = null;
		FileOutputStream out = null;
		String newFileName = null;

		try 
		{
			source = "assets/" + source;
			Log.i("PlatformHelperBridge", "copyAsset " + source);
			in = APKExpansionSupport.getAPKExpansionZipFile(mContext, getVersionCode(), 0).getInputStream(source);
			out = new FileOutputStream(destination);

			byte[] buffer = new byte[4096];
			int read;
			while ((read = in.read(buffer)) != -1)
			{
				out.write(buffer, 0, read);
			}
		}
		catch (Exception e) 
		{
			String error = e.toString();
			Log.e("PlatformHelperBridge", "Error copying file: " + error);
			return error;
		}
		finally
		{
			try
			{
				in.close();
				out.close();
			}
			catch (IOException e) 
			{
				String error = e.toString();
				Log.e("PlatformHelperBridge", "Could not close stream: " + error);
				return error;
			}
		}

		return null;
	}




	//Internal Java callback
	public static void SendPermissionResult(String result, int code)
	{
		Log.d("PlatformHelperBridge", "SendPermissionResult: " + result + "   code: " + code);            
		UnityPlayer.UnitySendMessage(_permissionMessageGameObject, _permissionMessageMethod, result);
	}
}

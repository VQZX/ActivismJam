using System;
using Flusk.Patterns;
using UnityEngine;

namespace Flusk.Management.Platform
{
	[Serializable]
	public class TextureSet
	{
		public int MinRAM;
		public int MinVRes;
		public int TextureLimit;


		public void Set()
		{
			QualitySettings.masterTextureLimit = TextureLimit;
			Debug.Log("[PLATFORM] Reducing Texture Resolution by: " + QualitySettings.masterTextureLimit + " level(s)");		
		}

		public bool CheckAndSet(int vres, int ram)
		{
			if(((MinVRes <= 0) || (vres >= MinVRes)) && (ram >= MinRAM))
			{
				Set();
				return true;
			}
			return false;
		}
	}


	public class PlatformSettings : PersistentSingleton<PlatformSettings>
	{
		[SerializeField]
		protected TextureSet	HighResTextureSet;
		[SerializeField]
		protected TextureSet	NormalResTextureSet;
		[SerializeField]
		protected TextureSet	LowResTextureSet;
		[SerializeField]
		protected int			MaxAndroidResolution = -1;
		[SerializeField]
		protected bool			NoMSAA = false;
		[SerializeField]
		protected bool			NoAnisotropic = false;


		public static int RAM { get; private set; }
		


		protected override void Awake ()
		{
			base.Awake();

			//process some system information for our purposes
			RAM = (SystemInfo.systemMemorySize + SystemInfo.graphicsMemorySize);
			
Debug.LogFormat("[PLATFORM] System Memory {0}\nGraphics memory{1}", SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize);

			//make sure some settings are definitely disabled
			if(NoMSAA)
			{
				QualitySettings.antiAliasing = 0;
			}
			if(NoAnisotropic)
			{
				QualitySettings.anisotropicFiltering = 0;
			}

#if !UNITY_EDITOR && UNITY_ANDROID
			// HACK: Some Androids haven't rotated by the time we call this code, so we currently assume this is a landscape game
			int screenWidth, screenHeight;
			if (Screen.height > Screen.width)
			{
				screenWidth = Screen.height;
				screenHeight = Screen.width;
			}
			else
			{
				screenWidth = Screen.width;
				screenHeight = Screen.height;
			}
			
			//limit the vertical resolution of the game	on Android for memory and performance reasons
			if((MaxAndroidResolution > 0) && (screenHeight > MaxAndroidResolution))
			{
				float ratio = screenWidth / (float)screenHeight;
				
				int newHeight = MaxAndroidResolution;
				float width = MaxAndroidResolution * ratio;
				
				// Round to nearest 4
				int newWidth = Mathf.RoundToInt(width * 0.25f) * 4;
				
				Screen.SetResolution(newWidth, newHeight, true);
				Debug.LogFormat("[PLATFORM] Reducing Screen Resolution to: {0} x {1}", newWidth, newHeight);
			}
#else
			int screenWidth = Screen.width;
			int	screenHeight = Screen.height;
#endif

			//check which texture set we need to use
			if(!HighResTextureSet.CheckAndSet(screenHeight, RAM))
			{
				if(!NormalResTextureSet.CheckAndSet(screenHeight, RAM))
				{
					LowResTextureSet.Set();
				}			
			}
		}
	}
}

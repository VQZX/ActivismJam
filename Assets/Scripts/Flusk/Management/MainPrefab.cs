using Flusk.Patterns;
using System;
using UnityEngine;

namespace Flusk.Management
{
    public class MainPrefab : PersistentSingleton<MainPrefab>
    {
        [SerializeField] protected GameObject[] prefabs;

        public static event Action ManagementLoaded;

        public static void Load()
        {
            (Instance as MainPrefab).Initialise();
        }

        public void Initialise()
        {
            int count = prefabs.Length;
            for (int i = 0; i < count; ++i)
            {
                Instantiate(prefabs[i]);
            }
            if (ManagementLoaded != null)
            {
                ManagementLoaded();
            }
        }

        public void ForceSet()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

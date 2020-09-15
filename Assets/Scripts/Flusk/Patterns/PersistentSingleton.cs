using UnityEngine;

namespace Flusk.Patterns
{
    public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
    {
        protected override void Awake()
        {
            base.Awake();
            Object.DontDestroyOnLoad(transform.gameObject);
        }
    }
}

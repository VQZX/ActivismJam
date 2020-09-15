using UnityEngine;

namespace Flusk.Patterns
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; protected set; }

        public static bool InstanceExists
        {
            get { return Instance != null; }
        }

        public static bool TryGetInstance(out T instance)
        {
            instance = Instance;
            return InstanceExists;
        }

        protected virtual void Awake()
        {
            Set();
        }

        protected virtual void Set()
        {
            if (Instance == null)
            {
                Instance = (T) this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}

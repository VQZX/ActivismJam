using UnityEngine;

namespace Flusk
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T instance;
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
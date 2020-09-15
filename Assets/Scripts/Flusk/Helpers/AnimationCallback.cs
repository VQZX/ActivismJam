using UnityEngine;
using UnityEngine.Events;

namespace Flusk.Helpers
{
    public class AnimationCallback : MonoBehaviour
    {
        [SerializeField] protected UnityEvent[] callbacks;

        public void Invoke(int i)
        {
            int length = callbacks.Length;
            if (i >= length || i < 0)
            {
                Debug.LogWarning("Callback index does not exist");
                return;
            }
            callbacks[i].Invoke();
        }
        
    }
}
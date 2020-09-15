using UnityEngine;

namespace Flusk.Management
{
    public class InitBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Initialisation.StartUp();
        }
    }
}

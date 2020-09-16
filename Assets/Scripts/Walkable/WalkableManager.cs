using Flusk.Patterns;
using UnityEngine;

namespace Walkable
{
    public class WalkableManager : PersistentSingleton<WalkableManager>
    {
        [SerializeField]
        protected WalkableBaseSettings settings;
    }
}

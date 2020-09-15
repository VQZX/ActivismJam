using System;
using UnityEngine;

namespace Walkable
{
    [CreateAssetMenu(fileName = "WalkableSettings.asset", menuName = "Walkable/Settings", order = 0)]
    public class WalkableBaseSettings : ScriptableObject
    {
        [SerializeField]
        protected Plane planeInfo;
    }
}
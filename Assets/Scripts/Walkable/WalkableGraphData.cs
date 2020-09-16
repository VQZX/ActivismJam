using System;
using UnityEngine;

namespace Walkable
{
    [CreateAssetMenu(fileName = "WalkableGraphData", menuName = "Walkable/Graph Data", order = 0)]
    public class WalkableGraphData : ScriptableObject
    {
        [SerializeField]
        protected Graph walkGraph;

        public Graph WalkGraph => walkGraph;
        
    }
}
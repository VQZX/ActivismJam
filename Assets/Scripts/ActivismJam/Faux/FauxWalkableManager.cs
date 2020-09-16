using Flusk.Patterns;
using UnityEngine;
using Walkable;

namespace ActivismJam.Faux
{
    public class FauxWalkableManager : Singleton<FauxWalkableManager>
    {
        [SerializeField]
        protected WalkableGraphData graphData;

        public WalkableGraphData GraphData => graphData;
    }
}
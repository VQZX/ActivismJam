using System;
using System.Collections.Generic;
using UnityEngine;

namespace Walkable
{
    [CreateAssetMenu(fileName = "Graph.asset", menuName = "Walkable/Graph", order = 0)]
    public class Graph : ScriptableObject
    {
        [SerializeField]
        protected string title;
        public string Title => title;

        [SerializeField]
        protected string id;
        public string Id => id;
        
        [SerializeField]
        protected List<Area> areas;

        [SerializeField]
        protected List<Connection> connections;
    }
}
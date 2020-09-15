using System;
using System.Collections.Generic;
using UnityEngine;

namespace Walkable.Catalogue
{
    [CreateAssetMenu(fileName = "GraphCatalogue.asset", menuName = "Walkable/Graph", order = 0)]
    public class GraphCatalogue : ScriptableObject
    {
        [SerializeField]
        protected List<Graph> graph;
    }
}
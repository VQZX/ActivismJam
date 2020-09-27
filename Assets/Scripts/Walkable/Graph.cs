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
        protected string scene;
        public string Scene => scene;
        
        [SerializeField]
        protected List<Area> areas;

        [SerializeField]
        protected List<Connection> connections;

        /// <summary>
        /// Need to figure out why this is reversed
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointInside(Vector3 point)
        {
            foreach (var area in areas)
            {
                if (area.IsPointInside(point))
                {
                    return true;
                }
            }
            return false;
        }

        public Vector3 GetClosestPointOnGraph(Vector3 point)
        {
            // Go through the lines and fine smallest distance
            var currentClosestPoint = Vector3.one * float.MaxValue;
            var currentSmallestDistance = float.MaxValue;
            foreach (var area in areas)
            {
                var currentPoint = area.GetClosestPointOnPolygon(point);
                var distance = Vector3.Distance(point, currentPoint);
                if (distance < currentSmallestDistance)
                {
                    currentSmallestDistance = distance;
                    currentClosestPoint = currentPoint;
                }
            }
            return currentClosestPoint;
        }
    }
}
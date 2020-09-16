using System.Collections.Generic;
using UnityEngine;

namespace Walkable
{
    public class Path
    {
        public List<Point> points;
        public Point this[int index] => points[index];
    }

    public struct Point
    {
        public Vector3 position;
    }
}
using UnityEngine;

namespace Flusk.Geometry
{
    public struct Line
    {
        public Vector3 A, B;

        public Vector3 AtoB => B - A;

        public float SquareMagnitude => Vector3.SqrMagnitude(AtoB);

        public Line(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            var aToPoint = point - A;
            var dot = Vector3.Dot(aToPoint, AtoB);

            var directionFromAToPoint = dot / SquareMagnitude;

            return A + AtoB * directionFromAToPoint;
        }
    }
}
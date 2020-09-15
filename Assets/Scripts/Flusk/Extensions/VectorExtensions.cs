using System;
using UnityEngine;

namespace Flusk.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 IsolateComponents(this Vector2 vector, Vector2 referenceVector)
        {
            return new Vector2(
                Math.Abs(referenceVector.x) > float.Epsilon ? vector.x : 0, 
                Math.Abs(referenceVector.y) > float.Epsilon ? vector.y : 0);
        }
        
        public static Vector3 IsolateComponents(this Vector3 vector, Vector3 referenceVector)
        {
            return new Vector3(
                Math.Abs(referenceVector.x) > float.Epsilon ? vector.x : 0, 
                Math.Abs(referenceVector.y) > float.Epsilon ? vector.y : 0,
                Math.Abs(referenceVector.z) > float.Epsilon ? vector.z : 0);
        }
    }
}
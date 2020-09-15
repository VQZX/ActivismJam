using UnityEngine;

namespace Flusk.Extensions
{
    public static class QuaternionExtensions
    {
        public static Quaternion Difference(this Quaternion lhs, Quaternion rhs)
        {
            Quaternion inverse = lhs * Quaternion.identity;
            return inverse * rhs;
        }
    }
}
using UnityEngine;

namespace Flusk.Extensions
{
    public static class AnimationCurveExtensions
    {
        public static float GetFinalTime(this AnimationCurve curve)
        {
            int length = curve.length - 1;
            Keyframe key = curve.keys[length];
            float time = key.time;
            return time;
        }
    }
}
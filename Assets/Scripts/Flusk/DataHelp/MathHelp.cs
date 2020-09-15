using UnityEngine;

namespace Flusk.DataHelp
{
    public static class MathHelp
    {
        public static float Map(this float value, float min1, float max1, float min2, float max2)
        {
            float y = ((value - min1) / (max1 - min1)) * (max2 - min2) + min2;
            return y;
        }
        
        public static int Map(this int value, int min1, int max1, int min2, int max2)
        {
            int y = ((value - min1) / (max1 - min1)) * (max2 - min2) + min2;
            return y;
        }

        public static float GetMin( this AnimationCurve curve )
        {
            float check = 0;
            float min = Mathf.Infinity;
            float time = curve.keys[curve.length - 1].time;
            while( check < time )
            {
                check += Time.deltaTime;
                float current = curve.Evaluate(check);
                min = Mathf.Min(current, min);
            }
            return min;
        }

        public static float GetMax (this AnimationCurve curve)
        {
            float check = 0;
            float max = Mathf.Infinity;
            float time = curve.keys[curve.length - 1].time;
            while (check < time)
            {
                check += Time.deltaTime;
                float current = curve.Evaluate(check);
                max = Mathf.Max(current, max);
            }
            return max;
        }
    }
}

using System;
using Random = UnityEngine.Random;

namespace Flusk.Utility.Range
{
    [Serializable]
    public struct FloatRange
    {
        public float Min, Max;
        
        public float Size => Max - Min;

        public FloatRange(float min, float max)
        {
            Min = min;
            Max = max;
        }
        
        public float Rand()
        {
            return Random.Range(Min, Max);
        }
    }
}
using System;
using Random = UnityEngine.Random;

namespace Flusk.Utility.Range
{
    [Serializable]
    public struct IntRange
    {
        public int Min, Max;

        public int Size => Max - Min;

        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }
        
        public int Rand()
        {
            return Random.Range(Min, Max);
        }
    }
}
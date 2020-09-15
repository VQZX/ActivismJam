using System;
using UnityEngine;

namespace Flusk.Controls
{
    [Serializable]
    public struct KeyData
    {
        public KeyCode Code;
        public float AxisValue;
        public KeyState State;
    }
}
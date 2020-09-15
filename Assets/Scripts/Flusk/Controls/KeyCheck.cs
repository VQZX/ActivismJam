using System;
using UnityEngine;

namespace Flusk.Controls
{
    [Serializable]
    public struct KeyCheck
    {
        public string Description;
        public KeyCode Code;
        public KeyState State;

        private delegate bool KeyAction(KeyCode code);

        private KeyAction stateCheck;

        public void Init()
        {
            switch (State)
            {
                case KeyState.Down:
                {
                    stateCheck = Input.GetKeyDown;
                    break;
                }

                case KeyState.Continuous:
                {
                    stateCheck = Input.GetKey;
                    break;
                }

                case KeyState.Up:
                {
                    stateCheck = Input.GetKeyUp;
                    break;
                }
            }
        }

        public void NameCheck()
        {
            Description = ToString();
        }

        public bool Check()
        {
            bool state = stateCheck != null && stateCheck(Code);
            return state;
        }

        public override string ToString()
        {
            string output = string.Format("{0} {1}", State, Code);
            return output;
        }
    }
}
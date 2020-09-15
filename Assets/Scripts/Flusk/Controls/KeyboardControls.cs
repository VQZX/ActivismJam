using System;
using Flusk.Patterns;
using UnityEngine;

namespace Flusk.Controls
{
    public class KeyboardControls : PersistentSingleton<KeyboardControls>
    {
        [SerializeField] protected KeyCheck[] codes;

        public event Action<KeyData> KeyHit;

        public bool CheckKey(KeyCode code)
        {
            return Input.GetKey(code);
        }
        
        protected virtual void Start()
        {
            var count = codes.Length;
            for (var i = 0; i < count; ++i)
            {
                codes[i].Init();
            }
        }

        protected virtual void Update()
        {
            Check();
        }

        private void Check()
        {
            var count = codes.Length;
            for (var i = 0; i < count; ++i)
            {
                var current = codes[i];
;                if (!current.Check())
                {
                    continue;
                }
                KeyData data = new KeyData();
                data.Code = current.Code;
                data.AxisValue = 0;
                data.State = current.State;
                if (KeyHit != null)
                {
                    KeyHit(data);
                }
            }
        }

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            var count = codes.Length;
            for (var i = 0; i < count; ++i)
            {
                codes[i].NameCheck();
            }
        }
#endif
    }
}

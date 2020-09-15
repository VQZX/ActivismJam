using System;
using UnityEngine;

namespace Walkable
{
    [CreateAssetMenu(fileName = "Connection.asset", menuName = "Walkable/Connection", order = 0)]
    public class Connection : ScriptableObject
    {
        [SerializeField]
        protected string title;
        public string Title => title;

        [SerializeField]
        protected string id;
        public string Id => id;
        
        protected Area areaData;
    }

    public struct AreaData
    {
        public Area LeftHandSide, RightHandSide;
    }
    
}
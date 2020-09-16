using System.Collections.Generic;
using UnityEngine;

namespace Walkable
{
    [CreateAssetMenu(fileName = "Area.asset", menuName = "Walkable/Area", order = 0)]
    public class Area : ScriptableObject
    {
        [SerializeField]
        protected string title;
        public string Title => title;

        [SerializeField]
        protected string id;
        public string Id => id;
        
        [SerializeField]
        protected List<Vector3> areaPoints;
        
        #if UNITY_EDITOR
        public List<Vector3> AreaPoints
        {
            get { return areaPoints; }
            set { areaPoints = value; }
        }
        #endif
        
        protected List<ConnectionData> connectionData;
    }
}

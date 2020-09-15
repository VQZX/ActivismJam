using System.Collections.Generic;
using UnityEngine;

namespace Walkable
{
    [CreateAssetMenu(fileName = "Area.asset", menuName = "Walkable/Area", order = 0)]
    public class Area : ScriptableObject
    {
        [SerializeField]
        protected string name;
        public string Name => name;

        [SerializeField]
        protected string id;
        public string Id => id;
        
        [SerializeField]
        protected List<Vector3> areaPoints;

        [SerializeField]
        protected List<ConnectionData> connectionData;
    }
}

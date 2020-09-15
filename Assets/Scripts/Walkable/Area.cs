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

        [SerializeField]
        protected List<ConnectionData> connectionData;
    }
}

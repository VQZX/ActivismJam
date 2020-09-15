using System.Collections.Generic;
using UnityEngine;

namespace Walkable.Catalogue
{
    [CreateAssetMenu(fileName = "ConnectionCatalogue.asset", menuName = "Walkable/Connection Catalogue", order = 0)]
    public class ConnectionCatalogue : ScriptableObject
    {
        [SerializeField]
        protected List<Connection> connections;
    }
}
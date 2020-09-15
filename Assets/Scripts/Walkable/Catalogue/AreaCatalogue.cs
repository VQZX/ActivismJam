using System.Collections.Generic;
using UnityEngine;

namespace Walkable.Catalogue
{
    [CreateAssetMenu(fileName = "AreaCatalogue.asset", menuName = "Walkable/Area Catalogue", order = 0)]
    public class AreaCatalogue : ScriptableObject
    {
        [SerializeField]
        protected List<Area> area;
    }
}
#if UNITY_EDITOR
using Flusk.Attributes;
using UnityEngine;

namespace Flusk.DataHelp
{
    [ExecuteInEditMode]
    public class DisplayRelation : MonoBehaviour
    {
        [SerializeField]
        protected Transform target;

        [ReadOnly, SerializeField]
        protected Vector3 directionToTarget;

        [ReadOnly, SerializeField]
        protected float distanceToTarget;

        [ReadOnly, SerializeField]
        protected float heightAbove;

        protected virtual void Update()
        {
            if (target == null)
            {
                return;
            }
            directionToTarget = (target.position - transform.position);
            distanceToTarget = directionToTarget.magnitude;
            heightAbove = (transform.position.y - target.position.y);
            directionToTarget.Normalize();
        }
    }
}
#endif


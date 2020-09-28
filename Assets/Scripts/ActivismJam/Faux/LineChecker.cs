using Flusk.Geometry;
using UnityEngine;

namespace ActivismJam.Faux
{
    public class LineChecker : MonoBehaviour
    {
        [SerializeField]
        protected GameObject template;

        [SerializeField]
        protected Transform A, B;

        private GameObject cloneClick, cloneClosest;

        private Line line;
    
        private void Awake()
        {
            line = new Line(A.position, B.position);
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var screenPosition = Input.mousePosition;
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = A.position.z;

            DisplayClickPoint(worldPosition);
            DisplayClosestPoint(worldPosition);
        }

        private void DisplayClosestPoint(Vector3 worldPosition)
        {
            var closestPoint = line.GetClosestPoint(worldPosition);
            if (cloneClosest == null)
            {
                cloneClosest = Object.Instantiate(template, closestPoint, Quaternion.identity);
                cloneClosest.name = "Closet Point";
            }
            else
            {
                cloneClosest.transform.position = closestPoint;
            }
        }

        private void DisplayClickPoint(Vector3 worldPosition)
        {
            if (cloneClick == null)
            {
                cloneClick = Object.Instantiate(template, worldPosition, Quaternion.identity);
                cloneClick.name = "Click Point";
            }
            else
            {
                cloneClick.transform.position = worldPosition;
            }
        }
    }
}

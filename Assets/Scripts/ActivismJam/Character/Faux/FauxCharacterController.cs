using ActivismJam.Faux;
using UnityEngine;

namespace ActivismJam.Character.Faux
{
    public class FauxCharacterController : MonoBehaviour
    {
        [SerializeField]
        protected Camera sceneCamera;

        [SerializeField]
        protected float speed = 1.0f;

        private Vector3 currentGoal;
        private Vector3 currentOrigin;

        private float currentProgress;

        private bool canClick = true;
        
        /// <summary>
        /// TODO: Convert this to state machine
        /// </summary>
        private bool isMoving;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Clicked();
            }

            if (isMoving)
            {
                UpdateMethod();
            }
        }

        private void UpdateMethod()
        {
            currentProgress += Time.deltaTime * speed;
            var nextPosition = Vector3.Lerp(currentOrigin, currentGoal, currentProgress);
            transform.position = nextPosition;
            if (currentProgress >= 1)
            {
                isMoving = false;
            }
        }

        private void Clicked()
        {
            var mousePosition = Input.mousePosition;
            var worldPosition = sceneCamera.ScreenToWorldPoint(mousePosition);
            var worldPositionZ = transform.position.z;
            worldPosition.z = worldPositionZ;

            var instance = FauxWalkableManager.Instance;
            var walkGraph = instance.GraphData.WalkGraph;
            
            var isValid = walkGraph.IsPointInside(worldPosition);
            if (!isValid)
            {
                // Find closest point to edge of area if not inside graph
                worldPosition = walkGraph.GetClosestPointOnGraph(worldPosition);
                worldPosition.z = worldPositionZ;
            }
   
            currentOrigin = transform.position;
            currentGoal = worldPosition;
            currentProgress = 0;
            isMoving = true;
        }
    }
}
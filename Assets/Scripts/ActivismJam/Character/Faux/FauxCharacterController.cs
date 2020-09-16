using System;
using UnityEngine;

namespace ActivismJam.Character.Faux
{
    public class FauxCharacterController : MonoBehaviour
    {
        [SerializeField]
        protected Camera sceneCamera;

        [SerializeField]
        protected float speed;

        public string id;

        private Vector3 currentGoal;
        private Vector3 currentOrigin;

        private float currentProgress;
        
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
            worldPosition.z = transform.position.z;

            currentOrigin = transform.position;
            currentGoal = worldPosition;
            currentProgress = 0;
            isMoving = true;
        }
    }
}
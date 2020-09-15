using System;
using UnityEngine;

namespace ActivismJam.Character.Faux
{
    public class FauxCharacterController : MonoBehaviour
    {
        [SerializeField]
        protected Camera camera;

        [SerializeField]
        protected float speed;
        

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
            transform.localPosition = nextPosition;
            if (currentProgress >= 1)
            {
                isMoving = false;
            }
        }

        private void Clicked()
        {
            var mousePosition = Input.mousePosition;
            var worldPosition = camera.ScreenToViewportPoint(mousePosition);
            worldPosition.z = transform.localPosition.z;

            currentOrigin = transform.localPosition;
            currentGoal = worldPosition;
            currentProgress = 0;
            isMoving = true;
        }
    }
}
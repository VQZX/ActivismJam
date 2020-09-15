using Flusk.Patterns;
using UnityEngine;

namespace Flusk.Management
{

    /// <summary>
    /// Class for keeping tracking of all the mousey things
    /// </summary>
    public class MouseManager : Singleton<MouseManager>
    {
        [SerializeField]
        protected int sampleAmount = 10;
        
        public Vector2 Delta { get; protected set; }
        public Vector2 ScreenPosition { get; protected set; }
        public Vector2 ViewPosition { get; protected set; }
        public Vector2 DistanceFromOrigin { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Vector2 MouseAxis { get; protected set; }
        public Vector2 ExpandedAxis { get; protected set; }
        
        public Vector2 AverageAxis { get; protected set; }
        public Vector2 AverageExpandedAxis { get; protected set; }

        protected Vector2 previousScreen;

        private static readonly Vector2 ScreenViewCenter = new Vector2(0.5f, 5f);
        private readonly Vector2 origin = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        private Vector2 accumulativeAxis;
        private int currentSample;

        public void GetMotionData(out Vector3 euler, out float angle)
        {
            var rotationDelta = MouseManager.Instance.Delta;
            euler = new Vector3(-rotationDelta.y, rotationDelta.x);
            angle = Mathf.Atan2(euler.y, euler.x);
        }

        public void GetMotionDataStatic(out Vector3 euler, out float angle)
        {
            Vector2 difference = ViewPosition - ScreenViewCenter;
            euler = new Vector3(-difference.y, difference.x);
            angle = Mathf.Atan2(difference.y, difference.x);
        }

        public bool IsMouseOffScreen()
        {
            return ScreenPosition.x < 0 || ScreenPosition.y < 0 || ScreenPosition.x > Screen.width ||
                   ScreenPosition.y > Screen.height;
        }
        

        protected virtual void Start()
        {
            ScreenPosition = Input.mousePosition;
            ViewPosition = Camera.main.ScreenToViewportPoint(ScreenPosition);
        }

        protected virtual void Update()
        {
            Delta = ((Vector2) Input.mousePosition) - ScreenPosition;
            ScreenPosition = Input.mousePosition;
            ViewPosition = Camera.main.ScreenToViewportPoint(ScreenPosition);
            DistanceFromOrigin = ScreenPosition - origin;
            Velocity = Delta / Time.deltaTime;

            MouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            ExpandedAxis = new Vector2(MouseAxis.x * Screen.width, MouseAxis.y * Screen.height);

            accumulativeAxis += MouseAxis;
            currentSample++;
            if (currentSample < sampleAmount)
            {
                return;
            }
            AverageAxis = accumulativeAxis / sampleAmount;
            AverageExpandedAxis = new Vector2(AverageAxis.x * Screen.width, AverageAxis.y * Screen.height);
        }
    }
}

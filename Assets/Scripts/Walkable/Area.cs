using System.Collections.Generic;
using Flusk.Geometry;
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

        private List<Line> lines = new List<Line>();
        
        private bool LinesInitialised { get; set; }
        
#if UNITY_EDITOR
        public List<Vector3> AreaPoints
        {
            get { return areaPoints; }
            set { areaPoints = value; }
        }
#endif
        
        protected List<ConnectionData> connectionData;

        public bool IsPointInside(Vector3 point)
        {
	        if (!LinesInitialised)
	        {
		        CreateLines();
	        }
	        
	        if (IsOnLine(point))
	        {
		        return true;
	        }

	        var line = new Line(point, new Vector2(100000, point.y));
	        var amountOfLine = 0;
	        
	        if (IsIntersectingWithLines(line, ref amountOfLine))
	        {
		        Debug.Log("Amount Of Lines "+amountOfLine);
		        return amountOfLine % 2 == 1;
	        }
	        Debug.Log("Point not inside");
	        return false;
        }

        private bool IsIntersectingWithLines(Line lineA, ref int amount)
        {
	        bool isIntersecting = false;
	        CreateLines();
	        Debug.Log("Line count "+lines.Count);
	        foreach (var line in lines)
	        {
		        Vector2 point = Vector2.zero;
		        if (lineA.IsIntersecting(line, ref point))
		        {
			        isIntersecting = true;
			        amount++;
			        Debug.LogFormat($"Intersecting: {isIntersecting} -- Times: {amount}");
		        }
	        }
	        return isIntersecting;
        }

        private bool IsOnLine(Vector3 point)
        {
	        foreach (var line in lines)
	        {
		        if (line.IsOnLine(point))
		        {
			        //Debug.Log("On Line");
			        return true;
		        }
	        }
			//Debug.Log("Not On Line");
	        return false;
        }

        // Assume Outside of polygon
        public Vector3 GetClosestPointOnPolygon(Vector3 point)
        {
	        CreateLines();
	        // Go through the lines and fine smallest distance
	        var currentClosestPoint = Vector3.one * float.MaxValue;
	        var currentSmallestDistance = float.MaxValue;
	        foreach (var line in lines)
	        {
		        var pointOnPolygon = line.GetClosestPoint(point);
		        var distance = Vector3.Distance(point, pointOnPolygon);
		        if (distance < currentSmallestDistance)
		        {
			        currentSmallestDistance = distance;
			        currentClosestPoint = pointOnPolygon;
		        }
	        }
	        Debug.Log($"Distance: {currentSmallestDistance}, Point: ${currentClosestPoint}");
	        return currentClosestPoint;
        }

        private void CreateLines()
        {
	        lines = new List<Line>();
	        var length = areaPoints.Count;
	        for (int i = 0; i < length - 1; i++)
	        {
		        var a = areaPoints[i];
		        var b = areaPoints[i + 1];
		        lines.Add(new Line(a, b));
		        if (i == length - 2)
		        {
			        lines.Add(new Line(areaPoints[length - 1], areaPoints[0]));
		        }
	        }

	        LinesInitialised = lines.Count > 0;
        }

    }
}

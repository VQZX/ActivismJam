using System;
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
	        var n = areaPoints.Count;
	        // There must be at least 3 vertices in polygon[] 
	        if (n < 3) 
	        { 
		        return false; 
	        } 

	        // Create a point for line segment from p to infinite 
	        Vector3 extreme = new Vector3(float.MaxValue, point.y); 

	        // Count intersections of the above line 
	        // with sides of polygon 
	        int count = 0, i = 0; 
	        do
	        { 
		        int next = (i + 1) % n; 

		        // Check if the line segment from 'p' to 
		        // 'extreme' intersects with the line 
		        // segment from 'polygon[i]' to 'polygon[next]' 
		        if (IsIntersecting(areaPoints[i], 
			        areaPoints[next], point, extreme)) 
		        { 
			        // If the point 'p' is colinear with line 
			        // segment 'i-next', then check if it lies 
			        // on segment. If it lies, return true, otherwise false 
			        if (Collinearity(areaPoints[i], point, areaPoints[next]) == 0) 
			        { 
				        return OnSegment(areaPoints[i], point, 
					        areaPoints[next]); 
			        } 
			        count++; 
		        } 
		        i = next; 
	        } while (i != 0); 

	        // Return true if count is odd, false otherwise 
	        return (count % 2 == 1); // Same as (count%2 == 1) 
        }

        public static bool IsIntersecting(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
        {
	        int o1 = Collinearity(p1, q1, p2); 
	        int o2 = Collinearity(p1, q1, q2); 
	        int o3 = Collinearity(p2, q2, p1); 
	        int o4 = Collinearity(p2, q2, q1); 
	        
	        // General case 
	        if (o1 != o2 && o3 != o4) 
	        { 
		        return true; 
	        } 

	        // Special Cases 
	        // p1, q1 and p2 are colinear and 
	        // p2 lies on segment p1q1 
	        if (o1 == 0 && Area.OnSegment(p1, p2, q1)) 
	        { 
		        return true; 
	        } 

	        // p1, q1 and p2 are colinear and 
	        // q2 lies on segment p1q1 
	        if (o2 == 0 && Area.OnSegment(p1, q2, q1)) 
	        { 
		        return true; 
	        } 

	        // p2, q2 and p1 are colinear and 
	        // p1 lies on segment p2q2 
	        if (o3 == 0 && Area.OnSegment(p2, p1, q2)) 
	        { 
		        return true; 
	        } 

	        // p2, q2 and q1 are colinear and 
	        // q1 lies on segment p2q2 
	        if (o4 == 0 && Area.OnSegment(p2, q1, q2)) 
	        { 
		        return true; 
	        } 

	        // Doesn't fall in any of the above cases 
	        return false; 
        }

        public static int Collinearity(Vector3 p, Vector3 q, Vector3 r)
        {
            var val = (q.y - p.y) * (r.x - q.x) -  
                      (q.x - p.x) * (r.y - q.y); 
  
            if (Math.Abs(val) < float.Epsilon)  
            { 
                return 0; // colinear 
            } 
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        static bool OnSegment(Vector3 p, Vector3 q, Vector3 r) 
        { 
	        if (q.x <= Math.Max(p.x, r.x) && 
	            q.x >= Math.Min(p.x, r.x) && 
	            q.y <= Math.Max(p.y, r.y) && 
	            q.y >= Math.Min(p.y, r.y)) 
	        { 
		        return true; 
	        } 
	        return false; 
        }

        // Assume Outside of polygon
        public Vector3 GetClosestPointOnPolygon(Vector3 point)
        {
	        var lines = new List<Line>();
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

        public static Vector3 Intersect(Vector3 line1V1, Vector3 line1V2, Vector3 line2V1, Vector3 line2V2)
        {
	        //Line1
	        float A1 = line1V2.y - line1V1.y;
	        float B1 = line1V2.x - line1V1.x;
	        float C1 = A1*line1V1.x + B1*line1V1.x;

	        //Line2
	        float A2 = line2V2.y - line2V1.y;
	        float B2 = line2V2.x - line2V1.x;
	        float C2 = A2 * line2V1.x + B2 * line2V1.y;

	        float det = A1*B2 - A2*B1;
	        if (det == 0)
	        {
		        throw new Exception("Parallel Lines do not intersect");
	        }
	        else
	        {
		        float x = (B2*C1 - B1*C2)/det;
		        float y = (A1 * C2 - A2 * C1) / det;
		        return new Vector3(x,y,0);
	        }
        }
    }
}

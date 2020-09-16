﻿using System;
using System.Collections.Generic;
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
        
    }
}

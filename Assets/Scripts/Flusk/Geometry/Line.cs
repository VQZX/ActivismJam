using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Flusk.Geometry
{
    public struct Line
    {
        public Vector2 A, B;

        public Vector2 AtoB => B - A;
        
        public Vector2 Direction { get; private set; }

        public float Gradient { get; private set; }
        
        public float Constant { get; private set; }

        public float SquareMagnitude => Vector3.SqrMagnitude(AtoB);
        
        public Line(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
            Direction = (B - A).normalized;
            Gradient = (B.y - A.y)/(B.x - A.x);
            
            // y = mx + c
            // c = y - mx;

            Constant = A.y - Gradient * A.x;
        }

        public Vector3 GetClosestPoint(Vector2 point)
        {
            // Edge case 1: on the line itself
            if (IsOnLine(point))
            {
                return point;
            }

            // Edge case 2: past Maximum x and y, and past minimum x, and y
            if (IsPastExtents(point, out var extentsPoint))
            {
                return extentsPoint;
            }
            return ReturnCloseStPointWhenInRange(point);
        }

        public bool IsOnLine(Vector2 point)
        {
            var y = Gradient * point.x + Constant;
            return Math.Abs(y - point.y) < float.Epsilon;
        }

        public bool IsPastExtents(Vector2 point, out Vector2 extentsPoint)
        {
            var xMin = Mathf.Min(A.x, B.x);
            var xMax = Mathf.Max(A.x, B.x);
            var yMin = Mathf.Min(A.y, B.y);
            var yMax = Mathf.Max(A.y, B.y);
            
            if (Gradient > 0)
            {
                // A(xMax, yMax)
                // B(xMin, yMin)
                
                // past A
                if (point.x > xMax && point.y > yMax)
                {
                    extentsPoint = A;
                    return true;
                }
                // point B
                if (point.x < xMin && point.y < yMin)
                {
                    extentsPoint = B;
                    return true;
                }
                extentsPoint = Vector2.zero;
                return false;
            }

            if (Gradient < 0)
            {
                // A(xMin, yMax)
                // B(xMax, yMin)

                // past A
                if (point.x < xMin && point.y > yMax)
                {
                    extentsPoint = A;
                    return true;
                }
                // point B
                if (point.x > xMax && point.y < yMin)
                {
                    extentsPoint = B;
                    return true;
                }
                extentsPoint = Vector2.zero;
                return false;
            }

            extentsPoint = Vector2.zero;
            return false;

        }

        private Vector3 ReturnCloseStPointWhenInRange(Vector2 point)
        {
            var perpendicularDirection = -Vector2.Perpendicular(Direction);

            var secondPoint = point + perpendicularDirection * float.MaxValue;

            var line = new Line(point, secondPoint);

            // y = m1x + c1
            // y = m2x + c2

            // m1x + c1 = m2x + c2
            // x(m1-m2) = c2 - c1
            // x = (c2-c1)/(m1-m2)

            var result = new Vector2();

            result.x = (line.Constant - Constant) / (Gradient - line.Gradient);
            result.y = Gradient * result.x + Constant;
            //TODO: refactor this to be more elegant
            Vector2 pastExtents;
            if (IsPastExtents(result, out pastExtents))
            {
                return pastExtents;
            }
            
            return result;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Visualization2D
{
    public class Triangle
    {
        public Vector2 V1 => points[0];
        public Vector2 V2 => points[1];
        public Vector2 V3 => points[2];

        private Vector2[] points;


        public Vector2 Circumcenter;
        public float Radius;

        public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            points = new[] {v1, v2, v3};
            CalculateCircumcenter();
        }

        public Vector2 CalculateCircumcenter()
        {
            var A = Vector2.zero;
            var B = V2 - V1;
            var C = V3 - V1;

            var D = 2 * (B.x * C.y - B.y * C.x);

            var Ux = (C.y * (B.x * B.x + B.y * B.y) - B.y * (C.x * C.x + C.y * C.y)) / D;
            var Uy = (B.x * (C.x * C.x + C.y * C.y) - C.x * (B.x * B.x + B.y * B.y)) / D;

            Circumcenter = new Vector2(Ux, Uy) + V1;
            Radius = new Vector2(Ux, Uy).magnitude;


            return new Vector2(Ux, Uy) + V1;
        }

        public Vector2 CalculateCentroidPosition()
        {
            var c = (V1 + V2 + V3) / 3;
            return c;
        }

        public bool HasVertex(Vector2 vertex)
        {
            return points.Contains(vertex);
        }

        public bool IsPointInsideCircumcircle(Vector2 point)
        {
            return Radius >= (point - Circumcenter).magnitude;
        }

        public Edge[] GetEdges()
        {
            return new[] {new Edge(V1, V2), new Edge(V2, V3), new Edge(V3, V1)};
        }
    }
}

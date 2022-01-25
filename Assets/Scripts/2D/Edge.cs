using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Visualization2D
{


    public class Edge
    {
        public Vector2 Start => points[0];
        public Vector2 End  => points[1];

        private Vector2[] points;

        public Edge(Vector2 start, Vector2 end)
        {
            points = new[] {start, end};
        }

        public bool Equals(Edge other)
        {
            return !(points.Except(other.points).Any());
        }
    }
}

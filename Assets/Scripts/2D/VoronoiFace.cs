using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoronoiFace
{
    public Plane Plane;
    public List<Vector3> points;


    public VoronoiFace(List<Vector3> points, Plane plane)
    {
        Plane = plane;
        this.points = points;
    }

    public List<Edge> GetEdges()
    {
        List<Edge> edges = new List<Edge>();
        for (var index = 0; index < points.Count; index++)
        {
            var point = points[index];
            Vector3 point2 = Vector3.zero;
            if (index + 1 == points.Count) point2 = points[0];
            else point2 = points[index + 1];

            var e = new Edge(point, point2);
            edges.Add(e);
        }

        return edges;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


        SortEdges();
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

    public void SortEdges()
    {
        Vector3 mid = Vector3.zero;
        foreach (var vector3 in points)
        {
            mid += vector3;
        }

        mid /= points.Count;

        var refVec = (points[0] - mid).normalized;

        var p = this.Plane;
        points = points.OrderBy(vert =>
            Mathf.Sign(Vector3.Dot(Vector3.Cross(vert - mid, refVec), p.normal)) *
            Mathf.Atan2(Vector3.Cross((vert - mid).normalized, refVec).magnitude, Vector3.Dot((vert - mid).normalized, refVec))).ToList();
    }
}

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
        var rotationQuat = Quaternion.FromToRotation(Plane.normal, Vector3.back);
        var inverseQuat = Quaternion.Inverse(rotationQuat);
        List<Vector3> rotatedPoints = new List<Vector3>();
        foreach (var vector3 in points)
        {
            rotatedPoints.Add(rotationQuat * vector3);
        }

        Vector3 mid = Vector3.zero;
        foreach (var vector3 in rotatedPoints)
        {
            mid += vector3;
        }

        mid /= points.Count;

        rotatedPoints = rotatedPoints.OrderBy(t => Math.Atan2(t.y - mid.y, t.x - mid.y)).ToList();
        points = rotatedPoints.Select(x => inverseQuat * x).ToList();
    }
}

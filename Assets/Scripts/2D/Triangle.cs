using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public struct Triangle
{
    public Vector3 V1 => points[0];
    public Vector3 V2 => points[1];
    public Vector3 V3 => points[2];

    private Vector3[] points;


    public Vector3 Circumcenter;
    public float Radius;

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) : this()
    {
        points = new[] {v1, v2, v3};
        CalculateCircumcenter();
    }

    public void CalculateCircumcenter()
    {
        var A = Vector3.zero;
        var B = V2 - V1;
        var C = V3 - V1;

        var D = 2 * (B.x * C.y - B.y * C.x);

        var Ux = (C.y * (B.x * B.x + B.y * B.y) - B.y * (C.x * C.x + C.y * C.y)) / D;
        var Uy = (B.x * (C.x * C.x + C.y * C.y) - C.x * (B.x * B.x + B.y * B.y)) / D;

        Circumcenter = new Vector3(Ux, Uy) + V1;
        Radius = new Vector3(Ux, Uy).magnitude;

        // var edges = GetEdges();
        //
        // Edge a = edges[0];
        // Edge b = edges[1];
        // float x = 0, y = 0;
        // if (float.IsInfinity(a.f))
        // {
        //     x = a.Mid.x;
        //     y = b.f * x + b.g;
        // }
        // else if (double.IsInfinity(b.f))
        // {
        //     x = b.Mid.x;
        //     y = a.f * x + a.g;
        // }
        // else
        // {
        //     x = (b.g - a.g) / (a.f - b.f);
        //     y = b.f * x + b.g;
        // }
        //
        // Circumcenter = new Vector3(x, y);
        // Radius = (new Vector3(V1.x, V2.y) - Circumcenter).magnitude;
    }

    public Vector3 CalculateCentroidPosition()
    {
        var c = (V1 + V2 + V3) / 3;
        return c;
    }

    public bool HasVertex(Vector3 vertex)
    {
        foreach (var vector3 in points)
        {
            if (Mathf.Abs(Vector3.Distance(vector3, vertex)) < 0.0001f)
            {
                return true;
            }
        }
        return false;

        return points.Contains(vertex);
    }

    public bool IsPointInsideCircumcircle(Vector3 point)
    {
        return Radius - (point - Circumcenter).magnitude > 0;
    }

    public Vector3[] GetVertices()
    {
        return points;
    }

    public Edge[] GetEdges()
    {
        return new[] {new Edge(V1, V2), new Edge(V2, V3), new Edge(V3, V1)};
    }

    private bool Approximately(Vector3 me, Vector3 other, float allowedDifference)
    {
        var dx = me.x - other.x;
        if (Mathf.Abs(dx) > allowedDifference)
            return false;

        var dy = me.y - other.y;
        if (Mathf.Abs(dy) > allowedDifference)
            return false;

        var dz = me.z - other.z;

        return Mathf.Abs(dz) >= allowedDifference;
    }
}

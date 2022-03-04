using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Edge
{
    public Vector3 Start
    {
        get => points[0];
        set => points[0] = value;
    }

    public Vector3 End
    {
        get => points[1];
        set => points[1] = value;
    }

    public Vector3 Mid;

    public float f;
    public float g;

    private Vector3[] points;

    public Edge(Vector3 start, Vector3 end)
    {
        points = new[] {start, end};

        Mid = new Vector3((start.x + end.x) * 0.5f, (start.y + end.y) * 0.5f);

        f = (start.x - end.x) / (end.y - start.y);
        g = Mid.y - f * Mid.x;
    }

    public bool Equals(Edge other)
    {
        return !(points.Except(other.points).Any());
    }
}

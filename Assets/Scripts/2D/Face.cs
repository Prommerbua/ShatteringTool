using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Face
{
    private Vector3[] points;

    public Vector3 p1 => points[0];
    public Vector3 p2 => points[1];
    public Vector3 p3 => points[2];

    public Face(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        points = new[] {p1, p2, p3};
    }

    public bool Equals(Face other)
    {
        return !(points.Except(other.points).Any());
    }
}

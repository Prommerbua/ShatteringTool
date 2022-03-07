using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Tetraeder
{
    public Vector3 p1 => points[0];
    public Vector3 p2 => points[1];
    public Vector3 p3 => points[2];
    public Vector3 p4 => points[3];

    private Vector3[] points;
    private List<Tetraeder> neighbors;

    public bool isBad;

    public Vector3 circumcenter;
    public float radius;

    public Tetraeder(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) : this()
    {
        points = new[] {p1, p2, p3, p4};
        neighbors = new List<Tetraeder>();

        calculateCircumsphere();
    }

    private void calculateCircumsphere()
    {
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;
        Vector3 v3 = p4 - p1;

        float l1 = v1.sqrMagnitude;
        float l2 = v2.sqrMagnitude;
        float l3 = v3.sqrMagnitude;
        circumcenter = p1 + (l1 * Vector3.Cross(v2, v3) + l2 * Vector3.Cross(v3, v1) + l3 * Vector3.Cross(v1, v2)) /
            (2 * Vector3.Dot(v1, Vector3.Cross(v2, v3)));
        radius = (p1 - circumcenter).magnitude;
        // circumcenter = circumcenter * 1000;
        // circumcenter = new Vector3((int) circumcenter.x / 1000.0f, (int) circumcenter.y / 1000.0f, (int) circumcenter.z / 1000.0f);
    }

    public Face[] GetFaces()
    {
        return new[] {new Face(p1, p2, p3), new Face(p2, p3, p4), new Face(p3, p4, p1), new Face(p4, p1, p2)};
    }

    public bool HasVertex(Vector3 vertex)
    {
        return points.Contains(vertex);
    }

    public bool IsPointInsideCircumSphere(Vector3 point)
    {
        return radius >= (point - circumcenter).magnitude;
    }

    public Vector3[] GetVertices()
    {
        return points;
    }
}

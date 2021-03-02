using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector2 V1;
    public Vector2 V2;
    public Vector2 V3;

    public Edge E1;
    public Edge E2;
    public Edge E3;

    private Vector2 _circumcirclePosition;
    public float CircumcircleRadius;

    public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        this.V1 = v1;
        this.V2 = v2;
        this.V3 = v3;

        E1 = new Edge(V1, V2);
        E2 = new Edge(V2, V3);
        E3 = new Edge(V3, V1);
    }

    public Vector2 CalculateCircumcirclePosition()
    {
        var A = Vector2.zero;
        var B = V2 - V1;
        var C = V3 - V1;

        var D = 2 * (B.x * C.y - B.y * C.x);

        var Ux = (C.y * (B.x * B.x + B.y * B.y) - B.y * (C.x * C.x + C.y * C.y)) / D;
        var Uy = (B.x * (C.x * C.x + C.y * C.y) - C.x * (B.x * B.x + B.y * B.y)) / D;

        _circumcirclePosition = new Vector2(Ux,Uy) + V1;
        CircumcircleRadius =    new Vector2(Ux, Uy).magnitude;


        return new Vector2(Ux,Uy) + V1;
    }

    public Vector2 CalculateCentroidPosition()
    {
        var c =  (V1 + V2 + V3) / 3;
        return c;
    }

    public bool IsPointInsideCircumcircle(Vector2 point)
    {
        var length = (_circumcirclePosition - point).magnitude;
        return CircumcircleRadius - length > 0;
    }

    public List<Edge> GetEdges()
    {
        List<Edge> edges = new List<Edge>();

        edges.Add(E1);
        edges.Add(E2);
        edges.Add(E3);
        return edges;
    }

    public List<Vector2> GetVertices()
    {
        List<Vector2> vertices = new List<Vector2>();

        vertices.Add(V1);
        vertices.Add(V2);
        vertices.Add(V3);
        return vertices;
    }
}

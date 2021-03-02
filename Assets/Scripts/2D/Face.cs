using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public Edge2D incEdge;

    public Face(Edge2D incEdge)
    {
        this.incEdge = incEdge;
    }

    private Vector2 _circumcirclePosition;
    public float CircumcircleRadius;




    public bool IsPointInsideCircumcircle(Vector2 point)
    {
        var length = (_circumcirclePosition - point).magnitude;
        return CircumcircleRadius - length > 0;
    }

    public Vector2 CalculateCentroidPosition()
    {
        var vertices = incEdge.GetTriangleVertices();
        var v1 = vertices[0].Pos;
        var v2 = vertices[1].Pos;
        var v3 = vertices[2].Pos;

        var c =  (v1 + v2 + v3) / 3;
        return c;
    }

    public List<Edge2D> GetEdges()
    {
        List<Edge2D> edges = new List<Edge2D>();

        edges.Add(incEdge);
        edges.Add(incEdge.next);
        edges.Add(incEdge.prev);
        return edges;
    }

    public List<Vertex2D> GetVertices()
    {
        List<Vertex2D> vertices = new List<Vertex2D>();

        vertices.Add(incEdge.org);
        vertices.Add(incEdge.twin.org);
        vertices.Add(incEdge.prev.org);
        return vertices;
    }
}

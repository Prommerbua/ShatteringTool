using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Edge2D incEdge;

    public Triangle(Edge2D incEdge)
    {
        this.incEdge = incEdge;
    }

    public bool isBad = false;
    private Vector2 _circumcirclePosition;
    private float _circumcircleRadius;


    public Vector2 CalculateCircumcirclePosition()
    {
        //get triangle vertices
        var vertices = incEdge.GetTriangleVertices();
        var v1 = vertices[0];
        var v2 = vertices[1];
        var v3 = vertices[2];

        var D = 2 * (v1.Pos.x * (v2.Pos.y - v3.Pos.y) + v2.Pos.x * (v3.Pos.y - v1.Pos.y) +
                     v3.Pos.x * (v2.Pos.y - v1.Pos.y));

        var Ux = (1 / D) * ((v1.Pos.x * v1.Pos.x + v1.Pos.y * v1.Pos.y) * (v2.Pos.y - v3.Pos.y) +
                            (v2.Pos.x * v2.Pos.x + v2.Pos.y * v2.Pos.y) * (v3.Pos.y - v1.Pos.y) +
                            (v3.Pos.x * v3.Pos.x + v3.Pos.y * v3.Pos.y) * (v1.Pos.y - v2.Pos.y));

        var Uy = (1 / D) * ((v1.Pos.x * v1.Pos.x + v1.Pos.y * v1.Pos.y) * (v3.Pos.x - v2.Pos.x) +
                            (v2.Pos.x * v2.Pos.x + v2.Pos.y * v2.Pos.y) * (v1.Pos.x - v3.Pos.x) +
                            (v3.Pos.x * v3.Pos.x + v3.Pos.y * v3.Pos.y) * (v2.Pos.x - v1.Pos.x));

        _circumcirclePosition = new Vector2(Ux,Uy);
        _circumcircleRadius = (_circumcirclePosition - v1.Pos).magnitude;

        return new Vector2(Ux,Uy);
    }

    public bool IsPointInsideCircumcircle(Vector2 point)
    {
        var length = (_circumcirclePosition - point).magnitude;
        return _circumcircleRadius - length > 0;
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

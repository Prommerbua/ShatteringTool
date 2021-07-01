using System;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : IEquatable<Triangle>
{
    public static int idCounter;
    public DelaunayVertex2D p1, p2, p3;
    public Edge[] edges;

    public Vector3 circumceter;
    public Vector3 centerOfMass;
    public double radius;

    public bool bad = false;

    private int id;
    public Triangle(DelaunayVertex2D p1, DelaunayVertex2D p2, DelaunayVertex2D p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;

        edges = new Edge[3];

        edges[0] = new Edge(p1, p2, this);
        edges[1] = new Edge(p2, p3, this);
        edges[2] = new Edge(p3, p1, this);
        id = idCounter++;
        calculateCircumcenter();

    }

    public Triangle(DelaunayVertex2D p1, Edge e, Triangle old)
    {
        this.p1 = p1;
        p2 = e.start;
        p3 = e.end;

        edges = new Edge[3];
        edges[0] = new Edge(p1, p2, this);
        edges[1] = new Edge(p1, p3, this);

        if (e.left == old)
        {
            e.left = this;
        }
        else if (e.right == old)
        {
            e.right = this;
        }


        edges[2] = e;
        id = idCounter++;
        calculateCircumcenter();
    }

    //Find and return neighbours of a triangle
    public List<Triangle> Neighbours()
    {
        List<Triangle> neigbours = new List<Triangle>();
        foreach (Edge e in edges)
        {
            if (e.left == this && e.right != null) neigbours.Add(e.right);
            if (e.right == this && e.left != null) neigbours.Add(e.left);
        }
        return neigbours;
    }

    //Calculate circumcenter of the triangle
    private void calculateCircumcenter()
    {

        Edge a = edges[0];
        Edge b = edges[1];
        float x = 0, y = 0;
        if (float.IsInfinity(a.f))
        {
            x = a.mid.x;
            y = b.f * x + b.g;
        }
        else if (float.IsInfinity(b.f))
        {
            x = b.mid.x;
            y = a.f * x + a.g;
        }
        else
        {
            x = (b.g - a.g) / (a.f - b.f);
            y = b.f * x + b.g;
        }
        circumceter = new Vector3(x, y);
        radius = (new Vector3(p1.loc.x, p1.loc.y) - circumceter).sqrMagnitude;

        centerOfMass = (p1.loc + p2.loc + p3.loc) / 3;
    }

    //Is new added point inside the circumcircle of the point
    public bool withinRange(DelaunayVertex2D p)
    {
        return radius - (new Vector3(p.loc.x, p.loc.y) - circumceter).sqrMagnitude > 0;
    }

    //Are two triangles shareing an edge
    public Edge shareEdge(Triangle t)
    {
        if (t.hasEdge(edges[0])) return edges[0];
        if (t.hasEdge(edges[1])) return edges[1];
        if (t.hasEdge(edges[2])) return edges[2];
        return null;
    }

    public bool hasVertex(DelaunayVertex2D v)
    {
        return p1 == v || p2 == v || p3 == v;
    }

    public bool hasEdge(Edge e)
    {
        if (edges[0].Equals(e) || edges[1].Equals(e) ||edges[2].Equals(e)) return true;
        return false;
    }

    //Replace edge. e and edges[i] are equal but have some mismatching properties
    public bool replaceEdge(Edge e)
    {
        for(int i = 0; i < 3; i++)
        {
            if (edges[i].Equals(e))
            {
                edges[i] = e;
                return true;
            }
        }
        return false;
    }

    public bool Equals(Triangle other)
    {
        return
            p1 == other.p1 && (p2 == other.p2 && p3 == other.p3 || p3 == other.p2 && p2 == other.p3) ||
            p2 == other.p1 && (p1 == other.p2 && p3 == other.p3 || p3 == other.p2 && p1 == other.p3) ||
            p3 == other.p1 && (p2 == other.p2 && p1 == other.p3 || p1 == other.p2 && p2 == other.p3);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        return Equals((Triangle)obj);
    }

    public override int GetHashCode()
    {
        return id;
    }

    public void DebugTriangle(Vector3 loc)
    {
        Debug.DrawLine((p1.loc + loc), (p2.loc + loc), Color.blue, 100f);
        Debug.DrawLine((p2.loc + loc), (p3.loc + loc), Color.blue, 100f);
        Debug.DrawLine((p3.loc + loc), (p1.loc + loc), Color.blue, 100f);
    }
}

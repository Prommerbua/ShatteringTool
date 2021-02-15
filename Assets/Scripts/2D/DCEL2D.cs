using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//source: https://searchcode.com/codesearch/view/49254163/

public class DCEL2D
{
    public List<Triangle> Triangles;
    public List<Vertex2D> Vertices;

    public DCEL2D()
    {
        Triangles = new List<Triangle>();
        Vertices = new List<Vertex2D>();
    }

    public void AddTriangle(ref Vertex2D v1, ref Vertex2D v2, ref Vertex2D v3)
    {
        bool v1Exists = false;
        bool v2Exists = false;
        bool v3Exists = false;
        foreach (var vertex in Vertices)
        {
            if (v1.Pos == vertex.Pos)
            {
                v1 = vertex;
                v1Exists = true;
            }

            if (v2.Pos == vertex.Pos)
            {
                v2 = vertex;
                v2Exists = true;
            }

            if (v3.Pos == vertex.Pos)
            {
                v3 = vertex;
                v3Exists = true;
            }
        }

        //4 cases: 2 vertices of triangle already exist(3cases depends on which vertices) or it's the first triangle

        var e1 = new Edge2D(v1); //A->B
        var e11 = new Edge2D(v1); //A->C
        var e2 = new Edge2D(v2); //B->C
        var e22 = new Edge2D(v2); //B->A
        var e3 = new Edge2D(v3); //C->A
        var e33 = new Edge2D(v3); //C->B

        if(!v1Exists) v1.IncEdge = e1;
        if(!v2Exists) v2.IncEdge = e2;
        if(!v3Exists) v3.IncEdge = e3;

        var t = new Triangle(e1);

        e1.left = t;
        e1.next = e2;
        e1.prev = e3;
        e1.twin = e22;

        e11.left = null;
        e11.next = e33;
        e11.prev = e22;
        e11.twin = e3;

        e2.left = t;
        e2.next = e3;
        e2.prev = e1;
        e2.twin = e33;

        e22.left = null;
        e22.next = e11;
        e22.prev = e33;
        e22.twin = e1;

        e3.left = t;
        e3.next = e1;
        e3.prev = e2;
        e3.twin = e11;

        e33.left = null;
        e33.next = e22;
        e33.prev = e11;
        e33.twin = e2;

        Triangles.Add(t);

        if(!v1Exists) Vertices.Add(v1);
        if(!v2Exists) Vertices.Add(v2);
        if(!v3Exists) Vertices.Add(v3);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//source: https://searchcode.com/codesearch/view/49254163/

public class DCEL2D
{
    public List<Face> Faces;
    public List<Vertex2D> Vertices;
    public List<Edge2D> Edges;


    public DCEL2D(List<Triangle> triangles)
    {
        Faces = new List<Face>();
        Vertices = new List<Vertex2D>();
        Edges = new List<Edge2D>();

        InitializeDCEL(triangles);
    }

    private void InitializeDCEL(List<Triangle> triangles)
    {
        foreach (var triangle in triangles)
        {

        }
    }


    public void AddTriangle(Vertex2D v1, Vertex2D v2, Vertex2D v3)
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
        if (v1Exists && v2Exists)
        {
            var e1 = new Edge2D(v1); //A->C
            var e2 = new Edge2D(v2); //B->C
            var e3 = new Edge2D(v3); //C->A
            var e33 = new Edge2D(v3); //C->B

            v3.IncEdge = e3;

            var t = new Face(e3);
            //if (v1.IncEdge.twin.left == null) v1.IncEdge.twin.left = t;

            e1.left = null;
            e1.next = e33;
            e1.prev = v1.IncEdge.twin;
            e1.twin = e3;

            e2.left = t;
            e2.next = e3;
            e2.prev = v1.IncEdge;
            e2.twin = e33;

            e3.left = t;
            e3.next = v1.IncEdge;
            e3.prev = e2;
            e3.twin = e1;

            e33.left = null;
            e33.next = v1.IncEdge.twin;
            e33.prev = e1;
            e33.twin = e2;

            v1.IncEdge.next = e2;
            v1.IncEdge.left = t;
            v1.IncEdge.prev = e3;

            v2.IncEdge = e2;
            v2.IncEdge.left = t;
            v2.IncEdge.prev = v1.IncEdge;

            Faces.Add(t);
            Vertices.Add(v3);
        }
        // else if (v1Exists && v3Exists)
        // {
        //     var e1 = new Edge2D(v1); //A->B
        //     var e2 = new Edge2D(v2); //B->C
        //     var e3 = new Edge2D(v2); //B->A
        //     var e33 = new Edge2D(v3); //C->B
        //
        //     v2.IncEdge = e2;
        //
        //     var t = new Triangle(e2);
        //
        //     e1.left = t;
        //     e1.next = e2;
        //     e1.prev = v3.IncEdge;
        //     e1.twin = e3;
        //
        //     e2.left = t;
        //     e2.next = e1.prev;
        //     e2.prev = e1;
        //     e2.twin = e33;
        //
        //     e3.left = null;
        //     e3.next = e1.prev.twin;
        //     e3.prev = e33;
        //     e3.twin = e1;
        //
        //     e33.left = null;
        //     e33.next = e3;
        //     e33.prev = e3.next;
        //     e33.twin = e2;
        //
        //     Triangles.Add(t);
        //     Vertices.Add(v2);
        // }
        // else if (v2Exists && v3Exists)
        // {
        //     var e1 = new Edge2D(v1); //A->B
        //     var e2 = new Edge2D(v1); //A->C
        //     var e3 = new Edge2D(v2); //B->A
        //     var e33 = new Edge2D(v3); //C->A
        //
        //     v1.IncEdge = e1;
        //
        //     var t = new Triangle(e1);
        //
        //     e1.left = t;
        //     e1.next = v2.IncEdge;
        //     e1.prev = e33;
        //     e1.twin = e3;
        //
        //     e2.left = null;
        //     e2.next = v2.IncEdge.twin;
        //     e2.prev = e1.next;
        //     e2.twin = e33;
        //
        //     e3.left = null;
        //     e3.next = e2;
        //     e3.prev = v2.IncEdge.twin;
        //     e3.twin = e1;
        //
        //     e33.left = t;
        //     e33.next = e1;
        //     e33.prev = e1.next;
        //     e33.twin = e2;
        //
        //     Triangles.Add(t);
        //     Vertices.Add(v1);
        // }
        else
        {
            var e1 = new Edge2D(v1); //A->B
            var e11 = new Edge2D(v1); //A->C
            var e2 = new Edge2D(v2); //B->C
            var e22 = new Edge2D(v2); //B->A
            var e3 = new Edge2D(v3); //C->A
            var e33 = new Edge2D(v3); //C->B

            v1.IncEdge = e1;
            v2.IncEdge = e2;
            v3.IncEdge = e3;

            var t = new Face(e1);

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

            Faces.Add(t);
            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);
        }
    }
}



using System.Collections.Generic;
using UnityEngine;

public class BowyerWatson : MonoBehaviour
{
    public float width;
    public float height;
    public int pointCount;
    private List<Vector2> points;
    private List<Triangle> triangulation;

    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector2>();
        for (int i = 0; i < pointCount; i++)
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);
            points.Add(new Vector2(x, y));
        }

        DelaunayTriangulation();
    }

    private void DelaunayTriangulation()
    {
        triangulation = new List<Triangle>();

        //Create a supertriangle which has all points in it
        Vertex2D v1 = new Vertex2D(-1, -1);
        Vertex2D v2 = new Vertex2D((int) (width * 2 + 1), -1);
        Vertex2D v3 = new Vertex2D(-1, (int) (height * 2 + 1));

        var e1 = new Edge2D(v1);  //A->B
        var e11 = new Edge2D(v1); //A->C
        var e2 = new Edge2D(v2);  //B->C
        var e22 = new Edge2D(v2); //B->A
        var e3 = new Edge2D(v3);  //C->A
        var e33 = new Edge2D(v3); //C->B

        var t = new Triangle(e1);

        e1.left = t;
        e1.next = e2;
        e1.prev = e3;
        e1.twin = e11;

        e11.left = null;
        e11.next = e33;
        e11.prev = e22;
        e11.twin = e1;

        e2.left = t;
        e2.next = e3;
        e2.prev = e1;
        e2.twin = e22;

        e22.left = null;
        e22.next = e11;
        e22.prev = e33;
        e22.twin = e2;

        e3.left = t;
        e3.next = e1;
        e3.prev = e2;
        e3.twin = e33;

        e33.left = null;
        e33.next = e22;
        e33.prev = e11;
        e33.twin = e3;

        triangulation.Add(t);


        foreach (var point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangulation)
            {
                //Check if point is inside circumcircle of triangle
                //if yes add triangle to badtriangles

                //empty polygon set (edges?)

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (points != null)
        {
            foreach (var point in points)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }

        Gizmos.color = Color.white;
        if (triangulation != null)
        {
            foreach (var triangle in triangulation)
            {
                // Gizmos.DrawLine(new Vector2(triangle.vertices[0].x, triangle.vertices[0].y), new Vector2(triangle.vertices[1].x, triangle.vertices[1].y));
                // Gizmos.DrawLine(new Vector2(triangle.vertices[1].x, triangle.vertices[1].y), new Vector2(triangle.vertices[2].x, triangle.vertices[2].y));
                // Gizmos.DrawLine(new Vector2(triangle.vertices[2].x, triangle.vertices[2].y), new Vector2(triangle.vertices[0].x, triangle.vertices[0].y));

                Gizmos.DrawLine(new Vector2(triangle.incEdge.org.x, triangle.incEdge.org.y), new Vector2(triangle.incEdge.next.org.x, triangle.incEdge.next.org.y));
                Gizmos.DrawLine(new Vector2(triangle.incEdge.next.org.x, triangle.incEdge.next.org.y), new Vector2(triangle.incEdge.prev.org.x, triangle.incEdge.prev.org.y));
                Gizmos.DrawLine(new Vector2(triangle.incEdge.prev.org.x, triangle.incEdge.prev.org.y), new Vector2(triangle.incEdge.org.x, triangle.incEdge.org.y));


            }
        }



        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0,0), new Vector2(0, height));
        Gizmos.DrawLine(new Vector2(0,0), new Vector2(width, 0));
        Gizmos.DrawLine(new Vector2(width,0), new Vector2(width, height));
        Gizmos.DrawLine(new Vector2(0,height), new Vector2(width, height));
    }
}



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
        Triangle superTriangle = new Triangle();
        Vertex2D v1 = new Vertex2D(-1, -1);
        Vertex2D v2 = new Vertex2D((int) (width * 2 + 1), -1);
        Vertex2D v3 = new Vertex2D(-1, (int) (height * 2 + 1));

        superTriangle.vertices = new List<Vertex2D>();
        superTriangle.vertices.Add(v1);
        superTriangle.vertices.Add(v2);
        superTriangle.vertices.Add(v3);

        triangulation.Add(superTriangle);


        foreach (var point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangulation)
            {
                //Check if point is inside circumcircle of triangle
                //if yes add triangle to badtriangles
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
                Gizmos.DrawLine(new Vector2(triangle.vertices[0].x, triangle.vertices[0].y), new Vector2(triangle.vertices[1].x, triangle.vertices[1].y));
                Gizmos.DrawLine(new Vector2(triangle.vertices[1].x, triangle.vertices[1].y), new Vector2(triangle.vertices[2].x, triangle.vertices[2].y));
                Gizmos.DrawLine(new Vector2(triangle.vertices[2].x, triangle.vertices[2].y), new Vector2(triangle.vertices[0].x, triangle.vertices[0].y));
            }
        }



        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0,0), new Vector2(0, height));
        Gizmos.DrawLine(new Vector2(0,0), new Vector2(width, 0));
        Gizmos.DrawLine(new Vector2(width,0), new Vector2(width, height));
        Gizmos.DrawLine(new Vector2(0,height), new Vector2(width, height));
    }
}

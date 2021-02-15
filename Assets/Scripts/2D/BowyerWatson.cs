using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BowyerWatson : MonoBehaviour
{
    public float width;
    public float height;
    public int pointCount;
    private List<Vector2> _points;
    private DCEL2D _dcel;

    // Start is called before the first frame update
    void Start()
    {
        _points = new List<Vector2>();
        _dcel = new DCEL2D();
        for (int i = 0; i < pointCount; i++)
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);
            _points.Add(new Vector2(x, y));
        }

        DelaunayTriangulation();
    }

    private void DelaunayTriangulation()
    {
        //Create a supertriangle which has all points in it
        Vertex2D v1 = new Vertex2D(-1, -1);
        Vertex2D v2 = new Vertex2D((int) (width * 2 + 1), -1);
        Vertex2D v3 = new Vertex2D(-1, (int) (height * 2 + 1));

        _dcel.AddTriangle(ref v1, ref v2, ref v3);


        foreach (var point in _points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in _dcel.Triangles)
            {
                triangle.CalculateCircumcirclePosition();
                if (triangle.IsPointInsideCircumcircle(point))
                {
                    badTriangles.Add(triangle);
                }
            }

            List<Edge2D> polygon = new List<Edge2D>();
            for (var i = 0; i < badTriangles.Count; i++)
            {
                var edges = badTriangles[i].GetEdges();
                for (var j = 0; j < edges.Count; j++)
                {
                    var edge = edges[j];
                    if (edge.twin.left == null) polygon.Add(edge);
                    else
                    {
                        Debug.Log("Edge Shared");
                    }
                }
            }

            foreach (var badTriangle in badTriangles)
            {
                _dcel.Triangles.Remove(badTriangle);
            }

            foreach (var edge2D in polygon)
            {
                Vertex2D newTriV1 = edge2D.org;
                Vertex2D newTriV2 = edge2D.twin.org;
                Vertex2D newTriV3 = new Vertex2D(point);


                _dcel.AddTriangle(ref newTriV1, ref newTriV2, ref newTriV3);
            }
        }

        // foreach (var triangle in _dcel.Triangles.ToList())
        // {
        //     foreach (var vertex in triangle.GetVertices())
        //     {
        //         if (vertex == v1 || vertex == v2 || vertex == v3)
        //         {
        //             _dcel.Triangles.Remove(triangle);
        //         }
        //     }
        //
        // }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_points != null)
        {
            foreach (var point in _points)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }

        Gizmos.color = Color.white;
        if (_dcel != null)
        {
            foreach (var triangle in _dcel.Triangles)
            {
                // Gizmos.DrawLine(new Vector2(triangle.vertices[0].x, triangle.vertices[0].y), new Vector2(triangle.vertices[1].x, triangle.vertices[1].y));
                // Gizmos.DrawLine(new Vector2(triangle.vertices[1].x, triangle.vertices[1].y), new Vector2(triangle.vertices[2].x, triangle.vertices[2].y));
                // Gizmos.DrawLine(new Vector2(triangle.vertices[2].x, triangle.vertices[2].y), new Vector2(triangle.vertices[0].x, triangle.vertices[0].y));

                Gizmos.DrawLine(new Vector2(triangle.incEdge.org.Pos.x, triangle.incEdge.org.Pos.y),
                    new Vector2(triangle.incEdge.next.org.Pos.x, triangle.incEdge.next.org.Pos.y));
                Gizmos.DrawLine(new Vector2(triangle.incEdge.next.org.Pos.x, triangle.incEdge.next.org.Pos.y),
                    new Vector2(triangle.incEdge.prev.org.Pos.x, triangle.incEdge.prev.org.Pos.y));
                Gizmos.DrawLine(new Vector2(triangle.incEdge.prev.org.Pos.x, triangle.incEdge.prev.org.Pos.y),
                    new Vector2(triangle.incEdge.org.Pos.x, triangle.incEdge.org.Pos.y));


                //Gizmos.DrawSphere(triangle.CalculateCircumcirclePosition(), 2f);
                //Gizmos.color = Color.white;
                //Gizmos.DrawWireSphere(triangle.CalculateCircumcirclePosition(), (triangle.incEdge.org.Pos - triangle.CalculateCircumcirclePosition()).magnitude);
            }
        }


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, height));
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(width, 0));
        Gizmos.DrawLine(new Vector2(width, 0), new Vector2(width, height));
        Gizmos.DrawLine(new Vector2(0, height), new Vector2(width, height));
    }
}

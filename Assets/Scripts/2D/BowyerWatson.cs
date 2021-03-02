using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BowyerWatson : MonoBehaviour
{
    public float width;
    public float height;
    public int pointCount;
    private List<Vector2> _points;
    private List<Triangle> _triangulation;

    private DCEL2D _dcel;

    // Start is called before the first frame update
    void Start()
    {
        _points = new List<Vector2>();
        _triangulation = new List<Triangle>();
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
        Vector2 v1 = new Vector2(-1, -1);
        Vector2 v2 = new Vector2((int) (width * 2 + 1), -1);
        Vector2 v3 = new Vector2(-1, (int) (height * 2 + 1));

        var t = new Triangle(v1,v2,v3);
        _triangulation.Add(t);

        foreach (var point in _points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in _triangulation)
            {
                triangle.CalculateCircumcirclePosition();
                if (triangle.IsPointInsideCircumcircle(point))
                {
                    badTriangles.Add(triangle);
                }
            }

            List<Edge> polygon = new List<Edge>();
            for (var i = 0; i < badTriangles.Count; i++)
            {
                var edges = badTriangles[i].GetEdges();
                for (var j = 0; j < edges.Count; j++)
                {
                    var edge = edges[j];
                    bool shared = false;
                    foreach (var badTriangle in badTriangles)
                    {
                        if (badTriangles[i] == badTriangle) continue;
                        if (edge == badTriangle.E1 || edge == badTriangle.E2 || edge == badTriangle.E3)
                        {
                            shared = true;
                            break;
                        }
                    }
                    //Check if edge is shared
                    //if (edge.twin.left != null && edge.left != null) Debug.Log("Edge Shared");
                    if(shared) Debug.Log("Edge Shared");
                    else
                    {
                        polygon.Add(edge);
                    }
                }
            }

            foreach (var badTriangle in badTriangles)
            {
                //_dcel.Triangles.Remove(badTriangle);
                _triangulation.Remove(badTriangle);
            }

            foreach (var edge2D in polygon)
            {
                //Debug.Log(newTriV1.Pos + ", " + newTriV2.Pos + ", " + newTriV3.Pos);
                _triangulation.Add(new Triangle(edge2D.Start, edge2D.End, point));
            }
        }

        foreach (var triangle in _triangulation.ToList())
        {
            foreach (var vertex in triangle.GetVertices())
            {
                if (vertex == v1 || vertex == v2 || vertex == v3)
                {
                    _triangulation.Remove(triangle);
                }
            }

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(new Vector2(-1, -1), 1f);
        Gizmos.DrawSphere(new Vector2((int) (width * 2 + 1), -1), 1f);
        Gizmos.DrawSphere(new Vector2(-1, (int) (height * 2 + 1)), 1f);

        if (_points != null)
        {
            foreach (var point in _points)
            {
                Gizmos.DrawSphere(point, 0.8f);
            }
        }

        Gizmos.color = Color.white;
        if (_dcel != null)
        {
            int count = 0;
            foreach (var triangle in _triangulation)
            {

                Gizmos.color = Color.white;

                var vertices = triangle.GetVertices();
                // Gizmos.DrawLine(new Vector2(triangle.incEdge.org.Pos.x, triangle.incEdge.org.Pos.y),
                //     new Vector2(triangle.incEdge.twin.org.Pos.x, triangle.incEdge.twin.org.Pos.y));
                // Gizmos.DrawLine(new Vector2(triangle.incEdge.next.org.Pos.x, triangle.incEdge.next.org.Pos.y),
                //     new Vector2(triangle.incEdge.next.twin.org.Pos.x, triangle.incEdge.next.twin.org.Pos.y));
                // Gizmos.DrawLine(new Vector2(triangle.incEdge.prev.org.Pos.x, triangle.incEdge.prev.org.Pos.y),
                //     new Vector2(triangle.incEdge.prev.twin.org.Pos.x, triangle.incEdge.prev.twin.org.Pos.y));

                Gizmos.DrawLine(vertices[0], vertices[1]);
                Gizmos.DrawLine(vertices[1], vertices[2]);
                Gizmos.DrawLine(vertices[2], vertices[0]);


                GUIStyle g = new GUIStyle();
                g.normal.textColor = Color.white;
                g.fontSize = 30;

                //Handles.Label(triangle.CalculateCentroidPosition(), count.ToString(), g);

                count++;

                Gizmos.color = Color.blue;
                //Gizmos.DrawWireSphere(triangle.CalculateCircumcirclePosition(), triangle.CircumcircleRadius);
            }
        }


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, height));
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(width, 0));
        Gizmos.DrawLine(new Vector2(width, 0), new Vector2(width, height));
        Gizmos.DrawLine(new Vector2(0, height), new Vector2(width, height));
    }
}



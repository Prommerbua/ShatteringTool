using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Visualization2D
{


    public class BowyerWatson2D : MonoBehaviour
    {
        public float width;
        public float height;
        public int pointCount;
        private List<Vector2> _points;
        private List<Triangle> _triangulation;
        Dictionary<Vector2, List<Triangle>> _vertexTriangleMap;
        private List<int> _outIndices;
        private List<Vector3> _outVertices;




        [SerializeField] private float pointRadius = 0.5f;
        [SerializeField] private Color pointColor = Color.red;
        [SerializeField] private Color delaunayLineColor = Color.white;
        [SerializeField] private bool drawTriangleNumbers = false;
        [SerializeField] private bool drawDelaunay = true;
        [SerializeField] private bool drawVoronoi;
        [SerializeField] private bool drawCircumcenter;


        // Start is called before the first frame update
        void Start()
        {
            _points = new List<Vector2>();
            _triangulation = new List<Triangle>();
            for (int i = 0; i < pointCount; i++)
            {
                var x = Random.Range(0, width);
                var y = Random.Range(0, height);
                _points.Add(new Vector2(x, y));
            }

            // _points.Add(new Vector2(15, 10));
            // _points.Add(new Vector2(30, 40));
            // _points.Add(new Vector2(40, 25));
            // _points.Add(new Vector2(43, 30));
            // _points.Add(new Vector2(50, 27));
            // _points.Add(new Vector2(60, 5));
            // _points.Add(new Vector2(70, 40));

            DelaunayTriangulation();

            // Polygon poly = new Polygon();
            //
            // for (int i = 0; i < pointCount; i++)
            // {
            //     poly.Add(new Vertex(_points[i].x, _points[i].y));
            //
            //     if (i == pointCount - 1)
            //     {
            //         poly.Add(new Segment(new Vertex(_points[i].x, _points[i].y), new Vertex(_points[0].x, _points[0].y)));
            //     }
            //     else
            //     {
            //         poly.Add(new Segment(new Vertex(_points[i].x, _points[i].y),
            //             new Vertex(_points[i + 1].x, _points[i + 1].y)));
            //     }
            // }
            //
            // var _mesh = poly.Triangulate();
            //
            // foreach (var t in _mesh.Triangles)
            // {
            //     for (int i = 0; i < t.vertices.Length; i++)
            //     {
            //
            //     }
            // }
        }


        private void DelaunayTriangulation()
        {
            //Create a supertriangle which has all points in it
            Vector2 v1 = new Vector2(-1, -1);
            Vector2 v2 = new Vector2((int) (width * 2 + 1), -1);
            Vector2 v3 = new Vector2(-1, (int) (height * 2 + 1));

            var t = new Triangle(v1, v2, v3);
            _triangulation.Add(t);

            foreach (var point in _points)
            {
                List<Triangle> badTriangles = new List<Triangle>();

                foreach (var triangle in _triangulation)
                {
                    triangle.CalculateCircumcenter();
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
                        if (!shared) polygon.Add(edge);
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



            _vertexTriangleMap = new Dictionary<Vector2, List<Triangle>>();
            foreach (var triangle in _triangulation)
            {
                foreach (var vertex in triangle.GetVertices())
                {
                    if (_vertexTriangleMap.ContainsKey(vertex))
                    {
                        _vertexTriangleMap[vertex].Add(triangle);
                    }
                    else
                    {
                        _vertexTriangleMap.Add(vertex, new List<Triangle> {triangle});
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = pointColor;

            if (_points != null)
            {
                foreach (var point in _points)
                {
                    Gizmos.DrawSphere(point, pointRadius);
                }
            }

            if (_triangulation != null)
            {
                if (drawDelaunay)
                {
                    int count = 0;
                    foreach (var triangle in _triangulation)
                    {
                        Gizmos.color = delaunayLineColor;

                        var vertices = triangle.GetVertices();
                        Gizmos.DrawLine(vertices[0], vertices[1]);
                        Gizmos.DrawLine(vertices[1], vertices[2]);
                        Gizmos.DrawLine(vertices[2], vertices[0]);


                        GUIStyle g = new GUIStyle();
                        g.normal.textColor = Color.white;
                        g.fontSize = 30;

                        if (drawTriangleNumbers)
                        {
                            Handles.Label(triangle.CalculateCentroidPosition(), count.ToString(), g);
                        }

                        if (drawCircumcenter)
                        {
                            Gizmos.DrawSphere(triangle.CalculateCircumcenter(), 0.6f);
                        }

                        count++;

                        //Gizmos.DrawWireSphere(triangle.CalculateCircumcirclePosition(), triangle.CircumcircleRadius);
                    }
                }
            }

            if (_vertexTriangleMap != null)
            {
                if (drawVoronoi)
                {
                    Gizmos.color = Color.cyan;
                    foreach (var triangle in _triangulation)
                    {
                        foreach (var triangle2 in _triangulation)
                        {
                            if (triangle.HasSharedEdge(triangle2))
                            {
                                Gizmos.DrawLine(triangle.CalculateCircumcenter(),
                                    triangle2.CalculateCircumcenter());
                            }
                        }
                    }
                }
            }


            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, height));
            Gizmos.DrawLine(new Vector2(0, 0), new Vector2(width, 0));
            Gizmos.DrawLine(new Vector2(width, 0), new Vector2(width, height));
            Gizmos.DrawLine(new Vector2(0, height), new Vector2(width, height));
        }
    }
}

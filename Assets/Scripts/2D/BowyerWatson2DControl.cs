using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BowyerWatson2DControl : MonoBehaviour
{
    [SerializeField] private float pointRadius = 0.5f;
    [SerializeField] private Color pointColor = Color.red;
    [SerializeField] private Color delaunayLineColor = Color.white;
    [SerializeField] private bool drawTriangleNumbers = false;
    [SerializeField] private bool drawDelaunay = true;
    [SerializeField] private bool drawVoronoi;
    [SerializeField] private bool drawCircumcenter;

    public float width;
    public float height;
    public int pointCount;

    private BowyerWatson2D bw;

    private List<Vector3> _points;

    private Plane plane;

    // Start is called before the first frame update
    void Start()
    {
        _points = new List<Vector3>();
        for (int i = 0; i < pointCount; i++)
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);

            // var x = RandomGaussian(0, width);
            // var y = RandomGaussian(0, height);
            _points.Add(new Vector3(x, y, 0));


            //Add Control Points
            // _points.Add(new Vector3(2 * width - x, y));
            // _points.Add(new Vector3(-2 * width - x, y));
            // _points.Add(new Vector3(x, 2 * height -y));
            // _points.Add(new Vector3(x, -2 * height - y));
        }

        // _points.Add(new Vector3(-7.8f, -1.5f, -7.7f));
        // _points.Add(new Vector3(-1.2f, 1.0f, -0.8f));
        // _points.Add(new Vector3(-0.2f, 1.1f, -0.4f));


        //plane = new Plane(_points[0], _points[1], _points[2]);
        plane = new Plane(Vector3.back, Vector3.zero);
        bw = new BowyerWatson2D(_points, plane);
    }

    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }


    private void OnDrawGizmos()
    {
        if (bw == null) return;

        Gizmos.color = pointColor;

        //if (bw.Points == null || bw.Points.Count != pointCount) return;
        // if (bw.RotatedPoints == null || bw.RotatedPoints.Count != pointCount) return;
        // if (bw.RevertedPoints == null || bw.RevertedPoints.Count != pointCount) return;

        foreach (var point in bw.Points)
        {
            Gizmos.DrawSphere(point, pointRadius);
            Gizmos.DrawRay(point, plane.normal * 10);
        }

        //Gizmos.color = Color.blue;
        // foreach (var point in bw.RotatedPoints)
        // {
        //     Gizmos.DrawSphere(point, pointRadius);
        //     Gizmos.DrawRay(point, Vector3.back * 10);
        // }

        // Gizmos.color = Color.magenta;
        // foreach (var point in bw.RevertedPoints)
        // {
        //     Gizmos.DrawSphere(point, pointRadius - 0.1f);
        //     Gizmos.DrawRay(point, plane.normal * 10);
        // }

        Gizmos.color = Color.magenta;
        if (drawVoronoi)
        {
            foreach (var voronoiFace in bw.voronoiFaces)
            {
                for (int i = 0; i < voronoiFace.points.Count; i++)
                {
                    var p1 = voronoiFace.points[i];
                    Vector3 p2 = Vector3.zero;
                    if (i + 1 < voronoiFace.points.Count) p2 = voronoiFace.points[i + 1];
                    else p2 = voronoiFace.points[0];

                    Gizmos.DrawLine(p1, p2);
                }
            }
        }



        // if (bw.RotatedTriangulation != null)
        // {
        //     Gizmos.color = Color.cyan;
        //     foreach (var triangle in bw.RotatedTriangulation)
        //     {
        //
        //         Gizmos.DrawSphere(triangle.V1, pointRadius - 0.2f);
        //         Gizmos.DrawSphere(triangle.V2, pointRadius - 0.2f);
        //         Gizmos.DrawSphere(triangle.V3, pointRadius - 0.2f);
        //         Gizmos.DrawLine(triangle.V1, triangle.V2);
        //         Gizmos.DrawLine(triangle.V2, triangle.V3);
        //         Gizmos.DrawLine(triangle.V3, triangle.V1);
        //     }
        // }


        if (bw.Triangulation != null)
        {
            if (drawDelaunay)
            {
                int count = 0;
                foreach (var triangle in bw.Triangulation)
                {
                    Gizmos.color = delaunayLineColor;
                    foreach (var vector3 in _points.Except(triangle.GetVertices()))
                    {
                        if (triangle.IsPointInsideCircumcircle(vector3))
                        {
                            Gizmos.color = Color.blue;
                        }
                    }


                    Gizmos.DrawLine(triangle.V1, triangle.V2);
                    Gizmos.DrawLine(triangle.V2, triangle.V3);
                    Gizmos.DrawLine(triangle.V3, triangle.V1);


                    GUIStyle g = new GUIStyle();
                    g.normal.textColor = Color.white;
                    g.fontSize = 30;

                    if (drawTriangleNumbers)
                    {
                        Handles.Label(triangle.CalculateCentroidPosition(), count.ToString(), g);
                    }

                    if (drawCircumcenter)
                    {
                        Gizmos.DrawSphere(triangle.Circumcenter, 0.6f);
                        Gizmos.DrawWireSphere(triangle.Circumcenter, triangle.Radius);
                    }

                    count++;

                    //Gizmos.DrawWireSphere(triangle.CalculateCircumcirclePosition(), triangle.CircumcircleRadius);
                }
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, height));
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(width, 0));
        Gizmos.DrawLine(new Vector2(width, 0), new Vector2(width, height));
        Gizmos.DrawLine(new Vector2(0, height), new Vector2(width, height));
    }

    void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;

        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(corner0, corner2);
        Gizmos.DrawLine(corner1, corner3);
        Gizmos.DrawLine(corner0, corner1);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner0);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(position, normal);
    }
}

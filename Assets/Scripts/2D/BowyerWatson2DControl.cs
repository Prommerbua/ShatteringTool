using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        var _points = new List<Vector3>();
        for (int i = 0; i < pointCount; i++)
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);
            _points.Add(new Vector3(x, y, 0));
        }

        bw = new BowyerWatson2D(_points);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDrawGizmos()
    {
        if (bw == null) return;

        Gizmos.color = pointColor;

        if (bw.Points == null  || bw.Points.Count != pointCount) return;

        foreach (var point in bw.Points)
        {
            Gizmos.DrawSphere(point, pointRadius);
        }

        if (bw.Triangulation != null)
        {
            if (drawDelaunay)
            {
                int count = 0;
                foreach (var triangle in bw.Triangulation)
                {
                    Gizmos.color = delaunayLineColor;

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
                        Gizmos.DrawSphere(triangle.CalculateCircumcenter(), 0.6f);
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
}

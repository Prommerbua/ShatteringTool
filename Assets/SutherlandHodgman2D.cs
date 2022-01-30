using System.Collections.Generic;
using System.Linq;
using Habrador_Computational_Geometry;
using UnityEditor;
using UnityEngine;

public class SutherlandHodgman2D : MonoBehaviour
{
    public float width;
    public float height;

    private List<Vector3> subjectPolygon = new List<Vector3>();
    private List<Edge> clipPolygon = new List<Edge>();

    List<Vector3> outputList = new List<Vector3>();

    Vector3 intersection;

    // Start is called before the first frame update
    void Start()
    {
        subjectPolygon.Add(new Vector3(10, 10));
        subjectPolygon.Add(new Vector3(-7, 15));
        subjectPolygon.Add(new Vector3(-10, -6));
        subjectPolygon.Add(new Vector3(8, -5));


        clipPolygon.Add(new Edge(new Vector3(0, 0), new Vector3(0, height)));
        clipPolygon.Add(new Edge(new Vector3(0, height), new Vector3(width, height)));
        clipPolygon.Add(new Edge(new Vector3(width, height), new Vector3(width, 0)));
        clipPolygon.Add(new Edge(new Vector3(width, 0), new Vector3(0, 0)));



        var tmp = IsInside(new Vector3(0,0), new Vector3(0,10), new Vector3(1, 5));


        ClipPolygon();
    }

    private void ClipPolygon()
    {
        outputList = subjectPolygon.ToList();
        foreach (var clipEdge in clipPolygon)
        {
            List<Vector3> inputList = outputList.ToList();
            outputList.Clear();


            for (int i = 0; i < inputList.Count; i++)
            {
                Vector3 currentPoint = inputList[i];
                var index = i - 1;
                if (index < 0) index = inputList.Count - 1;
                Vector3 prevPoint = inputList[index];

                var isCurrentInside = IsInside(clipEdge.Start, clipEdge.End, currentPoint);
                var isPrevInside = IsInside(clipEdge.Start, clipEdge.End, prevPoint);


                Vector3 intersection = Vector3.zero;
                var intersect = ComputeIntersection(out intersection, prevPoint, currentPoint, clipEdge);

                if (!isCurrentInside)
                {
                    if (isPrevInside)
                    {
                        outputList.Add(intersection);
                    }
                    outputList.Add(currentPoint);
                }
                else if (!isPrevInside)
                {
                    outputList.Add(intersection);
                }
            }
        }
    }

    private bool ComputeIntersection(out Vector3 intersection, Vector3 prevPoint, Vector3 point, Edge clipEdge)
    {
        var A1 = point.y - prevPoint.y;
        var B1 = prevPoint.x - point.x;

        var C1 = A1 * prevPoint.x + B1 * prevPoint.y;


        var A2 = clipEdge.End.y - clipEdge.Start.y;
        var B2 = clipEdge.Start.x - clipEdge.End.x;

        var C2 = A2 * clipEdge.Start.x + B2 * clipEdge.Start.y;

        var det = A1 * B2 - A2 * B1;

        if (det == 0)
        {
            Debug.Log("Parallel");
            intersection = Vector3.zero;
            return false;
        }

        var x = (B2 * C1 - B1 * C2) / det;
        var y = (A1 * C2 - A2 * C1) / det;
        intersection = new Vector3(x, y);
        return true;
    }

    public static bool IsInside(Vector3 start,Vector3 end, Vector3 point)
    {
        return ((end.x - start.x)*(point.y - start.y) - (end.y - start.y)*(point.x - start.x)) > 0;
    }

    private void OnDrawGizmos()
    {
        if (subjectPolygon.Count != 0)
        {
            for (var index = 0; index < subjectPolygon.Count; index++)
            {
                var vector3 = subjectPolygon[index];
                Vector3 next = subjectPolygon[(index + 1) % subjectPolygon.Count];

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(vector3, 1.0f);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(vector3, next);
            }
        }


        Gizmos.color = Color.red;
        if (outputList.Count != 0)
        {
            foreach (var vector3 in outputList)
            {
                Gizmos.DrawSphere(vector3, 1.0f);
            }
        }


        Gizmos.color = Color.green;
        if (intersection != Vector3.zero)
        {
            Gizmos.DrawSphere(intersection, 0.5f);
        }


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, height));
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(width, 0));
        Gizmos.DrawLine(new Vector2(width, 0), new Vector2(width, height));
        Gizmos.DrawLine(new Vector2(0, height), new Vector2(width, height));
    }
}

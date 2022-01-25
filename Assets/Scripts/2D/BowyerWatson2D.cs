using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Visualization2D;


public class BowyerWatson2D
{
    public List<Vector3> Points;
    public List<Triangle> Triangulation;

    private Quaternion rotationQuat;

    private Triangle _superTri;

    public BowyerWatson2D(List<Vector3> points)
    {
        Points = points;

        CreateRotationMatrix();


        ComputeTriangulation();
    }

    private void CreateRotationMatrix()
    {
        Plane p = new Plane(Points[0], Points[1], Points[2]);

        Vector3 n



    }

    public void ComputeTriangulation()
    {
        Triangulation = new List<Triangle>();

        //Create a supertriangle which has all points in it
        Vector2 v1 = new Vector2(-1, -1);
        Vector2 v2 = new Vector2((int) (3000 * 2 + 1), -1);
        Vector2 v3 = new Vector2(-1, (int) (3000 * 2 + 1));

        _superTri = new Triangle(v1, v2, v3);
        Triangulation.Add(_superTri);

        DelaunayTriangulation();
    }


    private void DelaunayTriangulation()
    {
        List<Triangle> badTriangles = new List<Triangle>();
        List<Edge> polygon = new List<Edge>();
        List<Triangle> toRemove = new List<Triangle>();


        for (var index = 0; index < Points.Count; index++)
        {
            var point = Points[index];
            badTriangles.Clear();

            foreach (var triangle in Triangulation)
            {
                if (triangle.IsPointInsideCircumcircle(point))
                {
                    badTriangles.Add(triangle);
                }
            }

            polygon.Clear();

            foreach (var triangle in badTriangles)
            {
                foreach (var edge in triangle.GetEdges())
                {
                    if (badTriangles.Except(new[] {triangle}).SelectMany(t => t.GetEdges()).All(e => !edge.Equals(e)))
                    {
                        polygon.Add(edge);
                    }
                }
            }

            foreach (var badTriangle in badTriangles)
            {
                Triangulation.Remove(badTriangle);
            }

            foreach (var edge in polygon)
            {
                Triangulation.Add(new Triangle(point, edge.Start, edge.End));
            }
        }

        for (int index = 0; index < Triangulation.Count; index++)
        {
            var triangle = Triangulation[index];
            if (triangle.HasVertex(_superTri.V1) || triangle.HasVertex(_superTri.V2) || triangle.HasVertex(_superTri.V3))
            {
                toRemove.Add(triangle);
            }
        }

        Triangulation.RemoveAll(t => toRemove.Contains(t));
    }
}

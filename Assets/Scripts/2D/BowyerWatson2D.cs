using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Visualization2D;


public class BowyerWatson2D
{
    public List<Vector3> Points;
    public List<Vector3> RotatedPoints = new List<Vector3>();
    public List<Vector3> RevertedPoints = new List<Vector3>();


    public List<Triangle> Triangulation;
    public List<Triangle> RotatedTriangulation = new List<Triangle>();

    public List<VoronoiFace> voronoiFaces = new List<VoronoiFace>();


    private Plane p;

    private Quaternion rotationQuat;
    private Quaternion inverseQuat;

    private Triangle _superTri;

    public BowyerWatson2D(List<Vector3> points, Plane plane)
    {
        Points = points;
        p = plane;
        CreateRotationMatrix(plane);

        ComputeTriangulation();
    }

    private void CreateRotationMatrix(Plane p)
    {
        // double angle = Math.Acos(Vector3.Dot(p.normal, new Vector3(0, 0, 1)));
        //
        // var axis = Vector3.Cross(p.normal, new Vector3(0, 0, 1));
        //
        // double cosA = Vector3.Dot(axis, new Vector3(1, 0, 0)) / axis.magnitude;
        // double cosB = Vector3.Dot(axis, new Vector3(0, 1, 0)) / axis.magnitude;
        // double cosC = Vector3.Dot(axis, new Vector3(0, 0, 1)) / axis.magnitude;
        //
        // var w = Math.Cos(angle / 2);
        // var x = Math.Sin(angle / 2) * axis.x;
        // var y = Math.Sin(angle / 2) * axis.y;
        // var z = Math.Sin(angle / 2) * axis.z;
        // rotationQuat = new Quaternion((float) x, (float) y, (float) z, (float) w);


        rotationQuat = Quaternion.FromToRotation(p.normal, Vector3.back);
        inverseQuat = Quaternion.Inverse(rotationQuat);
    }

    public void ComputeTriangulation()
    {
        Triangulation = new List<Triangle>();

        //Create a supertriangle which has all points in it

        // Vector3 v1 = new Vector3(0, 1, 0) * 3000 + moved;
        // Vector3 v2 = Quaternion.AngleAxis((float) (2*Math.PI / 3), new Vector3(0,0,1)) * new Vector3(0,1,0) * 3000f + moved;
        // Vector3 v3 = Quaternion.AngleAxis((float) (-2*Math.PI / 3), new Vector3(0,0,1)) * new Vector3(0,1,0) * 3000f + moved;

        Vector3 v1 = new Vector3(-300, -300);
        Vector3 v2 = new Vector3(-300, 600);
        Vector3 v3 = new Vector3(900, -300);

        _superTri = new Triangle(v1, v2, v3);
        Triangulation.Add(_superTri);

        DelaunayTriangulation();
        CreateVoronoiCells();
    }

    private void CreateVoronoiCells()
    {
        // foreach (var point in Points)
        // {
        //     var triangles = Triangulation.Where(t => t.HasVertex(point));
        //     var circumcenters = new List<Vector3>();
        //
        //
        //     foreach (var triangle in triangles)
        //     {
        //         circumcenters.Add(triangle.Circumcenter);
        //     }
        //
        //     SortClockwise(ref circumcenters);
        //     voronoiFaces.Add(new VoronoiFace(circumcenters, new Plane(Vector3.back, Vector3.zero)));
        // }


        foreach (var point in Points)
        {
            var neighbors = Triangulation.Where(t => t.HasVertex(point));


            List<Vector3> neighborVertices = new List<Vector3>();
            foreach (var triangle in neighbors)
            {
                neighborVertices.Add(triangle.V1);
                neighborVertices.Add(triangle.V2);
                neighborVertices.Add(triangle.V3);
            }

            neighborVertices = neighborVertices.Distinct().ToList();
            neighborVertices.Remove(point);

            foreach (var vertex in neighborVertices)
            {
                var dir = (vertex - point).normalized;
                var mid = (vertex + point) / 2;

                Plane p = new Plane(dir, mid);

                List<Vector3> VoronoiVertices = new List<Vector3>();
                foreach (var triangle in neighbors)
                {
                    if (Math.Abs(p.GetDistanceToPoint(triangle.Circumcenter)) < 0.01)
                    {
                        VoronoiVertices.Add(triangle.Circumcenter);
                    }
                }

                var face = new VoronoiFace(VoronoiVertices.ToList(), p);
                voronoiFaces.Add(face);
            }
        }
    }

    private void SortClockwise(ref List<Vector3> circumcenters)
    {
        Vector3 center = Vector3.zero;
        foreach (var vector3 in circumcenters)
        {
            center += vector3;
        }

        center = center / circumcenters.Count;



        circumcenters = circumcenters.OrderBy(t => Math.Atan2(t.y - center.y, t.x - center.y)).ToList();
    }


    private void DelaunayTriangulation()
    {
        List<Triangle> badTriangles = new List<Triangle>();
        List<Edge> polygon = new List<Edge>();
        List<Triangle> toRemove = new List<Triangle>();

        for (var index = 0; index < Points.Count; index++)
        {
            Vector3 point = rotationQuat * Points[index];
            RotatedPoints.Add(point);
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
                Triangulation.Add(new Triangle(edge.Start, edge.End, point));
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

        for (var index = 0; index < Triangulation.Count; index++)
        {

            var triangle = Triangulation[index];
            RotatedTriangulation.Add(triangle);
            var vertices = triangle.GetVertices();
            Triangulation[index] = new Triangle(inverseQuat * vertices[0], inverseQuat * vertices[1], inverseQuat * vertices[2]);
        }

        foreach (var rotatedPoint in RotatedPoints)
        {
            RevertedPoints.Add(inverseQuat * rotatedPoint);
        }
    }
}

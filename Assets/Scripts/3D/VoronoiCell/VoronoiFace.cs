using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiFace {

    VoronoiCell parent;
    public List<Vector3> vertices;
    public Plane plane;

    GameObject go;
    public VoronoiFace(List<Vector3> vertices, Plane plane)
    {
        this.vertices = vertices;
        this.plane = plane;
    }

    //Build the triangulated mesh for the face
    public void CreateObject(List<Vector3> meshVertices, List<int> meshTriangles, List<Vector3> meshNormals)
    {
        Vector3 center = new Vector3();
        foreach(Vector3 v in vertices) { center += v; }
        BowyerWatson2D bw = new BowyerWatson2D(vertices, plane, center / vertices.Count);
        bw.MakeDiagram();

        foreach(Triangle t in bw.triangulation)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(t.p1.loc);
            points.Add(t.p2.loc);
            points.Add(t.p3.loc);
            if ((Vector3.Cross(points[1] - points[0], points[2] - points[0]).normalized + plane.normal).sqrMagnitude < 2)
            {
                points.Reverse();
            }
            meshVertices.AddRange(points);

            meshTriangles.Add(meshVertices.Count - 1);
            meshTriangles.Add(meshVertices.Count - 3);
            meshTriangles.Add(meshVertices.Count - 2);

            meshNormals.Add(plane.normal);
            meshNormals.Add(plane.normal);
            meshNormals.Add(plane.normal);
        }
    }

    public void DestroyObject()
    {
        GameObject.DestroyImmediate(go);
        go = null;
    }
}

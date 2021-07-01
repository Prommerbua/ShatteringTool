using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HullDelaunayVoronoi.Delaunay;
using HullDelaunayVoronoi.Primitives;
using UnityEngine;
using UnityEngine.Rendering;

public class TriangulatePoints3D : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private int numPoints;
    [SerializeField] private Material material;

    private DelaunayTriangulation3 delaunay;

    void Start()
    {
        Vertex3[] vertices = new Vertex3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            Vector3 point =
                _meshFilter.mesh.GetRandomPointInsideNonConvex(_meshFilter.GetComponent<Renderer>().bounds.center);
            vertices[i] = new Vertex3(point.x, point.y, point.z);
        }

        delaunay = new DelaunayTriangulation3();
        delaunay.Generate(vertices);


        GameObject go = new GameObject();
        var meshfilter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;

        Mesh mesh = new Mesh();
        Vector3[] meshVertices = new Vector3[numPoints];


        mesh.vertices = meshVertices;
        mesh.RecalculateNormals();

        meshfilter.mesh = mesh;


        go.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
}

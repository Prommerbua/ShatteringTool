using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiCell {

    DelaunayVertex seed;

    public  List<DelaunayVertex> neighbours;
    public List<VoronoiFace> faces;

    public GameObject go;

    public bool destroyed = false;

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<int> triangles;

    Mesh mesh;

    public VoronoiCell(DelaunayVertex seed, List<DelaunayVertex> neighbours, List<VoronoiFace> faces)
    {
        this.seed = seed;
        this.neighbours = neighbours;
        this.faces = faces;
    }

    //Create mesh and object for the voronoi cell
    public void CreateObject()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();

        foreach(VoronoiFace face in faces)
        {
            face.CreateObject(vertices, triangles, normals);
        }

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        go = new GameObject();
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshCollider>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        go.transform.position = seed.loc;
    }


    //Update the mos of voronoi cell
    public void UpdateObject(List<DelaunayVertex> neighbours, List<VoronoiFace> faces)
    {
        if (destroyed) return;
        this.neighbours = neighbours;
        this.faces = faces;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();

        foreach (VoronoiFace face in faces)
        {
            face.CreateObject(vertices, triangles, normals);
        }

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
    }


    //Update the neighbouring voronoi cells
    public void UpdateNeighbours()
    {
        foreach(DelaunayVertex dv in seed.getNeighbours())
        {
            if(dv.cell == null)
            {
                dv.CalculateVoronoiCell();
                dv.cell.CreateObject();
            }
            else if(dv.cell != null && !dv.cell.destroyed)
            {
                dv.CalculateVoronoiCell();
            }
        }
    }


    public void DestoryObject()
    {
        destroyed = true;
        GameObject.Destroy(go);
    }
}

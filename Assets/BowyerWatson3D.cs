using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class BowyerWatson3D : MonoBehaviour
{
    List<Tetraeder> triangulation = new List<Tetraeder>();
    List<Vector3> points = new List<Vector3>();
    List<VoronoiFace> voronoiFaces = new List<VoronoiFace>();
    List<VoronoiCell> voronoiCells = new List<VoronoiCell>();
    private Tetraeder supertetra;


    //List<Vector3> voronoiVertices = new List<Vector3>();


    List<(Vector3, Vector3)> planes = new List<(Vector3, Vector3)>();


    [SerializeField] private int pointCount = 10;
    [SerializeField] private bool drawVoronoi;
    [SerializeField] private bool drawDelaunay;
    [SerializeField] private bool drawVoronoiNodes;
    [SerializeField] private GameObject meshToCut;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = meshToCut.GetComponent<MeshRenderer>();
        meshFilter = meshToCut.GetComponent<MeshFilter>();

        Stopwatch stopWatch = new Stopwatch();

        Vector3 p1 = new Vector3(-2000, -1000, -2000);
        Vector3 p2 = new Vector3(2000, -1000, -2000);
        Vector3 p3 = new Vector3(0, -1000, 2000);
        Vector3 p4 = new Vector3(0, 2000, 0);

        supertetra = new Tetraeder(p1, p2, p3, p4);
        triangulation.Add(supertetra);

        for (int i = 0; i < pointCount; i++)
        {
            points.Add(new Vector3(Random.Range(meshRenderer.bounds.min.x, meshRenderer.bounds.max.x),
                Random.Range(meshRenderer.bounds.min.y, meshRenderer.bounds.max.y),
                Random.Range(meshRenderer.bounds.min.z, meshRenderer.bounds.max.z)));
        }

        //points.AddRange(meshFilter.mesh.vertices.Distinct().Select(x => transform.TransformPoint(x)));


        stopWatch.Start();
        StartAlgorithm();
        stopWatch.Stop();
        Debug.Log("Bowyer Watson Calculation: " + stopWatch.Elapsed);

        stopWatch.Restart();
        CreateVoronoi();
        stopWatch.Stop();
        Debug.Log("Voronoi Conversion: " + stopWatch.Elapsed);

        stopWatch.Restart();
        VoronoiToMesh();
        stopWatch.Stop();
        Debug.Log("Mesh Creation: " + stopWatch.Elapsed);

        //CutMesh(meshToCut);
    }


    private void StartAlgorithm()
    {
        List<Tetraeder> badTetraeder = new List<Tetraeder>();
        List<Face> polygon = new List<Face>();
        List<Tetraeder> toRemove = new List<Tetraeder>();


        for (var index = 0; index < points.Count; index++)
        {
            var point = points[index];
            badTetraeder.Clear();

            foreach (var tetraeder in triangulation)
            {
                if (tetraeder.IsPointInsideCircumSphere(point))
                {
                    badTetraeder.Add(tetraeder);
                }
            }

            polygon.Clear();

            foreach (var tetraeder in badTetraeder)
            {
                foreach (var face in tetraeder.GetFaces())
                {
                    if (badTetraeder.Except(new[] {tetraeder}).SelectMany(t => t.GetFaces()).All(f => !face.Equals(f)))
                    {
                        polygon.Add(face);
                    }
                }
            }

            foreach (var tetraeder in badTetraeder)
            {
                triangulation.Remove(tetraeder);
            }

            foreach (var face in polygon)
            {
                Tetraeder t = new Tetraeder(point, face.p1, face.p2, face.p3);
                triangulation.Add(t);
            }
        }


        for (var index = 0; index < triangulation.Count; index++)
        {
            var tetraeder = triangulation[index];
            if (tetraeder.HasVertex(supertetra.p1) || tetraeder.HasVertex(supertetra.p2) || tetraeder.HasVertex(supertetra.p3) ||
                tetraeder.HasVertex(supertetra.p4))
            {
                toRemove.Add(tetraeder);
            }
        }

        triangulation.RemoveAll(t => toRemove.Contains(t));
    }

    private void CreateVoronoi()
    {
        foreach (var point in points)
        {
            var neighbors = triangulation.Where(t => t.HasVertex(point));
            //var neighbors = triangulation;

            List<Vector3> neighborVertices = new List<Vector3>();
            foreach (var tetraeder in neighbors)
            {
                neighborVertices.Add(tetraeder.p1);
                neighborVertices.Add(tetraeder.p2);
                neighborVertices.Add(tetraeder.p3);
                neighborVertices.Add(tetraeder.p4);
            }

            neighborVertices = neighborVertices.Distinct().ToList();
            neighborVertices.Remove(point);

            foreach (var vertex in neighborVertices)
            {
                var dir = (vertex - point).normalized;
                var mid = (vertex + point) / 2;

                Plane p = new Plane(dir, mid);

                //planes.Add((mid, dir));


                // var planeGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
                // var q = Quaternion.FromToRotation(Vector3.up, p.normal);
                // planeGO.transform.SetPositionAndRotation(mid, q);


                List<Vector3> VoronoiVertices = new List<Vector3>();
                foreach (var tetraeder in neighbors)
                {
                    if (Math.Abs(p.GetDistanceToPoint(tetraeder.circumcenter)) < 0.01)
                    {
                        VoronoiVertices.Add(tetraeder.circumcenter);
                    }
                }

                var face = new VoronoiFace(VoronoiVertices.ToList(), p);
                voronoiFaces.Add(face);
            }

            voronoiCells.Add(new VoronoiCell(point, voronoiFaces.ToList()));
            voronoiFaces.Clear();
        }
    }


    private void VoronoiToMesh()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();
        List<Vector3> meshVertices = new List<Vector3>();

        foreach (var voronoiCell in voronoiCells)
        {
            vertices.Clear();
            triangles.Clear();
            normals.Clear();

            foreach (var voronoiFace in voronoiCell.Faces)
            {
                var bw = new BowyerWatson2D(voronoiFace.points, voronoiFace.Plane);
                meshVertices.Clear();
                foreach (var triangle in bw.Triangulation)
                {
                    meshVertices.Add(triangle.V1);
                    meshVertices.Add(triangle.V2);
                    meshVertices.Add(triangle.V3);

                    if ((Vector3.Cross(meshVertices[1] - meshVertices[0], meshVertices[2] - meshVertices[0]).normalized +
                         voronoiFace.Plane.normal).sqrMagnitude < 2)
                    {
                        meshVertices.Reverse();
                    }

                    vertices.AddRange(meshVertices.ToList());
                    //voronoiVertices.AddRange(meshVertices.ToList());


                    triangles.Add(vertices.Count - 1);
                    triangles.Add(vertices.Count - 3);
                    triangles.Add(vertices.Count - 2);
                    normals.Add(voronoiFace.Plane.normal);
                    normals.Add(voronoiFace.Plane.normal);
                    normals.Add(voronoiFace.Plane.normal);
                }
            }

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            var go = new GameObject();
            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

            //go.transform.position = voronoiCell.Generator;
        }
    }

    private void CutMesh(GameObject meshToCut)
    {
        List<Vector3> outputList = meshFilter.mesh.vertices.ToList();

        foreach (var face in voronoiFaces)
        {
            List<Vector3> inputList = outputList;
            outputList.Clear();

            for (int i = 0; i < inputList.Count; i++)
            {
                Vector3 currentPoint = inputList[i];
                Vector3 prevPoint = inputList[(i - 1) % inputList.Count];
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (triangulation != null)
        {
            if (drawDelaunay)
            {
                for (int i = 0; i < triangulation.Count; i++)
                {
                    Gizmos.color = Color.red;
                    foreach (var vector3 in points.Except(triangulation[i].GetVertices()))
                    {
                        if (triangulation[i].IsPointInsideCircumSphere(vector3))
                        {
                            Gizmos.color = Color.blue;
                        }
                    }


                    Gizmos.DrawLine(triangulation[i].p1, triangulation[i].p2);
                    Gizmos.DrawLine(triangulation[i].p1, triangulation[i].p3);
                    Gizmos.DrawLine(triangulation[i].p1, triangulation[i].p4);
                    Gizmos.DrawLine(triangulation[i].p2, triangulation[i].p3);
                    Gizmos.DrawLine(triangulation[i].p2, triangulation[i].p4);
                    Gizmos.DrawLine(triangulation[i].p3, triangulation[i].p4);

                    // Gizmos.DrawSphere(triangulation[i].circumcenter, 5.0f);
                    // Debug.Log(triangulation[i].radius);
                    // Gizmos.DrawSphere(triangulation[i].circumcenter, triangulation[i].radius);
                }
            }

            Gizmos.color = Color.yellow;
            if (drawVoronoiNodes)
            {
                foreach (var tetraeder in triangulation)
                {
                    Gizmos.DrawSphere(tetraeder.circumcenter, 0.1f);
                }
            }
        }

        if (points == null || points.Count != pointCount) return;

        // Gizmos.color = Color.green;
        // for (int i = 0; i < pointCount; i++)
        // {
        //     Gizmos.DrawSphere(points[i], 1f);
        // }

        // Gizmos.color = Color.green;
        // if (voronoiVertices != null)
        // {
        //     foreach (var vertex in voronoiVertices)
        //     {
        //         Gizmos.DrawSphere(vertex, 0.1f);
        //     }
        // }

        // Gizmos.color = Color.red;
        // if (planes != null)
        // {
        //     foreach (var (mid, dir) in planes)
        //     {
        //         Gizmos.DrawSphere(mid, 0.05f);
        //         Gizmos.DrawRay(mid, dir);
        //     }
        // }


        Gizmos.color = Color.magenta;
        if (voronoiFaces != null)
        {
            if (drawVoronoi)
            {
                foreach (var voronoiCell in voronoiCells)
                {
                    foreach (var voronoiFace in voronoiCell.Faces)
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
            }
        }
    }
}

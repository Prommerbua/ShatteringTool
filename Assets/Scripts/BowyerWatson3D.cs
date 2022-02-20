using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Visualization2D;
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

    private List<Triangle> triangles;
    private VoronoiCell cell1;


    Vector3 intersectionPoint;
    Vector3 intersectionDir;

    private List<Vector3> PlaneLineintersections = new List<Vector3>();


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

        Vector3 A = new Vector3(0, 0, 0);
        Vector3 B = new Vector3(4, 0, 0);
        Vector3 C = new Vector3(4, 0, 4);
        Vector3 D = new Vector3(0, 0, 4);
        Vector3 E = new Vector3(0, 4, 0);
        Vector3 F = new Vector3(4, 4, 0);
        Vector3 G = new Vector3(4, 4, 4);
        Vector3 H = new Vector3(0, 4, 4);
        var face1 = new VoronoiFace(new List<Vector3> {A, B, C, D}, new Plane(Vector3.down, A));
        var face2 = new VoronoiFace(new List<Vector3> {A, B, F, E}, new Plane(Vector3.back, A));
        var face3 = new VoronoiFace(new List<Vector3> {B, F, G, C}, new Plane(Vector3.right, B));
        var face4 = new VoronoiFace(new List<Vector3> {C, D, H, G}, new Plane(Vector3.forward, C));
        var face5 = new VoronoiFace(new List<Vector3> {A, D, H, E}, new Plane(Vector3.left, A));
        var face6 = new VoronoiFace(new List<Vector3> {E, F, G, H}, new Plane(Vector3.up, E));

        cell1 = new VoronoiCell(Vector3.zero, new List<VoronoiFace> {face6, face2, face3, face4, face5, face1});

        stopWatch.Start();
        StartAlgorithm();
        stopWatch.Stop();
        Debug.Log("Bowyer Watson Calculation: " + stopWatch.Elapsed);

        stopWatch.Restart();
        CreateVoronoi();
        stopWatch.Stop();
        Debug.Log("Voronoi Conversion: " + stopWatch.Elapsed);

        stopWatch.Restart();
        //VoronoiToMesh();
        stopWatch.Stop();
        Debug.Log("Mesh Creation: " + stopWatch.Elapsed);

        stopWatch.Restart();
        CutMeshv2(meshToCut);
        stopWatch.Stop();
        Debug.Log("Mesh Cutting: " + stopWatch.Elapsed);
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
                    // meshvertices Clear?
                    meshVertices.Add(triangle.V1);
                    meshVertices.Add(triangle.V2);
                    meshVertices.Add(triangle.V3);

                    if ((Vector3.Cross(meshVertices[1] - meshVertices[0], meshVertices[2] - meshVertices[0]).normalized +
                         voronoiFace.Plane.normal).sqrMagnitude < 2)
                    {
                        meshVertices.Reverse();
                    }

                    vertices.AddRange(meshVertices.ToList());

                    triangles.Add(vertices.Count - 1);
                    triangles.Add(vertices.Count - 3);
                    triangles.Add(vertices.Count - 2);
                }
            }

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            var go = new GameObject("Cell");
            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
        }
    }

    private void CutMesh(GameObject meshToCut)
    {
        triangles = new List<Triangle>();
        var mesh = meshFilter.mesh;
        for (var index = 0; index < mesh.triangles.Length; index += 3)
        {
            var v1 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index]]);
            var v2 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index + 1]]);
            var v3 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index + 2]]);

            Triangle t = new Triangle(v1, v2, v3);
            triangles.Add(t);
        }


        foreach (var voronoiCell in voronoiCells)
        {
            foreach (var face in cell1.Faces.Take(1))
            {
                foreach (var triangle in triangles.Take(1))
                {
                    List<(Vector3, char)> tmpIntersections = new List<(Vector3, char)>();

                    ComputePlanePlaneIntersection(out intersectionPoint, out intersectionDir, face.Plane, face.points[0],
                        new Plane(triangle.V1, triangle.V2, triangle.V3), triangle.V1);

                    foreach (var edge in face.GetEdges())
                    {
                        Plane p = new Plane(edge.Start, edge.End, new Vector3(10, 10, 10));
                        Vector3 intersection = Vector3.zero;


                        ComputeLinePlaneIntersection(out intersection, intersectionDir, intersectionPoint, p.normal, edge.Start);

                        List<Vector3> isIntersecting = new List<Vector3> {edge.Start, edge.End, intersection};
                        isIntersecting = isIntersecting.OrderBy(x => Vector3.Dot(edge.End - edge.Start, x)).ToList();


                        if (intersection != Vector3.zero && isIntersecting[1] == intersection)
                        {
                            tmpIntersections.Add((intersection, 'f'));
                        }
                    }

                    foreach (var edge in triangle.GetEdges())
                    {
                        Plane p = new Plane(edge.Start, edge.End, new Vector3(10, 10, 10));
                        Vector3 intersection = Vector3.zero;

                        ComputeLinePlaneIntersection(out intersection, intersectionDir, intersectionPoint, p.normal, edge.Start);

                        List<Vector3> isIntersecting = new List<Vector3> {edge.Start, edge.End, intersection};
                        isIntersecting = isIntersecting.OrderBy(x => Vector3.Dot(edge.End - edge.Start, x)).ToList();

                        if (intersection != Vector3.zero && isIntersecting[1] == intersection)
                        {
                            tmpIntersections.Add((intersection, 't'));
                        }
                    }

                    if (!tmpIntersections.Any())
                    {
                        continue;
                    }

                    tmpIntersections = tmpIntersections.OrderBy(x => Vector3.Dot(intersectionDir, x.Item1)).ToList();

                    if (tmpIntersections[0].Item2 != tmpIntersections[1].Item2)
                    {
                        PlaneLineintersections.AddRange(tmpIntersections.Skip(1).Take(2).Select(x => x.Item1));
                    }

                    //TODO: Add Points from face that are inside mesh
                }
            }
        }
    }


    private void CutMeshv2(GameObject meshToCut)
    {
        // var lowerVerts = meshToCut.GetComponent<MeshFilter>().mesh.vertices.Select(x => this.meshToCut.transform.TransformPoint(x)).ToList();
        // var lowerTris = meshToCut.GetComponent<MeshFilter>().mesh.triangles.ToList();
        //
        // var upperVerts = meshToCut.GetComponent<MeshFilter>().mesh.vertices.Select(x => this.meshToCut.transform.TransformPoint(x)).ToList();
        // var upperTris = meshToCut.GetComponent<MeshFilter>().mesh.triangles.ToList();

        triangles = new List<Triangle>();
        var mesh = meshFilter.mesh;
        for (var index = 0; index < mesh.triangles.Length; index += 3)
        {
            var v1 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index]]);
            var v2 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index + 1]]);
            var v3 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index + 2]]);

            Triangle t = new Triangle(v1, v2, v3);
            triangles.Add(t);
        }

        var lowerTris = triangles.ToList();
        var upperTris = triangles.ToList();

        List<Vector3> tmpVertices = new List<Vector3>();
        foreach (var voronoiFace in cell1.Faces.Take(1))
        {
            List<Vector3> fillVertices = new List<Vector3>();

            foreach (var triangle in triangles)
            {
                List<Vector3> newVerts = new List<Vector3>();
                foreach (var edge in triangle.GetEdges())
                {
                    //Check if edge is intersecting with plane of face
                    var d = Vector3.Dot(voronoiFace.points[0] - edge.Start, voronoiFace.Plane.normal) /
                            Vector3.Dot(edge.End - edge.Start, voronoiFace.Plane.normal);

                    //if intersecting, calculate new vertices with interpolation
                    if (d >= 0 && d <= 1)
                    {
                        var newVert = edge.End * d + edge.Start * (1 - d);
                        newVerts.Add(newVert);
                        PlaneLineintersections.Add(newVert);
                    }
                }

                fillVertices.AddRange(newVerts);

                var trianglePlane = new Plane(triangle.V1, triangle.V2, triangle.V3);

                bool isV1Above = voronoiFace.Plane.GetSide(triangle.V1);
                bool isV2Above = voronoiFace.Plane.GetSide(triangle.V2);
                bool isV3Above = voronoiFace.Plane.GetSide(triangle.V3);

                tmpVertices.Clear();

                if (isV1Above && isV2Above && !isV3Above)
                {
                    Debug.Log("Case 1");

                    #region Case1

                    lowerTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));
                    upperTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));


                    //Upper (Quad)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V1);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(triangle.V1);
                    tmpVertices.Add(triangle.V2);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    //Lower (Triangle)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V3);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    #endregion
                }
                else if (isV1Above && !isV2Above && isV3Above)
                {
                    Debug.Log("Case 2");

                    #region Case2

                    lowerTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));
                    upperTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));

                    //Upper (Quad)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V1);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(triangle.V1);
                    tmpVertices.Add(triangle.V3);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    //Lower (Triangle)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V2);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    #endregion
                }
                else if (isV1Above && !isV2Above && !isV3Above)
                {
                    Debug.Log("Case 3");

                    #region Case3

                    lowerTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));
                    upperTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));

                    //Lower (Quad)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V2);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(triangle.V2);
                    tmpVertices.Add(triangle.V3);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    //Upper (Triangle)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V1);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    #endregion
                }
                else if (!isV1Above && isV2Above && isV3Above)
                {
                    Debug.Log("Case 4");

                    #region Case4

                    lowerTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));
                    upperTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));

                    //Upper (Quad)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V2);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(triangle.V2);
                    tmpVertices.Add(triangle.V3);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    //Lower (Triangle)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V1);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    #endregion
                }
                else if (!isV1Above && isV2Above && !isV3Above)
                {
                    Debug.Log("Case 5");

                    #region Case5

                    lowerTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));
                    upperTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));

                    //Lower (Quad)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V1);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(triangle.V1);
                    tmpVertices.Add(triangle.V3);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    //Upper (Triangle)
                    tmpVertices.Clear();
                    tmpVertices.Add(newVerts[1]);
                    tmpVertices.Add(newVerts[0]);
                    tmpVertices.Add(triangle.V2);

                    if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                         trianglePlane.normal).sqrMagnitude < 2)
                    {
                        tmpVertices.Reverse();
                    }

                    upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));

                    #endregion
                }
                else if (isV1Above && isV2Above && isV3Above)
                {
                    Debug.Log("No Intersection - All Above");
                    lowerTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));
                }
                else if (!isV1Above && !isV2Above && !isV3Above)
                {
                    Debug.Log("No Intersection - All below");
                    upperTris.RemoveAll(t => t.HasVertex(triangle.V1) && t.HasVertex(triangle.V2) && t.HasVertex(triangle.V3));
                }
                else
                {
                    Debug.Log("Case missing: " + isV1Above + isV2Above + isV3Above);
                }
            }

            //fill holes for both sides
            fillVertices = fillVertices.Distinct().ToList();
            var fillPlane = new Plane(fillVertices[0], fillVertices[1], fillVertices[3]);
            var bw = new BowyerWatson2D(fillVertices, fillPlane);

            foreach (var triangle in bw.Triangulation)
            {
                tmpVertices.Clear();
                tmpVertices.Add(triangle.V1);
                tmpVertices.Add(triangle.V2);
                tmpVertices.Add(triangle.V3);

                if ((Vector3.Cross(tmpVertices[1] - tmpVertices[0], tmpVertices[2] - tmpVertices[0]).normalized +
                     fillPlane.normal).sqrMagnitude < 2)
                {
                    tmpVertices.Reverse();
                }

                upperTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));
                tmpVertices.Reverse();
                lowerTris.Add(new Triangle(tmpVertices[2], tmpVertices[0], tmpVertices[1]));
            }
        }

        // var originalPosition = meshToCut.transform.position;
        // var originalRotation = meshToCut.transform.rotation;
        // var originalScale = meshToCut.transform.localScale;

        tmpVertices.Clear();
        var meshVertices = new List<Vector3>();
        var meshTris = new List<int>();

        foreach (var triangle in upperTris)
        {
            tmpVertices.Clear();
            tmpVertices.Add(triangle.V1);
            tmpVertices.Add(triangle.V2);
            tmpVertices.Add(triangle.V3);

            meshVertices.AddRange(tmpVertices.ToList());

            meshTris.Add(meshVertices.Count - 1);
            meshTris.Add(meshVertices.Count - 3);
            meshTris.Add(meshVertices.Count - 2);
        }

        var upperMesh = new Mesh();
        upperMesh.vertices = meshVertices.ToArray();
        upperMesh.triangles = meshTris.ToArray();
        upperMesh.RecalculateNormals();

        var go = new GameObject("Upper");
        // go.transform.position = originalPosition;
        // go.transform.rotation = originalRotation;
        // go.transform.localScale = originalScale;
        go.AddComponent<MeshFilter>().mesh = upperMesh;
        go.AddComponent<MeshCollider>().sharedMesh = upperMesh;
        go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));


        tmpVertices.Clear();
        meshVertices.Clear();
        meshTris.Clear();

        foreach (var triangle in lowerTris)
        {
            tmpVertices.Clear();
            tmpVertices.Add(triangle.V1);
            tmpVertices.Add(triangle.V2);
            tmpVertices.Add(triangle.V3);

            meshVertices.AddRange(tmpVertices.ToList());

            meshTris.Add(meshVertices.Count - 1);
            meshTris.Add(meshVertices.Count - 3);
            meshTris.Add(meshVertices.Count - 2);
        }

        var lowerMesh = new Mesh();
        lowerMesh.vertices = meshVertices.ToArray();
        lowerMesh.triangles = meshTris.ToArray();
        lowerMesh.RecalculateNormals();

        var go2 = new GameObject("Lower");
        // go2.transform.position = originalPosition;
        // go2.transform.rotation = originalRotation;
        // go2.transform.localScale = originalScale;
        go2.AddComponent<MeshFilter>().mesh = lowerMesh;
        go2.AddComponent<MeshCollider>().sharedMesh = lowerMesh;
        go2.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        meshToCut.SetActive(false);
    }

    private void RemoveTriangle(ref List<Vector3> vertices, ref List<int> tris, Triangle triangle)
    {
        int index1 = vertices.FindIndex(x => x == triangle.V1);
        int index2 = vertices.FindIndex(x => x == triangle.V2);
        int index3 = vertices.FindIndex(x => x == triangle.V3);

        var indices = new[] {index1, index2, index3};
        var index = tris.Window(3).ToList().FindIndex(w => w.OrderBy(i => i).SequenceEqual(indices.OrderBy(i => i))) * 3;
        if (index >= 0) tris.RemoveRange(index, 3);
    }


    private void ComputePlanePlaneIntersection(out Vector3 intersectionPoint, out Vector3 intersectionDir, Plane cuttingPlane,
        Vector3 pointOnCuttinPlane, Plane trianglePlane, Vector3 pointOnTriangle)
    {
        intersectionPoint = Vector3.zero;
        intersectionDir = Vector3.zero;

        //Get the normals of the planes.
        Vector3 plane1Normal = cuttingPlane.normal;
        Vector3 plane2Normal = trianglePlane.normal;

        //We can get the direction of the line of intersection of the two planes by calculating the
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet.
        intersectionDir = Vector3.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.
        Vector3 ldir = Vector3.Cross(plane2Normal, intersectionDir);

        float numerator = Vector3.Dot(plane1Normal, ldir);


        //Prevent divide by zero.
        if (Mathf.Abs(numerator) > 0.000001f)
        {
            Vector3 plane1ToPlane2 = pointOnCuttinPlane - pointOnTriangle;
            float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / numerator;
            intersectionPoint = pointOnTriangle + t * ldir;
        }
    }

    bool ComputeLinePlaneIntersection(out Vector3 intersection, Vector3 ray, Vector3 from, Vector3 normal, Vector3 coord)
    {
        float d = Vector3.Dot(normal, coord);

        if (Vector3.Dot(normal, ray) == 0)
        {
            intersection = Vector3.zero;
            return false;
        }

        float x = (d - Vector3.Dot(normal, from)) / Vector3.Dot(normal, ray);

        intersection = from + ray * x;
        return true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawRay(intersectionPoint, intersectionDir * 10);
        Gizmos.DrawRay(intersectionPoint, -intersectionDir * 10);

        Gizmos.color = Color.green;
        if (PlaneLineintersections.Count != 0)
        {
            foreach (var intersection in PlaneLineintersections)
            {
                Gizmos.DrawSphere(intersection, 0.1f);
            }
        }


        if (cell1.Faces == null) return;
        foreach (var face in cell1.Faces)
        {
            Gizmos.color = Color.blue;
            for (var index = 0; index < face.points.Count; index++)
            {
                var p1 = face.points[index];
                Vector3 p2 = Vector3.zero;
                if (index + 1 < face.points.Count) p2 = face.points[index + 1];
                else p2 = face.points[0];

                Gizmos.DrawLine(p1, p2);
            }

            // Gizmos.color = Color.red;
            //
            // Vector3 mid = Vector3.zero;
            // foreach (var point in face.points)
            // {
            //     mid += point;
            // }
            // mid /= 4;
            //
            // Gizmos.DrawRay(mid, face.Plane.normal * 5);
        }


        Gizmos.color = Color.red;
        foreach (var triangle in triangles.Take(4))
        {
            Gizmos.DrawLine(triangle.V1, triangle.V2);
            Gizmos.DrawLine(triangle.V2, triangle.V3);
            Gizmos.DrawLine(triangle.V3, triangle.V1);

            Gizmos.DrawSphere(triangle.V1, 0.4f);
            Gizmos.DrawSphere(triangle.V2, 0.4f);
            Gizmos.DrawSphere(triangle.V3, 0.4f);
        }


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
                            Debug.Log("Error in Delaunay!!");
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

public static class MoreEnumerable
{
    public static IEnumerable<IList<TSource>> Window<TSource>(this IEnumerable<TSource> source, int size)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

        return _();

        IEnumerable<IList<TSource>> _()
        {
            using var iter = source.GetEnumerator();

            // generate the first window of items
            var window = new TSource[size];
            int i;
            for (i = 0; i < size && iter.MoveNext(); i++)
                window[i] = iter.Current;

            if (i < size)
                yield break;

            while (iter.MoveNext())
            {
                // generate the next window by shifting forward by one item
                // and do that before exposing the data
                var newWindow = new TSource[size];
                Array.Copy(window, 1, newWindow, 0, size - 1);
                newWindow[size - 1] = iter.Current;

                yield return window;
                window = newWindow;
            }

            // return the last window.
            yield return window;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Habrador_Computational_Geometry;

public class CreateMeshFromVertices : MonoBehaviour
{
    public Material material;
    public MeshFilter srcMesh;


    private Mesh delaunayMesh;



    private HashSet<Vector3> vertices_Unity;

    private HashSet<Mesh> voronoiCellsMeshes;
    [SerializeField] private int pointCount;


    void Start()
    {
        vertices_Unity = new HashSet<Vector3>();

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 point = srcMesh.mesh.GetRandomPointInsideNonConvex(srcMesh.GetComponent<Renderer>().bounds.center);
            vertices_Unity.Add(point);
        }




        HashSet<MyVector3> points = new HashSet<MyVector3>(vertices_Unity.Select(x => x.ToMyVector3()));
        Normalizer3 normalizer = new Normalizer3(new List<MyVector3>(points));
        HashSet<MyVector3> points_normalized = normalizer.Normalize(points);

        //Convex Hull
        
        
        HalfEdgeData3 convexHull_normalized = _ConvexHull.Iterative_3D(points_normalized, removeUnwantedTriangles: false, normalizer);
        //HalfEdgeData3 convexHull_unnormalized = _ConvexHull.Iterative_3D(points, removeUnwantedTriangles: false, normalizer);

        var tmp = convexHull_normalized.ConvertToMyMesh("Triangulated Points", MyMesh.MeshStyle.HardEdges);
        delaunayMesh = tmp.ConvertToUnityMesh(true);

        // if (convexHull_normalized == null)
        // {
        //     Debug.LogError("ConvexHull is null");
        //     return;
        // }
        //
        HashSet<VoronoiCell3> voronoiCells_normalized = _Voronoi.Delaunay3DToVoronoi(convexHull_normalized);

        HalfEdgeData3 convexHull = normalizer.UnNormalize(convexHull_normalized);

        MyMesh myMesh = convexHull.ConvertToMyMesh("convex hull aka delaunay triangulation", MyMesh.MeshStyle.HardEdges);

        delaunayMesh = myMesh.ConvertToUnityMesh(generateNormals: false);


        //Voronoi
        HashSet<VoronoiCell3> voronoiCells = normalizer.UnNormalize(voronoiCells_normalized);

        //Generate a mesh for each separate cell
        voronoiCellsMeshes = GenerateVoronoiCellsMeshes(voronoiCells);

        foreach (var mesh in voronoiCellsMeshes)
        {
            GameObject go = new GameObject();
            var meshfilter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            meshfilter.mesh = mesh;
            go.transform.parent = gameObject.transform;
        }


        //Generate a single mesh for all cells where each vertex has a color belonging to that cell
        //Now we can display the mesh with an unlit shader where each vertex is associated with a color belonging to that cell
        //The problem is that the voronoi cell is not a flat surface on the mesh
        //But it looks flat if we are using an unlit shader
        // Mesh oneMesh = GenerateAndDisplaySingleMesh(voronoiCellsMeshes);
        //
        // if (meshFilter != null)
        // {
        //     meshFilter.mesh = oneMesh;
        // }
        // else
        // {
        //     Debug.LogError("Meshfilter is null");
        // }
    }

    private HashSet<Mesh> GenerateVoronoiCellsMeshes(HashSet<VoronoiCell3> voronoiCells)
    {
        HashSet<Mesh> meshes = new HashSet<Mesh>();

        foreach (VoronoiCell3 cell in voronoiCells)
        {
            List<Vector3> vertices = new List<Vector3>();

            List<int> triangles = new List<int>();

            List<Vector3> normals = new List<Vector3>();

            List<VoronoiEdge3> edges = cell.edges;

            //This is the center of the cell
            //To build the mesh, we just add triangles from the edges to the site pos
            MyVector3 sitePos = cell.sitePos;

            //In 3d space, the corners in the voronoi cell are not on the plane, so shading becomes bad
            //Shading improves if we calculate an average site pos by looking at each corner in the cell
            MyVector3 averageSitePos = default;

            for (int i = 0; i < edges.Count; i++)
            {
                averageSitePos += edges[i].p1;
            }

            averageSitePos = averageSitePos * (1f / edges.Count);

            vertices.Add(averageSitePos.ToVector3());

            //VoronoiEdge3 e0 = edges[0];

            //Vector3 normal = Vector3.Cross(e0.p2.ToVector3() - e0.p1.ToVector3(), e0.sitePos.ToVector3() - e0.p1.ToVector3()).normalized;

            //normals.Add(normal);


            //Another way to get a nicer looking surface is to use a vertex color
            //and then use a shader set to non-lit
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);

            List<Color> vertexColors = new List<Color>();

            vertexColors.Add(color);

            foreach (VoronoiEdge3 e in edges)
            {
                //Build a triangle with this edge and the voronoi site which is sort of the center
                vertices.Add(e.p2.ToVector3());
                vertices.Add(e.p1.ToVector3());
                //verts.Add(e.sitePos.ToVector3());

                //normals.Add(normal);
                //normals.Add(normal);

                vertexColors.Add(color);
                vertexColors.Add(color);

                int triangleCounter = triangles.Count;

                triangles.Add(0);
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 2);
            }

            Mesh mesh = new Mesh();

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            //mesh.SetNormals(normals);

            mesh.SetColors(vertexColors);

            mesh.RecalculateNormals();

            meshes.Add(mesh);
        }

        return meshes;
    }

    private Mesh GenerateAndDisplaySingleMesh(HashSet<Mesh> meshes)
    {
        Mesh voronoiCellsMesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> vertexColors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();

        foreach (Mesh mesh in meshes)
        {
            int numberOfVerticesBefore = vertices.Count;

            vertices.AddRange(mesh.vertices);
            vertexColors.AddRange(mesh.colors);
            normals.AddRange(mesh.normals);

            //Triangles are not the same
            int[] oldTriangles = mesh.triangles;

            for (int i = 0; i < oldTriangles.Length; i++)
            {
                triangles.Add(oldTriangles[i] + numberOfVerticesBefore);
            }
        }

        Mesh oneMesh = new Mesh();

        oneMesh.SetVertices(vertices);
        oneMesh.SetTriangles(triangles, 0);
        oneMesh.SetNormals(normals);
        oneMesh.SetColors(vertexColors);

        return oneMesh;
    }

}

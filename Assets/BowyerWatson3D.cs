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
    private Tetraeder supertetra;

    [SerializeField] private int pointCount = 10;
    [SerializeField] private bool drawVoronoi;
    [SerializeField] private bool drawDelaunay;
    [SerializeField] private bool drawVoronoiNodes;

    // Start is called before the first frame update
    void Start()
    {
        Stopwatch stopWatch = new Stopwatch();

        Vector3 p1 = new Vector3(-2000, -1000, -2000);
        Vector3 p2 = new Vector3(2000, -1000, -2000);
        Vector3 p3 = new Vector3(0, -1000, 2000);
        Vector3 p4 = new Vector3(0, 2000, 0);

        supertetra = new Tetraeder(p1, p2, p3, p4);
        triangulation.Add(supertetra);

        for (int i = 0; i < pointCount; i++)
        {
            points.Add(new Vector3(Random.Range(-300.0f, 300.0f), Random.Range(-300.0f, 300.0f), Random.Range(-300.0f, 300.0f)));
        }

        stopWatch.Start();
        StartAlgorithm();
        CreateVoronoi();
        stopWatch.Stop();
        Debug.Log(stopWatch.Elapsed);
    }

    public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool isClockWise = true;

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        if (determinant > 0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }

    private void CreateVoronoi()
    {
        foreach (var point in points)
        {
            var neighbors = triangulation.Where(t => t.HasVertex(point));

            List<Vector3> circumcenter = new List<Vector3>();
            List<Vector3> neighborVertices = new List<Vector3>();
            foreach (var tetraeder in neighbors)
            {
                circumcenter.Add(tetraeder.circumcenter);
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

                List<Vector3> VoronoiVertices = new List<Vector3>();
                foreach (var tetraeder in neighbors)
                {
                    if (Math.Abs(p.GetDistanceToPoint(tetraeder.circumcenter)) < 0.01)
                    {
                        VoronoiVertices.Add(tetraeder.circumcenter);
                    }
                }

                var face = new VoronoiFace(VoronoiVertices);
                voronoiFaces.Add(face);
            }
        }
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


    struct Tetraeder
    {
        public Vector3 p1 => points[0];
        public Vector3 p2 => points[1];
        public Vector3 p3 => points[2];
        public Vector3 p4 => points[3];

        private Vector3[] points;

        public bool isBad;

        public Vector3 circumcenter;
        public float radius;

        public Tetraeder(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) : this()
        {
            points = new[] {p1, p2, p3, p4};

            isBad = false;
            calculateCircumsphere();
        }

        private void calculateCircumsphere()
        {
            Vector3 v1 = p2 - p1;
            Vector3 v2 = p3 - p1;
            Vector3 v3 = p4 - p1;

            float l1 = v1.sqrMagnitude;
            float l2 = v2.sqrMagnitude;
            float l3 = v3.sqrMagnitude;
            circumcenter = p1 + (l1 * Vector3.Cross(v2, v3) + l2 * Vector3.Cross(v3, v1) + l3 * Vector3.Cross(v1, v2)) /
                (2 * Vector3.Dot(v1, Vector3.Cross(v2, v3)));
            radius = (p1 - circumcenter).magnitude;
            // circumcenter = circumcenter * 1000;
            // circumcenter = new Vector3((int) circumcenter.x / 1000.0f, (int) circumcenter.y / 1000.0f, (int) circumcenter.z / 1000.0f);
        }

        public Face[] GetFaces()
        {
            return new [] {new Face(p1, p2, p3), new Face(p2, p3, p4), new Face(p3, p4, p1), new Face(p4, p1, p2)};
        }

        public bool HasVertex(Vector3 vertex)
        {
            return points.Contains(vertex);
        }

        public bool IsPointInsideCircumSphere(Vector3 point)
        {
            return radius >= (point - circumcenter).magnitude;
        }
    }

    struct VoronoiFace
    {
        public List<Vector3> points;


        public VoronoiFace(List<Vector3> points)
        {
            this.points = points;
        }
    }

    struct Face
    {
        private Vector3[] points;

        public Vector3 p1 => points[0];
        public Vector3 p2 => points[1];
        public Vector3 p3 => points[2];

        public Face(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            points = new[] {p1, p2, p3};
        }

        public bool Equals(Face other)
        {
            return !(points.Except(other.points).Any());
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (triangulation != null)
        {
            if (drawDelaunay)
            {
                for (int i = 0; i < triangulation.Count; i++)
                {
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
                    Gizmos.DrawSphere(tetraeder.circumcenter, 5f);
                }
            }
        }

        if (points == null || points.Count != pointCount) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < pointCount; i++)
        {
            Gizmos.DrawSphere(points[i], 1f);
        }


        Gizmos.color = Color.magenta;
        if (voronoiFaces != null)
        {
            if (drawVoronoi)
            {
                foreach (var voronoiFace in voronoiFaces)
                {
                    for (var index = 0; index < voronoiFace.points.Count; index++)
                    {
                        var point = voronoiFace.points[index];
                        Vector3 point2 = Vector3.one;
                        if (index == voronoiFace.points.Count - 1) point2 = voronoiFace.points[0];
                        else point2 = voronoiFace.points[index + 1];

                        Gizmos.DrawLine(point, point2);
                    }
                }
            }
        }
    }
}

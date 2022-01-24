using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using MeshSplit;
using MeshSplitting.Splitables;
using MeshSplitting.Splitters;
using Debug = UnityEngine.Debug;

public class DelaunayControl : MonoBehaviour
{
    [SerializeField] private int pointCount;


    private BowyerWatson bw;
    // Use this for initialization

    private bool _drawDelaunay = false;
    private bool _drawVoronoi = false;

    public MeshFilter srcMesh;
    private Stopwatch _sw;
    private Splitable _splitable;
    private List<Vector3> points;

    public float CutPlaneSize;

    private Vector3 center;

    void Start()
    {
        UnityEngine.Random.InitState(11);
        _sw = new Stopwatch();
        _splitable = srcMesh.GetComponent<Splitable>();
        points = new List<Vector3>();
    }


    void CalculateVoronoiOnCollision(Vector3 contactPoint)
    {
        _sw.Start();
        //TODO: points based on collision position
        //var center = srcMesh.GetComponent<Renderer>().bounds.center;
        var centerPoint = contactPoint;
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 point = srcMesh.mesh.GetRandomPointInsideNonConvex(centerPoint);
            Vector3 doublePoint = new Vector3(point.x, point.y, point.z);
            points.Add(doublePoint);
        }

        _sw.Stop();
        Debug.Log("Point sampling took " + _sw.Elapsed);

        _sw.Restart();
        bw = new BowyerWatson(points, srcMesh);
        bw.MakeDiagram();
        _sw.Stop();

        Debug.Log("Computation took " + _sw.Elapsed);

        _sw.Restart();

        //createCells();

        foreach (var vertex in bw.vertices)
        {
            vertex.CalculateVoronoiCell();
            foreach (var plane in vertex.cell.faces)
            {
                center = Vector3.zero;
                foreach (Vector3 v in plane.vertices)
                {
                    center += v;
                }

                center /= plane.vertices.Count;
                Vector3 normal = plane.plane.normal;
                Vector3 fwd = Vector3.ProjectOnPlane(Vector3.forward, normal);


                GameObject goCutPlane = new GameObject("CutPlane", typeof(BoxCollider), typeof(Rigidbody), typeof(SplitterSingleCut));

                goCutPlane.GetComponent<Collider>().isTrigger = true;
                Rigidbody bodyCutPlane = goCutPlane.GetComponent<Rigidbody>();
                bodyCutPlane.useGravity = false;
                bodyCutPlane.isKinematic = true;

                Transform transformCutPlane = goCutPlane.transform;
                transformCutPlane.position = center;
                transformCutPlane.localScale = new Vector3(CutPlaneSize, .01f, CutPlaneSize);
                transformCutPlane.up = normal;
                float angleFwd = Vector3.Angle(transformCutPlane.forward, fwd);
                transformCutPlane.RotateAround(center, normal, normal.y < 0f ? -angleFwd : angleFwd);
            }
        }

        _sw.Stop();
        Debug.Log("Mesh Cutting took " + _sw.Elapsed);
    }

    // Update is called once per frame
    void Update()
    {
        if (_drawDelaunay) bw.DrawTriangulation();
        if (_drawVoronoi) bw.DrawVoronoi();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit");
        if (other.gameObject.name == "Projectile(Clone)")
        {
            Destroy(other.gameObject);
            CalculateVoronoiOnCollision(other.GetContact(0).point);
        }
    }

    public void drawTriangulation()
    {
        _drawDelaunay = !_drawDelaunay;
    }

    public void drawVoronoi()
    {
        _drawVoronoi = !_drawVoronoi;
    }

    public void createCells()
    {
        foreach (var vertex in bw.vertices)
        {
            vertex.CalculateVoronoiCell();
            vertex.CreateCell();
        }
    }

    // private void OnDrawGizmos()
    // {
    //     if (points != null)
    //     {
    //         foreach (var point in points)
    //         {
    //             Gizmos.color = Color.red;
    //             Gizmos.DrawSphere(point, 0.05f);
    //         }
    //     }
    //
    //     Gizmos.color = Color.red;
    //     if (bw != null)
    //     {
    //         foreach (var vertex in bw.vertices)
    //         {
    //             foreach (var plane in vertex.cell.faces)
    //             {
    //                 foreach (var planeVertex in plane.vertices)
    //                 {
    //                     Gizmos.DrawSphere(planeVertex, 0.2f);
    //                 }
    //             }
    //         }
    //     }
    //
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawSphere(center, 0.2f);
    // }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EzySlice;
using Habrador_Computational_Geometry;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FracturePattern : MonoBehaviour
{
    public GameObject Pattern;
    public StarterAssetsInputs _input;
    public Material fillMaterial;
    private Stopwatch _sw;
    private bool _broken = false;


    private Rigidbody _rb;

    private void Awake()
    {
        _input = new StarterAssetsInputs();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        //var meshes = Pattern.GetComponentsInChildren<MeshFilter>();
        _rb = GetComponent<Rigidbody>();
        _sw = new Stopwatch();


        //_input.Player.Shoot.started += Break;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Projectile(Clone)")
        {
            if (!_broken)
            {
                _broken = true;
                _rb.AddForce(other.rigidbody.velocity);
                Break(other.GetContact(0));
            }
        }
    }

    private void Break(ContactPoint impact)
    {
        //Align Fracture Pattern
        // var colliders = Pattern.GetComponentsInChildren<Collider>();
        // Bounds bounds = colliders[0].bounds;
        // foreach(var c in colliders) bounds.Encapsulate(c.bounds);
        //
        // var szA = GetComponent<Collider>().bounds.size;
        // var szB = bounds.size;
        // var scale = new Vector3(szA.x / szB.x, szA.y / szB.y, szA.z / szB.z);
        // Pattern.transform.localScale = scale;
        var patternGO = Instantiate(Pattern, impact.point, transform.rotation);






        Debug.Break();
        //Intersection: Clip Convex against all planes of cell of fracture pattern

        // foreach (var convex in GetComponentsInChildren<MeshCollider>())
        // {



        foreach (var cell in patternGO.GetComponentsInChildren<MeshCollider>())
        {
            Vector3 direction;
            float distance;
            // if (Physics.ComputePenetration(convex, convex.gameObject.transform.position, convex.gameObject.transform.rotation, cell, cell.gameObject.transform.position,
            //     cell.gameObject.transform.rotation, out direction, out distance))
            {
                // var hulls = new List<SlicedHull>();
                // var triangleCount = cell.sharedMesh.triangles.Length / 3;
                //
                // //Get Planes
                // for (int i = 0; i < triangleCount; i++)
                // {
                //     var sharedMesh = cell.sharedMesh;
                //     Vector3 V1 = sharedMesh.vertices[sharedMesh.triangles[i * 3]];
                //     Vector3 V2 = sharedMesh.vertices[sharedMesh.triangles[i * 3 + 1]];
                //     Vector3 V3 = sharedMesh.vertices[sharedMesh.triangles[i * 3 + 2]];
                //
                //     var plane = new Plane(V1, V2, V3);
                //
                //     if (!planes.Contains(plane)) planes.Add(plane);
                // }








                // var convexvertices = clonedMesh.vertices.ToList();
                // var cellVertices = cell.sharedMesh.vertices.ToList();
                // var newMesh = new MyMesh("Fractured");
                // var newVertices = SutherlandHodgman.ClipPolygon(convexvertices, cellVertices);
                // var newTriangles = TriangulateConvexPolygon(newVertices);
                //
                // newMesh.vertices = newVertices;
                // foreach (var triangle in newTriangles)
                // {
                // }
                //
                // clonedMesh.Clear();
                // clonedMesh.vertices = newVertices.ToArray();
                // clonedMesh.triangles = newTriangles.
                // convex.sharedMesh = clonedMesh;



                // foreach (var plane in planes)
                // {
                //     var hull = convex.gameObject.Slice(plane, fillMaterial);
                //     if(hull != null) hulls.Add(hull);
                // }
                //
                //
                // foreach (var slicedHull in hulls)
                // {
                //     slicedHull.CreateLowerHull(convex.gameObject, fillMaterial);
                //     slicedHull.CreateUpperHull(convex.gameObject, fillMaterial);
                //
                // }
                // convex.gameObject.SetActive(false);
                //Debug.Break();
                //
                //     if (hull != null)
                //     {
                //         hulls.Add(hull);
                //
                //         var go = hull.CreateLowerHull(convex.gameObject, fillMaterial);
                //         var rb = go.AddComponent<Rigidbody>();
                //         rb.velocity = GetComponent<Rigidbody>().velocity;
                //         var newCollider = go.AddComponent<MeshCollider>();
                //         newCollider.convex = true;
                //
                //         go = hull.CreateUpperHull(convex.gameObject, fillMaterial);
                //         rb = go.AddComponent<Rigidbody>();
                //         rb.velocity = GetComponent<Rigidbody>().velocity;
                //         newCollider = go.AddComponent<MeshCollider>();
                //         newCollider.convex = true;
                //     }
                // }
                // intersections.Add(hulls);
            }
        }
        //}


        // var hull = gameObject.Slice(transform.position, transform.up, fillMaterial);
        //
        // if (hull != null)
        // {
        //     _sw.Start();
        //     // var go = hull.CreateLowerHull(gameObject, fillMaterial);
        //     // var rb = go.AddComponent<Rigidbody>();
        //     // rb.velocity = GetComponent<Rigidbody>().velocity;
        //     // var newCollider = go.AddComponent<MeshCollider>();
        //     // newCollider.convex = true;
        //     // var patternScript= go.AddComponent<FracturePattern>();
        //     // patternScript.fillMaterial = fillMaterial;
        //     //
        //     //
        //     // go = hull.CreateUpperHull(gameObject, fillMaterial);
        //     // rb = go.AddComponent<Rigidbody>();
        //     // rb.velocity = GetComponent<Rigidbody>().velocity;
        //     // newCollider = go.AddComponent<MeshCollider>();
        //     // newCollider.convex = true;
        //     // patternScript= go.AddComponent<FracturePattern>();
        //     // patternScript.fillMaterial = fillMaterial;
        //
        //
        //     var lower = Instantiate(this);
        //     lower.GetComponent<MeshFilter>().mesh = hull.lowerHull;
        //     lower.GetComponent<MeshCollider>().sharedMesh = hull.lowerHull;
        //     lower.GetComponent<Rigidbody>().AddForce(_rb.velocity);
        //     Destroy(lower.GetComponent<FracturePattern>());
        //
        //     var upper = Instantiate(this);
        //     upper.GetComponent<MeshFilter>().mesh = hull.upperHull;
        //     upper.GetComponent<MeshCollider>().sharedMesh = hull.upperHull;
        //     upper.GetComponent<Rigidbody>().AddForce(_rb.velocity);
        //     Destroy(upper.GetComponent<FracturePattern>());
        //
        //
        //     gameObject.SetActive(false);
        //     _sw.Stop();
        //     Debug.Log("Instantiate took " + _sw.Elapsed);
        // }
        //Destroy(patternGO);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Rescale(Transform obj, Vector3 newScale)
    {
        if(obj.root != obj)
        {
            Transform parent = obj.parent;
            obj.SetParent(null);
            obj.localScale = newScale;
            obj.SetParent(parent, true);
        }
    }

    public static List<Triangle3> TriangulateConvexPolygon(List<Vector3> convexHullpoints)
    {
        List<Triangle3> triangles = new List<Triangle3>();

        for (int i = 2; i < convexHullpoints.Count; i++)
        {
            Vector3 a = convexHullpoints[0];
            Vector3 b = convexHullpoints[i - 1];
            Vector3 c = convexHullpoints[i];

            triangles.Add(new Triangle3(a, b, c));
        }

        return triangles;
    }
}




using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SutherlandHodgman3D
{
    private readonly GameObject meshToCut;
    private readonly List<VoronoiCell> voronoiCells;

    public SutherlandHodgman3D(GameObject meshToCut, List<VoronoiCell> voronoiCells)
    {
        this.meshToCut = meshToCut;
        this.voronoiCells = voronoiCells;
    }

    public List<Vector3> ClipPolygon()
    {
        var mesh = meshToCut.GetComponent<MeshFilter>().mesh;
        var faces = new List<VoronoiFace>();

        for (var index = 0; index < mesh.triangles.Length; index += 3)
        {
            var v1 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index]]);
            var v2 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index + 1]]);
            var v3 = meshToCut.transform.TransformPoint(mesh.vertices[mesh.triangles[index + 2]]);

            Plane p = new Plane(v1, v2, v3);
            var face = new VoronoiFace(new List<Vector3> {v1, v2, v3}, p);
            foreach (var meshVertex in mesh.vertices)
            {
                var point = meshToCut.transform.TransformPoint(meshVertex);
                var dot = Vector3.Dot(v1 - point, p.normal);
                if (Math.Abs(dot) < 0.00000001f)
                {
                    face.points.Add(point);
                }
            }

            face.points = face.points.Distinct().ToList();
            faces.Add(face);
        }

        faces = faces.GroupBy(x => x.Plane).Select(g => g.First()).ToList();
        var meshCell = new VoronoiCell(faces);

        var outputList = new List<Vector3>();

        foreach (var cell in voronoiCells)
        {
            foreach (var voronoiFace in cell.Faces)
            {
                var inputList = faces.ToList();
                faces.Clear();

                foreach (var face in inputList)
                {
                    Vector3 intersectionPoint = Vector3.zero;
                    Vector3 intersectionDir = Vector3.zero;


                    ComputePlanePlaneIntersection(out intersectionPoint, out intersectionDir, voronoiFace.Plane, voronoiFace.points[0],
                        face.Plane, face.points[0]);
                    if (intersectionPoint == Vector3.zero && intersectionDir == Vector3.zero) continue;

                    var tmpIntersections = new List<Vector3>();

                    foreach (var edge in voronoiFace.GetEdges())
                    {
                        Plane p = new Plane(edge.Start, edge.End, new Vector3(10, 10, 10));
                        Vector3 intersection = Vector3.zero;

                        ComputeLinePlaneIntersection(out intersection, intersectionDir, intersectionPoint, p.normal, edge.Start);

                        List<Vector3> isIntersecting = new List<Vector3> {edge.Start, edge.End, intersection};
                        isIntersecting = isIntersecting.OrderBy(x => Vector3.Dot(edge.End - edge.Start, x)).ToList();


                        if (intersection != Vector3.zero && isIntersecting[1] == intersection)
                        {
                            tmpIntersections.Add(intersection);
                        }
                    }

                    if (!tmpIntersections.Any()) continue;
                    tmpIntersections = tmpIntersections.OrderBy(x => Vector3.Dot(intersectionDir, x)).ToList();
                    var voronoiEdge = new Edge(tmpIntersections.First(), tmpIntersections.Last());

                    tmpIntersections.Clear();
                    foreach (var edge in face.GetEdges())
                    {
                        Plane p = new Plane(edge.Start, edge.End, new Vector3(10, 10, 10));
                        Vector3 intersection = Vector3.zero;

                        ComputeLinePlaneIntersection(out intersection, intersectionDir, intersectionPoint, p.normal, edge.Start);

                        List<Vector3> isIntersecting = new List<Vector3> {edge.Start, edge.End, intersection};
                        isIntersecting = isIntersecting.OrderBy(x => Vector3.Dot(edge.End - edge.Start, x)).ToList();


                        if (intersection != Vector3.zero && isIntersecting[1] == intersection)
                        {
                            //tmpIntersections.Add((intersection, 'f'));
                            tmpIntersections.Add(intersection);
                        }
                    }

                    if (!tmpIntersections.Any()) continue;
                    tmpIntersections = tmpIntersections.OrderBy(x => Vector3.Dot(intersectionDir, x)).ToList();
                    var meshEdge = new Edge(tmpIntersections.First(), tmpIntersections.Last());

                    Vector3 min1 = new Vector3(Mathf.Min(voronoiEdge.Start.x, voronoiEdge.End.x),
                        Mathf.Min(voronoiEdge.Start.y, voronoiEdge.End.y), Mathf.Min(voronoiEdge.Start.z, voronoiEdge.End.z));

                    Vector3 max1 = new Vector3(Mathf.Max(voronoiEdge.Start.x, voronoiEdge.End.x),
                        Mathf.Max(voronoiEdge.Start.y, voronoiEdge.End.y), Mathf.Max(voronoiEdge.Start.z, voronoiEdge.End.z));

                    Vector3 min2 = new Vector3(Mathf.Min(meshEdge.Start.x, meshEdge.End.x),
                        Mathf.Min(meshEdge.Start.y, meshEdge.End.y), Mathf.Min(meshEdge.Start.z, meshEdge.End.z));

                    Vector3 max2 = new Vector3(Mathf.Max(meshEdge.Start.x, meshEdge.End.x),
                        Mathf.Max(meshEdge.Start.y, meshEdge.End.y), Mathf.Max(meshEdge.Start.z, meshEdge.End.z));


                    Vector3 minIntersection = new Vector3(Math.Max(min1.x, min2.x), Math.Max(min1.y, min2.y), Math.Max(min1.z, min2.z));
                    Vector3 maxIntersection = new Vector3(Math.Min(max1.x, max2.x), Math.Min(max1.y, max2.y), Math.Min(min1.z, min2.z));

                    outputList.Add(minIntersection);
                    outputList.Add(maxIntersection);

                    // face.SortEdges();
                    // var outputFace = face.points.ToList();
                    // face.points.Clear();
                    // for (int i = 0; i < face.points.Count; i++)
                    // {
                    //     Vector3 currentPoint = face.points[i];
                    //     var index = i - 1;
                    //     if (index < 0) index = face.points.Count - 1;
                    //     Vector3 prevPoint = face.points[index];
                    //
                    //
                    //     Vector3 intersection = Vector3.zero;
                    //     var success = ComputeLinePlaneIntersection(out intersection, currentPoint - prevPoint, prevPoint,
                    //         voronoiFace.Plane.normal, voronoiFace.points[0]);
                    //
                    //     var prevInside = !voronoiFace.Plane.GetSide(prevPoint);
                    //     var currentInside = !voronoiFace.Plane.GetSide(currentPoint);
                    //
                    //     if (currentInside)
                    //     {
                    //         if (!prevInside)
                    //         {
                    //             outputFace.Add(intersection);
                    //         }
                    //
                    //         outputFace.Add(currentPoint);
                    //     }
                    //     else if (prevInside)
                    //     {
                    //         outputFace.Add(intersection);
                    //         outputFace.Add(prevPoint);
                    //     }
                    // }
                }
            }

            //outputList.Add(new VoronoiCell(faces));
        }

        return outputList;
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

    private bool ComputeLinePolygonIntersection(out Vector3 intersection, Vector3 prevPoint, Vector3 currentPoint, Triangle triangle)
    {
        if (prevPoint == currentPoint)
        {
            intersection = Vector3.zero;
            return false;
        }

        float tE = 0;
        float tL = 1;

        var ds = currentPoint - prevPoint;

        var plane = new Plane(triangle.V1, triangle.V2, triangle.V3);

        var N = -Vector3.Dot(prevPoint - triangle.V1, plane.normal);

        var D = Vector3.Dot(ds, plane.normal);

        //parallel
        if (D == 0)
        {
            intersection = Vector3.zero;
            return false;
        }

        var t = N / D;

        if (D < 0)
        {
            tE = Mathf.Max(tE, t);
            if (tE > tL)
            {
                intersection = Vector3.zero;
                return false;
            }
        }
        else if (D > 0)
        {
            tL = Mathf.Min(tL, t);
            if (tL < tE)
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        intersection = prevPoint + tE * ds;
        return true;
    }

    private bool IntersectTriangle(out Vector3 intersection, Vector3 prevPoint, Vector3 currentPoint, Triangle triangle)
    {
        intersection = Vector3.zero;
        if (prevPoint == currentPoint)
        {
            return false;
        }

        const float EPSILON = 0.0000001f;


        var edge1 = triangle.V2 - triangle.V1;
        var edge2 = triangle.V3 - triangle.V1;

        Vector3 dir = (currentPoint - prevPoint);
        Vector3 dirNorm = dir.normalized;

        Vector3 h = Vector3.Cross(dirNorm, edge2);

        float a = Vector3.Dot(edge1, h);

        if (a > -EPSILON && a < EPSILON)
        {
            return false;
        }

        Vector3 s = prevPoint - triangle.V1;
        float f = 1.0f / a;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
        {
            return false;
        }

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(dirNorm, q);

        if (v < 0.0f || u + v > 1.0f)
        {
            return false;
        }

        float t = f * Vector3.Dot(edge2, q);

        if (t > EPSILON && t < Mathf.Sqrt(Vector3.Dot(dir, dir)))
        {
            // segment intersection
            intersection = prevPoint + dirNorm * t;

            return true;
        }

        return false;
    }
}

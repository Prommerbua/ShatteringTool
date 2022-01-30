using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoronoiFace
{
    public Plane Plane;
    public List<Vector3> points;


    public VoronoiFace(List<Vector3> points, Plane plane)
    {
        Plane = plane;
        this.points = points;
    }
}

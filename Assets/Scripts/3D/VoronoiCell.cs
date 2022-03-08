using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoronoiCell
{
    public Vector3 Generator;
    public List<VoronoiFace> Faces;

    public VoronoiCell(List<VoronoiFace> faces)
    {
        this.Faces = faces;
        Generator = Vector3.zero;
    }

    public VoronoiCell(Vector3 generator, List<VoronoiFace> faces)
    {
        Generator = generator;
        this.Faces = faces;
    }
}

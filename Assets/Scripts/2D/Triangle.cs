using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Edge2D incEdge;

    public Triangle(Edge2D incEdge)
    {
        this.incEdge = incEdge;
    }

    public bool isBad = false;

}

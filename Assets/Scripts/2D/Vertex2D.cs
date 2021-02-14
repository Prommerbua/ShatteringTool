using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex2D
{
    public int x;
    public int y;
    public Edge2D incEdge;

    public Vertex2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex2D
{
    public Vector2 Pos;
    public Edge2D IncEdge;

    public Vertex2D(int x, int y)
    {
        this.Pos.x = x;
        this.Pos.y = y;
    }

    public Vertex2D(Vector2 pos)
    {
        this.Pos = pos;
    }
}

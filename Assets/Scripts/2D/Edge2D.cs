using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge2D
{
    public Vertex2D org;
    public Edge2D twin;
    public Edge2D prev;
    public Edge2D next;
    public Triangle left;

    public Edge2D(Vertex2D org)
    {
        this.org = org;
    }

}

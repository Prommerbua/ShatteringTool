using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge2D
{
    public Vertex2D org;
    public Edge2D twin;
    public Edge2D prev;
    public Edge2D next;
    public Face left;

    public Edge2D(Vertex2D org)
    {
        this.org = org;
    }

    //get corresponding vertices from triangle
    public Vertex2D[] GetTriangleVertices()
    {
        if (left == null)
        {
            Debug.LogWarning("No Triangle on this Edge");
            return null;
        }
        return new[]
        {
            org, next.org, prev.org
        };
    }

}

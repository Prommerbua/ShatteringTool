using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{

    public DelaunayVertex2D start;
    public DelaunayVertex2D end;

    public Vector3 mid;

    public Triangle left;
    public Triangle right;

    public float f;
    public float g;



    public Edge(DelaunayVertex2D left, DelaunayVertex2D right, Triangle parent)
    {
        this.left = parent;
        start = left;
        end = right;

        //Calculate the line equation for the perpendicula bisector
        float x1 = start.loc.x;
        float y1 = start.loc.y;
        float x2 = end.loc.x;
        float y2 = end.loc.y;
        mid = new Vector3((float) ((x1 + x2) * 0.5), (float) ((y1 + y2) * 0.5));

        f = (x1 - x2) / (y2 - y1);
        g = mid.y - f * mid.x;
    }

    public bool hasVertex(DelaunayVertex2D p)
    {
        return start.Equals(p) || end.Equals(p);
    }

    public void DebugLine(int xoffset)
    {
        Debug.DrawLine((start.loc + new Vector3(xoffset, 0, 0)), (end.loc + new Vector3(xoffset, 0, 0)), Color.red);
    }

    public void DebugLine()
    {
        Debug.DrawLine((start.loc), (end.loc), Color.green, 100f);
    }

    public bool Equals(Edge other)
    {
        return start == other.start && end == other.end || start == other.end && end == other.start;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        return this.Equals((Edge)obj);
    }

    public override int GetHashCode()
    {
        return start.GetHashCode() ^ end.GetHashCode();
    }
}

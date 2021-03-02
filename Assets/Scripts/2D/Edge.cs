using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : IEquatable<Edge>
{
    public Vector2 Start;
    public Vector2 End;


    public Edge(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }

    public static bool operator ==(Edge e1, Edge e2)
    {
        if (e1.End == e2.End && e1.Start == e2.Start) return true;
        else if (e1.Start == e2.End && e1.End == e2.Start) return true;
        else return false;
    }

    public static bool operator !=(Edge e1, Edge e2)
    {
        return !(e1 == e2);
    }

    public bool Equals(Edge other)
    {
        if (End == other.End && Start == other.Start) return true;
        else if (Start == other.End && End == other.Start) return true;
        else return false;
    }
}

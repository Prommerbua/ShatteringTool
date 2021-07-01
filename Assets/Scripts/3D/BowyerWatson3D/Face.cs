using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face: System.IEquatable<Face>{

    public DelaunayVertex p1,p2,p3;

    public Tetrahedron left;
    public Tetrahedron right;

    private int hashCode;
    public Face(DelaunayVertex p1, DelaunayVertex p2, DelaunayVertex p3, Tetrahedron parent)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        left = parent;
        hashCode = p1.GetHashCode() ^ p2.GetHashCode() << 2 ^ p3.GetHashCode() >> 2;
    }

    public void DebugFace(int xoffset, Color color)
    {
        Debug.DrawLine(p1.loc + Vector3.right*xoffset, p2.loc + Vector3.right * xoffset, color, 10f);
        Debug.DrawLine(p2.loc + Vector3.right * xoffset, p3.loc + Vector3.right * xoffset, color, 10f);
        Debug.DrawLine(p3.loc + Vector3.right * xoffset, p1.loc + Vector3.right * xoffset, color, 10f);
    }

    public bool Equals(Face other)
    {
        if (other == null) return false;
        return
            p1 == other.p1 && (p2 == other.p3 && p3 == other.p2 || p2 == other.p2 && p3 == other.p3) ||
            p1 == other.p2 && (p2 == other.p1 && p3 == other.p3 || p2 == other.p3 && p3 == other.p1) ||
            p1 == other.p3 && (p2 == other.p1 && p3 == other.p2 || p2 == other.p2 && p3 == other.p1);
    }

    public override bool Equals(object obj)
    {
        return this.Equals((Face)obj);
    }

    public override int GetHashCode()
    {
        return hashCode;
    }
}

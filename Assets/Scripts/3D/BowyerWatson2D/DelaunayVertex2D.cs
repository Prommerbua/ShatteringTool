using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayVertex2D {

    public static int idCounter = 0;
    public Vector3 loc;

    private List<Triangle> connectedTriangles;

    private int id;
    public DelaunayVertex2D(Vector3 loc)
    {
        id = idCounter++;
        this.loc = loc;
        this.connectedTriangles = new List<Triangle>();
    }

    public DelaunayVertex2D(float x, float y, float z)
    {
        id = idCounter++;
        this.loc = new Vector3(x, y, z);
        this.connectedTriangles = new List<Triangle>();
    }

    public void AddIncidentTetrahedron(Triangle t)
    {
        connectedTriangles.Add(t);
    }

    public void RemoveIncidentTetrahedron(Triangle t)
    {
        connectedTriangles.Remove(t);
    }

    public static bool operator ==(DelaunayVertex2D x, DelaunayVertex2D y)
    {
        return x.id == y.id;
    }
    public static bool operator !=(DelaunayVertex2D x, DelaunayVertex2D y)
    {
        return !(x == y);
    }

    public override bool Equals(object obj)
    {
        return id == ((DelaunayVertex2D)obj).id;
    }

    public override int GetHashCode()
    {
        return id;
    }
}

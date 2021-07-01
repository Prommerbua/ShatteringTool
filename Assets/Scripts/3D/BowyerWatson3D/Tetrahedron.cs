using System;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron: System.IEquatable<Tetrahedron>
{
    public DelaunayVertex p1, p2, p3, p4;
    public List<Face> faces;

    public Vector3 circumceter;
    public double radius;
    public Vector3 centerOfMass;

    public bool bad = false;

    public DelaunayVertex this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return this.p1;
                case 1:
                    return this.p2;
                case 2:
                    return this.p3;
                case 3:
                    return this.p4;
                default:
                    throw new IndexOutOfRangeException("Invalid Tetrahedon index!");
            }
        }
    }

    public Tetrahedron(DelaunayVertex p1, DelaunayVertex p2, DelaunayVertex p3, DelaunayVertex p4)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        this.p4 = p4;

        p1.AddIncidentTetrahedron(this);
        p2.AddIncidentTetrahedron(this);
        p3.AddIncidentTetrahedron(this);
        p4.AddIncidentTetrahedron(this);

        faces = new List<Face>();

        faces.Add(new Face(p1, p2, p3, this));
        faces.Add(new Face(p2, p3, p4, this));
        faces.Add(new Face(p3, p4, p1, this));
        faces.Add(new Face(p4, p1, p2, this));

        calculateCircumsphere();
    }

    public Tetrahedron(DelaunayVertex p1, Face f, Tetrahedron old)
    {
        this.p1 = p1;
        this.p2 = f.p1;
        this.p3 = f.p2;
        this.p4 = f.p3;

        p1.AddIncidentTetrahedron(this);
        p2.AddIncidentTetrahedron(this);
        p3.AddIncidentTetrahedron(this);
        p4.AddIncidentTetrahedron(this);

        faces = new List<Face>();

        faces.Add(new Face(p1, p2, p3, this));
        faces.Add(new Face(p3, p1, p4, this));
        faces.Add(new Face(p1, p2, p4, this));

        if(f.right != null && f.right.bad ||f.right == old)
        {
            f.right = this;
        }
        else if(f.left != null && f.left.bad || f.left == old)
        {
            f.left = this;
        }

        faces.Add(f);
        calculateCircumsphere();
    }

    //Calculate the circumcenter and the radius of circumsphere.
    private void calculateCircumsphere()
    {
        Vector3 v1 = p2.loc - p1.loc;
        Vector3 v2 = p3.loc - p1.loc;
        Vector3 v3 = p4.loc - p1.loc;


        float l1 = v1.sqrMagnitude;
        float l2 = v2.sqrMagnitude;
        float l3 = v3.sqrMagnitude;

        circumceter = p1.loc + (l1 * Vector3.Cross(v2, v3) + l2 * Vector3.Cross(v3, v1) + l3 * Vector3.Cross(v1, v2)) / (2 * Vector3.Dot(v1, Vector3.Cross(v2, v3)));
        radius = (p1.loc - circumceter).sqrMagnitude;
        circumceter = circumceter * 1000;
        circumceter = new Vector3((float) ((int)circumceter.x / 1000.0), (float) ((int)circumceter.y / 1000.0), (float) ((int)circumceter.z / 1000.0));

        centerOfMass = (p1.loc + p2.loc + p3.loc + p4.loc) / 4;
    }

    public void removeFromVertices()
    {
        p1.RemoveIncidentTetrahedron(this);
        p2.RemoveIncidentTetrahedron(this);
        p3.RemoveIncidentTetrahedron(this);
        p4.RemoveIncidentTetrahedron(this);
    }

    //Find neighbours of the tetrahedon
    public List<Tetrahedron> getNeighbors()
    {
        List<Tetrahedron> neighbors = new List<Tetrahedron>();
        foreach(Face f in faces){
            if (f.left == this && f.right != null) neighbors.Add(f.right);
            else if (f.right == this && f.left != null) neighbors.Add(f.left);
        }
        return neighbors;
    }

    //Replace face. f and faces[i] have same values but different memory adresses
    public void replaceFace(Face f)
    {
        for(int i = 0; i < 4; i++)
        {
            if (f.Equals(faces[i]))
            {
                faces[i] = f;
                break;
            }
        }
    }

    public bool hasFace(Face e)
    {
        if (faces[0].Equals(e)) return true;
        if (faces[1].Equals(e)) return true;
        if (faces[2].Equals(e)) return true;
        if (faces[3].Equals(e)) return true;
        return false;
    }

    public bool hasVertex(DelaunayVertex v)
    {
        return Equals(p1, v) || Equals(p2, v) || Equals(p3, v) || Equals(p4, v);
    }

    //Is the new vertex within the sircumsphare
    public bool withinRange(DelaunayVertex p)
    {
        return radius >= (p.loc - circumceter).sqrMagnitude;
    }

    //Do this and the other tetrahedon share a face
    public Face shareFace(Tetrahedron t)
    {
        for(int i = 0; i < 4; i++)
        {
            if (t.hasFace(faces[i]) && faces[i].right == null)
            {
                return faces[i];
            }
        }
        return null;
    }

    public void DebugTetra(int xoffset, Color color)
    {
        for(int i = 0; i < faces.Count; i++)
        {
            faces[i].DebugFace(xoffset, color);
        }
    }

    public void DebugVoronoi()
    {
        for(int i = 0; i < 4; i++)
        {
            Vector3 point = new Vector3();
            if(faces[i].left == this && faces[i].right != null)
            {
                point = faces[i].right.circumceter;
            }
            else if(faces[i].left != null && faces[i].right == this)
            {
                point = faces[i].left.circumceter;
            }else
            {
                continue;
            }

            Debug.DrawLine(circumceter, point, Color.green);

        }
    }

    public bool Equals(Tetrahedron other)
    {
        if (other == null) return false;
        return
            p1 == other.p1 && (
                p2 == other.p2 && ( p3 == other.p3 && p4 == other.p4 || p3 == other.p4 && p4 == other.p3 ) ||
                p2 == other.p3 && ( p3 == other.p2 && p4 == other.p4 || p3 == other.p4 && p4 == other.p2 ) ||
                p2 == other.p4 && ( p3 == other.p2 && p4 == other.p3 || p3 == other.p3 && p4 == other.p2 )
            ) ||

            p1 == other.p2 && (
                p2 == other.p1 && ( p3 == other.p3 && p4 == other.p4 || p3 == other.p4 && p4 == other.p3 ) ||
                p2 == other.p3 && ( p3 == other.p1 && p4 == other.p4 || p3 == other.p4 && p4 == other.p1 ) ||
                p2 == other.p4 && ( p3 == other.p1 && p4 == other.p3 || p3 == other.p3 && p4 == other.p1 )
            ) ||

            p1 == other.p3 && (
                p2 == other.p1 && ( p3 == other.p2 && p4 == other.p4 || p3 == other.p4 && p4 == other.p2 ) ||
                p2 == other.p2 && ( p3 == other.p1 && p4 == other.p4 || p3 == other.p4 && p4 == other.p1 ) ||
                p2 == other.p4 && ( p3 == other.p1 && p4 == other.p2 || p3 == other.p2 && p4 == other.p1 )
            ) ||

            p1 == other.p4 && (
                p2 == other.p1 && ( p3 == other.p2 && p4 == other.p3 || p3 == other.p3 && p4 == other.p2 ) ||
                p2 == other.p2 && ( p3 == other.p1 && p4 == other.p3 || p3 == other.p3 && p4 == other.p1 ) ||
                p2 == other.p3 && ( p3 == other.p1 && p4 == other.p2 || p3 == other.p2 && p4 == other.p1 )
            );
    }

    public override bool Equals(object obj)
    {
        return Equals((Tetrahedron)obj);
    }

    public override int GetHashCode()
    {
        return p1.GetHashCode() ^ p2.GetHashCode() << 2 ^ p3.GetHashCode() >> 2 ^ p4.GetHashCode() >> 1;
    }
}

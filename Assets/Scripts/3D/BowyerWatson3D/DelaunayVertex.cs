using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DelaunayVertex {

    public Vector3 loc;
    public BowyerWatson bw;

    public VoronoiCell cell;
    public List<Tetrahedron> connectedTetrahedrons;

    private int hashCode;
	public DelaunayVertex(Vector3 loc)
    {
        this.loc = loc;
        this.connectedTetrahedrons = new List<Tetrahedron>();

        hashCode = loc.x.GetHashCode() ^ loc.y.GetHashCode() << 2 ^ loc.z.GetHashCode() >> 2;
    }

    public DelaunayVertex(float x, float y, float z)
    {
        this.loc = new Vector3(x, y, z);
        this.connectedTetrahedrons = new List<Tetrahedron>();

        hashCode = loc.x.GetHashCode() ^ loc.y.GetHashCode() << 2 ^ loc.z.GetHashCode() >> 2;
    }

    public void AddIncidentTetrahedron(Tetrahedron t)
    {
        connectedTetrahedrons.Add(t);
    }

    public void RemoveIncidentTetrahedron(Tetrahedron t)
    {
        for(int i = 0; i < connectedTetrahedrons.Count; i++)
        {
            if (connectedTetrahedrons[i].Equals(t))
            {
                connectedTetrahedrons.RemoveAt(i);
                break;
            }
        }
    }

    //Find the Voronoi cell correspondinc to the vertex in Delaunay triangulation
    public void CalculateVoronoiCell()
    {
        List<DelaunayVertex> neighbours = new List<DelaunayVertex>();
        List<VoronoiFace> faces = new List<VoronoiFace>();

        //Find neighburing vertices
        foreach(Tetrahedron t in connectedTetrahedrons)
        {
            neighbours.Add(t.p1);
            neighbours.Add(t.p2);
            neighbours.Add(t.p3);
            neighbours.Add(t.p4);
        }
        neighbours = neighbours.Distinct().ToList();
        neighbours.Remove(this);


        foreach(DelaunayVertex n in neighbours)
        {
            //Calculate bisecting plane between the two vertices
            Vector3 dir = (n.loc - loc).normalized;
            Vector3 mid = (n.loc + loc) / 2;

            Plane p = new Plane(dir, mid);

            List<Vector3> VoronoiVertices = new List<Vector3>();

            //Find all the vertices that are on the plane
            foreach(Tetrahedron t in connectedTetrahedrons)
            {
                if(Math.Abs(p.GetDistanceToPoint(t.circumceter)) < 0.01)
                {
                    VoronoiVertices.Add(t.circumceter-loc);
                }
            }

            //Found vertices form a Voronoi face
            faces.Add(new VoronoiFace(VoronoiVertices, p));
        }

        //Create or update the Voronoi cell
        if (cell == null)
        {
            cell = new VoronoiCell(this, neighbours, faces);
        }else
        {
            cell.UpdateObject(neighbours, faces);
        }
    }

    //Find the neigbouring Delaunay vertices
    public List<DelaunayVertex> getNeighbours()
    {
        List<DelaunayVertex> neighbours = new List<DelaunayVertex>();
        foreach (Tetrahedron t in connectedTetrahedrons)
        {
            neighbours.Add(t.p1);
            neighbours.Add(t.p2);
            neighbours.Add(t.p3);
            neighbours.Add(t.p4);
        }
        neighbours = neighbours.Distinct().ToList();
        neighbours.Remove(this);

        return neighbours;
    }

    public void CreateCell()
    {
        cell.CreateObject();
    }

    public void DrawCell()
    {
        foreach(Tetrahedron t in connectedTetrahedrons)
        {
            if (t.shareFace(bw.super_tetrahedron) != null) continue;
            t.DebugVoronoi();
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        DelaunayVertex dv = (DelaunayVertex)obj;
        return (loc - dv.loc).sqrMagnitude < 0.001;
    }

    public override int GetHashCode()
    {
        return hashCode;
    }
}

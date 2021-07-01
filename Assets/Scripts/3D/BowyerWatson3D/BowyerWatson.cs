using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

public class BowyerWatson{

    public Tetrahedron super_tetrahedron;
    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> controlPoints = new List<Vector3>();
    public  List<DelaunayVertex> vertices = new List<DelaunayVertex>();

    private List<Tetrahedron> badTerahedrons = new List<Tetrahedron>();
    private Queue<Tetrahedron> queue = new Queue<Tetrahedron>();
    private HashSet<Tetrahedron> inQueue = new HashSet<Tetrahedron>();
    private HashSet<Tetrahedron> checke = new HashSet<Tetrahedron>();

    private List<Tetrahedron> newTetrahedrons = new List<Tetrahedron>();

    int i = 0;
    bool done = false;

    public List<Tetrahedron> triangulation;

    public BowyerWatson(int count)
    {
        generatePoint(count);
        createSuperTetrahedron();


        triangulation = new List<Tetrahedron>();
        triangulation.Add(super_tetrahedron);
    }

    public BowyerWatson(List<Vector3> points, MeshFilter meshfilter)
    {
        this.points = points;
        createSuperTetrahedron(meshfilter);


        triangulation = new List<Tetrahedron>();
        triangulation.Add(super_tetrahedron);
    }

    public void generatePoint(int count)
    {
        UnityEngine.Random.InitState(1);
        for (int i = 0; i < count; i++)
        {
            points.Add(new Vector3((float) (UnityEngine.Random.Range(0, 100) / 10d), (float) (UnityEngine.Random.Range(0, 100) / 10d), (float) (UnityEngine.Random.Range(0, 100) / 10d)));
        }
    }


    public void createSuperTetrahedron()
    {
        DelaunayVertex p1 = new DelaunayVertex(-100, -50, -100);
        DelaunayVertex p2 = new DelaunayVertex(100, -50, -100);
        DelaunayVertex p3 = new DelaunayVertex(0, -50, 100);
        DelaunayVertex p4 = new DelaunayVertex(0, 100, 0);

        super_tetrahedron = new Tetrahedron(p1, p2, p3, p4);


        // DelaunayVertex p1 = new DelaunayVertex(-2000, -1000, -2000);
        // DelaunayVertex p2 = new DelaunayVertex(2000, -1000, -2000);
        // DelaunayVertex p3 = new DelaunayVertex(0, -1000, 2000);
        // DelaunayVertex p4 = new DelaunayVertex(0, 2000, 0);
        //
        // super_tetrahedron = new Tetrahedron(p1, p2, p3, p4);
    }

    public void createSuperTetrahedron(MeshFilter meshFilter)
    {

        DelaunayVertex p1 = new DelaunayVertex(-20, -10, -20);
        DelaunayVertex p2 = new DelaunayVertex(20, -10, -20);
        DelaunayVertex p3 = new DelaunayVertex(0, -10, 20);
        DelaunayVertex p4 = new DelaunayVertex(0, 20, 0);

        super_tetrahedron = new Tetrahedron(p1, p2, p3, p4);


        // DelaunayVertex p1 = new DelaunayVertex(-2000, -1000, -2000);
        // DelaunayVertex p2 = new DelaunayVertex(2000, -1000, -2000);
        // DelaunayVertex p3 = new DelaunayVertex(0, -1000, 2000);
        // DelaunayVertex p4 = new DelaunayVertex(0, 2000, 0);
        //
        // super_tetrahedron = new Tetrahedron(p1, p2, p3, p4);
    }

    //Points used for benchmarking
    /*   public void generatePoint(int count)
       {
           for (int i = 0; i < count; i++)
           {
               points.Add(new DoubleVector(UnityEngine.Random.Range(-350, 350), UnityEngine.Random.Range(-50, 300), UnityEngine.Random.Range(-350, 350)));
           }

           int controlCOunt = 20;
           for (int i = 0; i < controlCOunt; i++)
           {
               controlPoints.Add(new DoubleVector(UnityEngine.Random.Range(-400, 400), -100, UnityEngine.Random.Range(-400, 400)));
           }
           for (int i = 0; i < controlCOunt; i++)
           {
               controlPoints.Add(new DoubleVector(UnityEngine.Random.Range(-400, 400), 400, UnityEngine.Random.Range(-400, 400)));
           }


           for (int i = 0; i < controlCOunt; i++)
           {
               controlPoints.Add(new DoubleVector(UnityEngine.Random.Range(-400, 400), UnityEngine.Random.Range(-100, 400), -400));
           }

           for (int i = 0; i < controlCOunt; i++)
           {
               controlPoints.Add(new DoubleVector(UnityEngine.Random.Range(-400, 400), UnityEngine.Random.Range(-100, 400), 400));
           }

           for (int i = 0; i < controlCOunt; i++)
           {
               controlPoints.Add(new DoubleVector(-400, UnityEngine.Random.Range(-100, 400), UnityEngine.Random.Range(-400, 400)));
           }

           for (int i = 0; i < controlCOunt; i++)
           {
               controlPoints.Add(new DoubleVector(400, UnityEngine.Random.Range(-100, 400), UnityEngine.Random.Range(-400, 400)));
           }

           points.AddRange(controlPoints);

       }
       */


    public void MakeDiagram()
    {
        while (!done)
        {
            DoStep();
        }
    }

    public bool DoStep(bool badTetrahedonsFound = false)
    {
        if (i < points.Count)
        {
            if (vertices.FindIndex(x => (x.loc - points[i]).sqrMagnitude < 0.001) != -1)
            {
                points.RemoveAt(i);
                return false;
            }
            vertices.Add(new DelaunayVertex(points[i]));
            vertices[i].bw = this;

            if (i == 0)
            {
                badTerahedrons.Add(super_tetrahedron);
                super_tetrahedron.bad = true;
                triangulation.RemoveAt(0);
            }
            else if (badTetrahedonsFound)
            {
                //If the bad tetrahedons were found before just remove them
                badTerahedrons.ForEach(x => x.bad = true);
                triangulation.RemoveAll(x => x.bad);
            }
            else
            {
                //Find bad tetrahedons
                badTerahedrons.Clear();
                queue.Clear();
                inQueue.Clear();
                checke.Clear();
                for (int j = triangulation.Count - 1; j >= 0; j--)
                {
                    if (triangulation[j].withinRange(vertices[i]))
                    {
                        queue.Enqueue(triangulation[j]);
                        break;
                    }
                }

                //      queue.Enqueue(LocateBadTetraHedron(vertices[i], triangulation[0]));

                while (queue.Count > 0)
                {
                    Tetrahedron t = queue.Dequeue();
                    if (t.withinRange(vertices[i]))
                    {
                        t.bad = true;
                        badTerahedrons.Add(t);

                        foreach (Tetrahedron tet in t.getNeighbors())
                        {

                            if (!checke.Contains(tet) && !inQueue.Contains(tet))
                            {
                                queue.Enqueue(tet);
                                inQueue.Add(tet);
                            }
                        }
                    }
                    checke.Add(t);
                }
            }
            triangulation.RemoveAll(t => t.bad);


            newTetrahedrons = new List<Tetrahedron>();
            //Process bad tetrahedons
            for (int j = 0; j < badTerahedrons.Count; j++)
            {
                Tetrahedron temp = badTerahedrons[j];
                temp.removeFromVertices();

                for (int k = 0; k < 4; k++)
                {
                    Face e = temp.faces[k];
                    temp.faces[k] = null;

                    //If a face of bad terahedon is no between two bad tetrahedons then create new tetrahedon
                    if (e.right == null || !e.left.bad || !e.right.bad)
                    {

                        Tetrahedron t = new Tetrahedron(vertices[i], e, temp);
                        for (int h = 0; h < newTetrahedrons.Count; h++)
                        {
                            Face f = newTetrahedrons[h].shareFace(t);
                            if (f != null)
                            {

                                t.replaceFace(f);

                                if (object.ReferenceEquals(f.left, newTetrahedrons[h]))
                                {
                                    f.right = t;
                                }
                                else if (object.ReferenceEquals(f.right, newTetrahedrons[h]))
                                {
                                    f.left = t;
                                }
                            }
                        }
                        triangulation.Add(t);
                        newTetrahedrons.Add(t);
                    }
                }
            }
            i++;

            return true;
        }
        else
        {
            // triangulation.RemoveAll(t =>
            //     t.hasVertex(super_tetrahedron.p1) || t.hasVertex(super_tetrahedron.p2) || t.hasVertex(super_tetrahedron.p3) ||
            //     t.hasVertex(super_tetrahedron.p4));

            done = true;
        }
        return false;
    }

    public void  AddPoints(List<Vector3> points, DelaunayVertex struckObject)
    {
        //For every new point added, check if they form edges with vertices whose cells are destoryed.
        //If yes then discard the point.
        //Else add it to triangulation
        foreach(Vector3 dv in points)
        {
            DelaunayVertex v = new DelaunayVertex(dv);
            getBadTetrahedrons(v, struckObject);
            HashSet<DelaunayVertex> thpoints = new HashSet<DelaunayVertex>();
            for (int j = 0; j < badTerahedrons.Count; j++)
            {
                thpoints.Add(badTerahedrons[j].p1);
                thpoints.Add(badTerahedrons[j].p2);
                thpoints.Add(badTerahedrons[j].p3);
                thpoints.Add(badTerahedrons[j].p4);
            }
            thpoints.Remove(struckObject);
            if (!thpoints.Any(x => x.cell != null && x.cell.destroyed))
            {
                this.points.Add(dv);
                DoStep(true);
            }
        }
    }



    //Find all tetrahedons whose circumcphere contains the new point
    private void getBadTetrahedrons(DelaunayVertex point, DelaunayVertex struck)
    {
        badTerahedrons.Clear();
        queue.Clear();
        inQueue.Clear();
        checke.Clear();
        queue.Enqueue(LocateBadTetraHedron(point, struck.connectedTetrahedrons[0]));
        while (queue.Count > 0)
        {
            Tetrahedron t = queue.Dequeue();
            checke.Add(t);
            if (t.withinRange(point))
            {
                badTerahedrons.Add(t);
                foreach (Tetrahedron tet in t.getNeighbors())
                {
                    if (!inQueue.Contains(tet) && !checke.Contains(tet))
                    {
                        queue.Enqueue(tet);
                        inQueue.Add(tet);
                    }
                }
            }
        }
    }



    HashSet<Face> jumpedFaces = new HashSet<Face>();
    //Walking algorithm for bad terahedon localization
    public Tetrahedron LocateBadTetraHedron(DelaunayVertex vertex, Tetrahedron start)
    {
        Tetrahedron current = start;
        jumpedFaces.Clear();
        while (!current.withinRange(vertex))
        {
            Face closest = null;
            double minDistance = double.MaxValue;
            foreach(Face f in current.faces)
            {
                if (!jumpedFaces.Contains(f) && onTheRight(current,f, vertex)){
                    double dist = distToFace(f, vertex);
                    if(dist < minDistance)
                    {
                        closest = f;
                        minDistance = dist;
                    }
                }
            }

            if(closest == null)
            {
                foreach(Face f in current.faces)
                {
                    if (!jumpedFaces.Contains(f))
                    {
                        closest = f;
                        break;
                    }
                }
            }

            jumpedFaces.Add(closest);

            if (closest.left == current) current = closest.right;
            else current = closest.left;

        }
        return current;
    }

    //Point plane distance
    public double distToFace(Face f, DelaunayVertex v)
    {
        Vector3 n = Vector3.Cross(f.p1.loc - f.p2.loc, f.p3.loc - f.p2.loc).normalized;

        return Math.Abs(Vector3.Dot(n, v.loc - f.p2.loc));
    }

    //Is the point on the right of the face
    public bool onTheRight(Tetrahedron t, Face f, DelaunayVertex v)
    {
        Vector3 n = Vector3.Cross(f.p1.loc - f.p2.loc, f.p3.loc - f.p2.loc);
        double s = Vector3.Dot(n,t.centerOfMass - f.p1.loc);

        if (s > 0) n *= -1;
        s = Vector3.Dot(n, v.loc - f.p1.loc);

        if (s > 0) return true;
        return false;
    }

    public void DrawTriangulation()
    {
            for (int k = 0; k < triangulation.Count; k++)
            {
                    triangulation[k].DebugTetra(0, Color.red);
            }

    }

    public void DrawVoronoi()
    {
        for(int k = 0; k < vertices.Count; k++)
        {
            if (!controlPoints.Contains(vertices[k].loc))
            {
                vertices[k].DrawCell();
            }

        }
    }

    public void DrawPoints()
    {
        foreach(Vector3 p in points)
        {
            Debug.DrawRay(p, Vector3.up, Color.cyan, 100f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BowyerWatson2D
{
    public Triangle super_triangle;
    public List<Vector3> points;
    public List<Triangle> triangulation;
    public List<DelaunayVertex2D> vertices;


    private List<Triangle> badTriangles = new List<Triangle>();


    private int i = 0;
    private bool done = false;

    private Quaternion rotQuat;
    private Quaternion inverseQuat;

    public BowyerWatson2D(List<Vector3> points, Plane plane, Vector3 center)
    {
        this.points = points;
        CreateRotationMatrix(plane);
        generateSuperTriangle(center);
        triangulation = new List<Triangle>(20);
        vertices = new List<DelaunayVertex2D>();
        triangulation.Add(super_triangle);
    }

    //Creates rotation matrix to rotate all the points on xy-plane
    private void CreateRotationMatrix(Plane p)
    {
        Vector3 n = p.normal;

        double cosA = Vector3.Dot(p.normal, new Vector3(0,0,1));
        rotQuat = Quaternion.AngleAxis((float) Math.Acos(cosA), Vector3.Cross(n, new Vector3(0, 0, 1))).normalized;
        var angle = (float) Math.Acos(cosA);
        var axis = Vector3.Cross(n, new Vector3(0, 0, 1)).normalized;

        rotQuat = new Quaternion((float) (Math.Sin(angle / 2) * axis.x), (float) (Math.Sin(angle / 2) * axis.y), (float) (Math.Sin(angle / 2) * axis.z), (float) Math.Cos(angle / 2));
        inverseQuat = new Quaternion(-rotQuat.x,-rotQuat.y,-rotQuat.z,rotQuat.w);

    }

    //Generates the initial super triangle to initilaize the process
    private void generateSuperTriangle(Vector3 center)
    {
        Vector3 moved = rotQuat * center;
        // DelaunayVertex2D p1 = new DelaunayVertex2D(new Vector3(0,1,0) * 3000 + moved);
        // DelaunayVertex2D p2 = new DelaunayVertex2D(Quaternion.AngleAxis((float) (2*Math.PI / 3), new Vector3(0, 0, 1)) * new Vector3(0, 1, 0) * 3000f + moved);
        // DelaunayVertex2D p3 = new DelaunayVertex2D(Quaternion.AngleAxis((float) (-2*Math.PI / 3), new Vector3(0, 0, 1)) * new Vector3(0, 1, 0) * 3000f + moved);

        DelaunayVertex2D p1 = new DelaunayVertex2D(new Vector3(0,1,0) * 3000 + moved);

        float angle = (float) (2*Math.PI / 3);
        var axis = new Vector3(0, 0, 1);
        Quaternion q = new Quaternion((float) (Math.Sin(angle / 2) * axis.x), (float) (Math.Sin(angle / 2) * axis.y),
            (float) (Math.Sin(angle / 2) * axis.z), (float) Math.Cos(angle / 2));
        DelaunayVertex2D p2 = new DelaunayVertex2D(q * new Vector3(0, 1, 0) * 3000f + moved);

        angle = (float) (-2*Math.PI / 3);
        q = new Quaternion((float) (Math.Sin(angle / 2) * axis.x), (float) (Math.Sin(angle / 2) * axis.y),
            (float) (Math.Sin(angle / 2) * axis.z), (float) Math.Cos(angle / 2));
        DelaunayVertex2D p3 = new DelaunayVertex2D(q * new Vector3(0, 1, 0) * 3000f + moved);

        super_triangle = new Triangle(p1, p2, p3);
    }

    public void MakeDiagram()
    {
        while (!done)
        {
            doStep();
        }
    }

    private bool doStep()
    {
        if (i < points.Count)
        {
            //Rotate point to xy-plane
            vertices.Add(new DelaunayVertex2D(rotQuat * points[i]));
            badTriangles.Clear();
            //Find all bad triangles
            for (int j = triangulation.Count - 1; j >= 0; j--)
            {
                if (triangulation[j].withinRange(vertices[i]))
                {
                    triangulation[j].bad = true;
                    badTriangles.Add(triangulation[j]);
                    triangulation.RemoveAt(j);
                }
            }

            List<Triangle> newTriangles = new List<Triangle>();
            //Process bad triangles
            for (int j = 0; j < badTriangles.Count; j++)
            {
                Triangle temp = badTriangles[j];
                for (int k = 0; k < 3; k++)
                {
                    Edge e = temp.edges[k];

                    //If edge of a bad triangle is not between two bad triangles, create new triangle with it
                    if (e.left == null || !e.left.bad || e.right == null || !e.right.bad)
                    {
                        Triangle t = new Triangle(vertices[i], e, temp);

                        //Find the neighbours of the new triangle
                        for (int h = 0; h < newTriangles.Count; h++)
                        {
                            Edge f = newTriangles[h].shareEdge(t);
                            if (f != null)
                            {

                                t.replaceEdge(f);
                                f.right = t;

                            }
                        }
                        triangulation.Add(t);
                        newTriangles.Add(t);
                    }
                }
            }
            i++;
            return true;
        }
        else
        {

            //Remove triangles connected with te vertices of the super triangle
            triangulation.RemoveAll(t => t.hasVertex(super_triangle.p1) || t.hasVertex(super_triangle.p2) ||
                    t.hasVertex(super_triangle.p3));

            //Undo the rotation to xy-plane
            foreach (DelaunayVertex2D v in vertices)
            {
                v.loc = inverseQuat * v.loc;
            }
            done = true;
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using Habrador_Computational_Geometry;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public static class SutherlandHodgman
{
    //Assumes the polygons are oriented counter clockwise
    //poly_1 is the polygon we want to cut
    //Assumes the polygon we want to remove from the other polygon is convex, so poly_2 has to be convex
    //We will end up with the intersection of the polygons
    public static List<Vector3> ClipPolygon(List<Vector3> poly_1, List<Vector3> poly_2)
    {
        //Calculate the clipping planes
        List<Plane3> clippingPlanes = new List<Plane3>();

        for (int i = 0; i < poly_2.Count; i++)
        {
            int iPlusOne = MathUtility.ClampListIndex(i + 1, poly_2.Count);

            Vector3 v1 = poly_2[i];
            Vector3 v2 = poly_2[iPlusOne];

            //Doesnt have to be center but easier to debug
            Vector3 planePos = (v1 + v2) * 0.5f;

            Vector3 planeDir = v2 - v1;

            //Should point inwards
            Vector3 planeNormal = new Vector3(-planeDir.z, 0f, planeDir.x).normalized;

            //Gizmos.DrawRay(planePos, planeNormal * 0.1f);

            clippingPlanes.Add(new Plane3(planePos, planeNormal));
        }



        List<Vector3> vertices = ClipPolygon(poly_1, clippingPlanes);

        return vertices;
    }



    //Sometimes its more efficient to calculate the planes once before we call the method
    //if we want to cut several polygons with the same planes
    public static List<Vector3> ClipPolygon(List<Vector3> poly_1, List<Plane3> clippingPlanes)
    {
        //Clone the vertices because we will remove vertices from this list
        List<Vector3> vertices = new List<Vector3>(poly_1);

        //Save the new vertices temporarily in this list before transfering them to vertices
        List<Vector3> vertices_tmp = new List<Vector3>();


        //Clip the polygon
        for (int i = 0; i < clippingPlanes.Count; i++)
        {
            Plane3 plane = clippingPlanes[i];

            for (int j = 0; j < vertices.Count; j++)
            {
                int jPlusOne = MathUtility.ClampListIndex(j + 1, vertices.Count);

                Vector3 v1 = vertices[j];
                Vector3 v2 = vertices[jPlusOne];

                //Calculate the distance to the plane from each vertex
                //This is how we will know if they are inside or outside
                //If they are inside, the distance is positive, which is why the planes normals have to be oriented to the inside
                float dist_to_v1 = _Geometry.GetSignedDistanceFromPointToPlane(v1, plane);
                float dist_to_v2 = _Geometry.GetSignedDistanceFromPointToPlane(v2, plane);

                //Case 1. Both are outside (= to the right), do nothing

                //Case 2. Both are inside (= to the left), save v2
                if (dist_to_v1 > 0f && dist_to_v2 > 0f)
                {
                    vertices_tmp.Add(v2);
                }
                //Case 3. Outside -> Inside, save intersection point and v2
                else if (dist_to_v1 < 0f && dist_to_v2 > 0f)
                {
                    Vector3 rayDir = (v2 - v1).normalized;

                    Vector3 intersectionPoint = _Intersections.GetRayPlaneIntersectionCoordinate(plane.pos, plane.normal, v1, rayDir);

                    vertices_tmp.Add(intersectionPoint);

                    vertices_tmp.Add(v2);
                }
                //Case 4. Inside -> Outside, save intersection point
                else if (dist_to_v1 > 0f && dist_to_v2 < 0f)
                {
                    Vector3 rayDir = (v2 - v1).normalized;

                    Vector3 intersectionPoint = _Intersections.GetRayPlaneIntersectionCoordinate(plane.pos, plane.normal, v1, rayDir);

                    vertices_tmp.Add(intersectionPoint);
                }
            }

            //Add the new vertices to the list of vertices
            vertices.Clear();

            vertices.AddRange(vertices_tmp);

            vertices_tmp.Clear();
        }

        return vertices;
    }


}

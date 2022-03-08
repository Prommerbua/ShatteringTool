using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A collection of classes to make the methods more general
namespace Habrador_Computational_Geometry
{
    //Catmull-Rom
    public class CatmullRom : _Curve
    {
        //Start and end point
        public Vector3 posA;
        public Vector3 posB;
        //Handles connected to the start and end points
        public Vector3 handleA;
        public Vector3 handleB;

        public CatmullRom(Vector3 posA, Vector3 posB, Vector3 handleA, Vector3 handleB)
        {
            this.posA = posA;
            this.posB = posB;

            this.handleA = handleA;
            this.handleB = handleB;
        }



        //
        // Position
        //

        public override Vector3 GetPosition(float t)
        {
            Vector3 interpolatedValue = GetPosition(posA, posB, handleA, handleB, t);

            return interpolatedValue;
        }

        //The curve has a start and end point (p1 and p2), and the shape is determined also by two handle points (p0 and p3)
        //The difference from the bezier case is that you can make it so the curve is going through these handle points
        //So if you have a set of points and want a smooth path between these points, you don't have to bother with handles
        //to determine the shape of the curve
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        public static Vector3 GetPosition(Vector3 posA, Vector3 posB, Vector3 handleA, Vector3 handleB, float t)
        {
            Vector3 p0 = handleA;
            Vector3 p1 = posA;
            Vector3 p2 = posB;
            Vector3 p3 = handleB;

            t = Mathf.Clamp01(t);

            //The coefficients of the cubic polynomial (except the 0.5f * which is added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 interpolatedPos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return interpolatedPos;
        }



        //
        // Derivative
        //

        public override float GetDerivative(float t)
        {
            Vector3 derivativeVec = GetDerivativeVec(posA, posB, handleA, handleB, t);

            float derivative = Vector3.Magnitude(derivativeVec);

            return derivative;
        }

        public static Vector3 GetDerivativeVec(Vector3 posA, Vector3 posB, Vector3 handleA, Vector3 handleB, float t)
        {
            Vector3 p0 = handleA;
            Vector3 p1 = posA;
            Vector3 p2 = posB;
            Vector3 p3 = handleB;

            t = Mathf.Clamp01(t);

            //The coefficients of the cubic polynomial (except the 0.5f * which is added later for performance)
            //MyVector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //Position is: a + b * t + c * t^2 + d * t^3
            //Derivative: b + t*2c + t^2 * 3d

            Vector3 derivativeVec = b + t * 2f * c + t * t * 3f * d;

            return derivativeVec;
        }

        public static Vector3 GetSecondDerivativeVec(Vector3 posA, Vector3 posB, Vector3 handleA, Vector3 handleB, float t)
        {
            Vector3 p0 = handleA;
            Vector3 p1 = posA;
            Vector3 p2 = posB;
            Vector3 p3 = handleB;

            t = Mathf.Clamp01(t);

            //The coefficients of the cubic polynomial (except the 0.5f * which is added later for performance)
            //MyVector3 a = 2f * p1;
            //MyVector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //Position is: a + b * t + c * t^2 + d * t^3
            //Derivative: b + t*2c + t^2 * 3d
            //Second derivative: 2c + 2*t*3d = 2c + t * 6d

            Vector3 derivativeVec = 2f * c + t * 6f * d;

            return derivativeVec;
        }

        public override Vector3 GetSecondDerivativeVec(float t)
        {
            return GetSecondDerivativeVec(posA, posB, handleA, handleB, t);
        }



        //
        // Tangent
        //

        public static Vector3 GetTangent(Vector3 posA, Vector3 posB, Vector3 handleA, Vector3 handleB, float t)
        {
            //The tangent is also the derivative vector
            Vector3 tangent = Vector3.Normalize(GetDerivativeVec(posA, posB, handleA, handleB, t));

            return tangent;
        }

        public override Vector3 GetTangent(float t)
        {
            Vector3 tangent = GetTangent(posA, posB, handleA, handleB, t);

            return tangent;
        }
    }
}

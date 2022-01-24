using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A collection of classes to make the methods more general
namespace Habrador_Computational_Geometry
{
    //Bezier with one handle
    public class BezierQuadratic : _Curve
    {
        //Start and end point
        public Vector3 posA;
        public Vector3 posB;
        //Handle connected to start and end points
        public Vector3 handlePos;


        public BezierQuadratic(Vector3 posA, Vector3 posB, Vector3 handlePos)
        {
            this.posA = posA;
            this.posB = posB;
            
            this.handlePos = handlePos;
        }


        //
        // Position at point t
        //

        public override Vector3 GetPosition(float t)
        {
            Vector3 interpolatedValue = GetPosition(posA, posB, handlePos, t);

            return interpolatedValue;
        }

        //Uses de Casteljau's algorithm 
        public static Vector3 GetPosition(Vector3 posA, Vector3 posB, Vector3 handlePos, float t)
        {
            t = Mathf.Clamp01(t);

            //MyVector3 interpolation_posA_handlePos = BezierLinear.GetPosition(posA, handlePos, t);
            //MyVector3 interpolation_handlePos_posB = BezierLinear.GetPosition(handlePos, posB, t);

            //MyVector3 finalInterpolation = BezierLinear.GetPosition(interpolation_posA_handlePos, interpolation_handlePos_posB, t);

            //Above can be simplified by putting it into one big equation
            //Layer 1
            //(1-t)A + tB = A - At + Bt
            //(1-t)B + tC = B - Bt + Ct

            //Layer 2
            //(1-t)(A - At + Bt) + t(B - Bt + Ct)
            //A - At + Bt - At + At^2 - Bt^2 + Bt - Bt^2 + Ct^2
            //A - 2At + 2Bt + At^2 - 2Bt^2 + Ct^2 
            //A - t(2(A - B)) + t^2(A - 2B + C)

            Vector3 A = posA;
            Vector3 B = handlePos;
            Vector3 C = posB;

            Vector3 finalInterpolation = A;

            finalInterpolation += -t * (2f * (A - B));

            //t^2 -> quadratic 
            finalInterpolation += Mathf.Pow(t, 2f) * (A - 2f * B + C);

            return finalInterpolation;
        }



        //
        // Tangent at point t (Forward direction if we travel along the curve)
        //

        public static Vector3 GetTangent(Vector3 posA, Vector3 posB, Vector3 handlePos, float t)
        {
            t = Mathf.Clamp01(t);

            //Alternative 1
            //Same as when we calculate position from t
            //MyVector3 interpolation_posA_handlePos = BezierLinear.GetPosition(posA, handlePos, t);
            //MyVector3 interpolation_handlePos_posB = BezierLinear.GetPosition(handlePos, posB, t);

            //MyVector3 tangent = MyVector3.Normalize(interpolation_handlePos_posB - interpolation_posA_handlePos);

            //Alternative 2
            //The tangent is also the derivative vector
            Vector3 tangent = Vector3.Normalize(GetDerivativeVec(posA, posB, handlePos, t));

            return tangent;
        }

        public override Vector3 GetTangent(float t)
        {
            Vector3 tangent = GetTangent(posA, posB, handlePos, t);

            return tangent;
        }



        //
        // Derivative at point t
        //

        public override float GetDerivative(float t)
        {
            //Alternative 1. Estimated
            //float derivative = InterpolationHelpMethods.EstimateDerivative(this, t);

            //Alternative 2. Exact
            Vector3 derivativeVec = GetDerivativeVec(posA, posB, handlePos, t);

            float derivative = Vector3.Magnitude(derivativeVec);

            return derivative;
        }

        public static Vector3 GetDerivativeVec(Vector3 posA, Vector3 posB, Vector3 handlePos, float t)
        {
            t = Mathf.Clamp01(t);

            Vector3 A = posA;
            Vector3 B = handlePos;
            Vector3 C = posB;

            //The derivative of the equation we use when finding position along the curve at t: 
            //-(2(A - B)) + t(2(A - 2B + C))

            Vector3 derivativeVector = -(2f * (A - B));

            derivativeVector += t * (2f * (A - 2f * B + C));

            return derivativeVector;
        }

        public static Vector3 GetSecondDerivativeVec(Vector3 posA, Vector3 posB, Vector3 handlePos, float t)
        {
            t = Mathf.Clamp01(t);

            Vector3 A = posA;
            Vector3 B = handlePos;
            Vector3 C = posB;

            //The derivative of the equation we use when finding position along the curve at t: 
            //2(A - 2B + C)

            Vector3 derivativeVector = 2f * (A - 2f * B + C);

            return derivativeVector;
        }

        public override Vector3 GetSecondDerivativeVec(float t)
        {
            return GetSecondDerivativeVec(posA, posB, handlePos, t);
        }
    }
}

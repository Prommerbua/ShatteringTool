using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habrador_Computational_Geometry
{
    //This will use Unity's Quaternion (which uses Vector3), but input and output will be MyVector3
    //This so we don't have to write our custom Quaternion class
    public struct MyQuaternion
    {
        private Quaternion unityQuaternion;
    
        public MyQuaternion(Vector3 forward)
        {
            this.unityQuaternion = Quaternion.LookRotation(forward.ToVector3());
        }

        public MyQuaternion(Vector3 forward, Vector3 up)
        {
            this.unityQuaternion = Quaternion.LookRotation(forward.ToVector3(), up.ToVector3());
        }

        public MyQuaternion(Quaternion quaternion)
        {
            this.unityQuaternion = quaternion;
        }



        //
        // Quaternion operations
        //

        //Rotate a quaternion some degrees around some axis
        public static MyQuaternion RotateQuaternion(MyQuaternion oldQuaternion, float angleInDegrees, Vector3 rotationAxis)
        {        
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angleInDegrees, rotationAxis.ToVector3());

            //To rotate a quaternion you just multiply it with the rotation quaternion
            //Important that rotationQuaternion is first!
            Quaternion newQuaternion = rotationQuaternion * oldQuaternion.unityQuaternion;

            MyQuaternion myNewQuaternion = new MyQuaternion(newQuaternion);

            return myNewQuaternion;
        }



        //Rotate a vector by using a quaternion
        public static Vector3 RotateVector(MyQuaternion quat, Vector3 vec)
        {
            UnityEngine.Vector3 rotatedVec = quat.unityQuaternion * vec.ToVector3();

            return rotatedVec.ToMyVector3();
        }

        public Vector3 RotateVector(Vector3 vec)
        {
            UnityEngine.Vector3 rotatedVec = unityQuaternion * vec.ToVector3();

            return rotatedVec.ToMyVector3();
        }



        //
        // Get directions from orientation
        //

        //If you multiply orientation with direction vector you will rotate the direction
        public Vector3 Forward => (unityQuaternion * UnityEngine.Vector3.forward).ToMyVector3();
        public Vector3 Right   => (unityQuaternion * UnityEngine.Vector3.right).ToMyVector3();
        public Vector3 Up      => (unityQuaternion * UnityEngine.Vector3.up).ToMyVector3();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habrador_Computational_Geometry
{
    public struct MyMeshVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public MyVector2 uv;

        public MyMeshVertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;

            this.uv = default;
        }

        public MyMeshVertex(Vector3 position, Vector3 normal, MyVector2 uv)
        {
            this.position = position;
            this.normal = normal;
            this.uv = uv;
        }
    }
}

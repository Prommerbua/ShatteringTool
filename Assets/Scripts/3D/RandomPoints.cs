using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomPoints : MonoBehaviour
{
    struct Voxel
    {
        public Vector3 pos;
        public Vector3 size;
    }

    public int numPoints;


    [SerializeField]private MeshFilter mesh;
    [SerializeField]private Renderer renderer;
    public float VoxelSize = 0.1f;
    private int _width;
    private int _height;
    private int _depth;
    private Voxel[] _grid;



    // Start is called before the first frame update
    void Start()
    {
        renderer = transform.GetComponent<Renderer>();
        SampleArea();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SampleArea()
    {
        //Voxelize Object
        //Voxelize();

        for (int i = 0; i < numPoints; i++)
        {
            Random.Range(0, mesh.sharedMesh.triangles.Length);

        }

    }

    private void Voxelize()
    {
        var bounds = renderer.bounds;
        _width = Mathf.RoundToInt(bounds.size.x);
        _height = Mathf.RoundToInt(bounds.size.y);
        _depth = Mathf.RoundToInt(bounds.size.z);




    }

    private void OnDrawGizmosSelected()
    {
        if (mesh != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.GetComponent<Renderer>().bounds.center, transform.GetComponent<Renderer>().bounds.size);
        }
    }
}

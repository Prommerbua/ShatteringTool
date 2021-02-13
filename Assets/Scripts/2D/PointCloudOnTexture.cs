using System.Collections.Generic;
using UnityEngine;

public class PointCloudOnTexture : MonoBehaviour
{
    private Texture2D _texture2D;
    [SerializeField] private int numberOfPoints;
    [SerializeField] private SpriteRenderer _renderer;
    private List<Vector2> points;

    private int resolutionWidth;
    private int resolutionHeight;


    // Start is called before the first frame update
    void Start()
    {
        _renderer.GetComponent<SpriteRenderer>();
        _texture2D = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB565, false);
        for (int y = 0; y < _texture2D.height; y++)
        {
            for (int x = 0; x < _texture2D.width; x++)
            {
                _texture2D.SetPixel(x, y, Color.white);
                // if (x == 0 || y == 0 || x == _texture2D.width - 1 || y == _texture2D.height - 1)
                // {
                //     _texture2D.SetPixel(x,y, Color.red);
                // }
            }
        }

        _texture2D.Apply();
        _renderer.sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height),
            Vector2.one * 0.5f);

        points = new List<Vector2>();

        CreateRandomPointCloud(numberOfPoints);
        DelaunayTriangulation();
    }

    private void DelaunayTriangulation()
    {
        List<Triangle> triangulation = new List<Triangle>();
        Triangle superTriangle = new Triangle();

        Vertex2D v1 = new Vertex2D(-1, -1);
        Vertex2D v2 = new Vertex2D(resolutionWidth * 2 + 1, -1);
        Vertex2D v3 = new Vertex2D(-1, resolutionHeight * 2 + 1);

        superTriangle.vertices = new List<Vertex2D>();
        superTriangle.vertices.Add(v1);
        superTriangle.vertices.Add(v2);
        superTriangle.vertices.Add(v3);

        triangulation.Add(superTriangle);

        foreach (var point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();
        }
    }

    private void CreateRandomPointCloud(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var x = Random.Range(0, _texture2D.width);
            var y = Random.Range(0, _texture2D.height);

            _texture2D.SetPixel(x, y, Color.black);
            points.Add(new Vector2(x, y));
        }

        _texture2D.Apply();
    }
}

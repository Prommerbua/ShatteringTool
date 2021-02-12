using System.Collections.Generic;
using UnityEngine;

public class Voronoi2D : MonoBehaviour
{
    private Texture2D _texture2D;
    [SerializeField] private int numberOfPoints;
    [SerializeField] private SpriteRenderer _renderer;
    private List<Vector2> points;
    public Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        _renderer.GetComponent<SpriteRenderer>();
        _texture2D = new Texture2D(256, 256, TextureFormat.RGB565, false);
        for (int y = 0; y < _texture2D.height; y++)
        {
            for (int x = 0; x < _texture2D.width; x++)
            {
                _texture2D.SetPixel(x,y, Color.white);
            }
        }
        _texture2D.Apply();
        _renderer.sprite = Sprite.Create(_texture2D, new Rect(0,0, _texture2D.width, _texture2D.height), Vector2.one * 0.5f);

        points = new List<Vector2>();

        CreateRandomPointCloud(numberOfPoints);
        CreateVoronoiCells();
    }

    private void CreateVoronoiCells()
    {

    }

    private void CreateRandomPointCloud(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var x = Random.Range(0, _texture2D.width);
            var y = Random.Range(0, _texture2D.height);

            _texture2D.SetPixel(x, y, Color.black);
            points.Add(new Vector2(x,y));
        }
        _texture2D.Apply();
    }
}

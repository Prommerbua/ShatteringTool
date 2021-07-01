using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject Projectile;
    public Transform SpawnLocation;
    private StarterAssetsInputs _input;
    public float MinDelay = 0.25f;
    public float InitialSpeed = 10.0f;

    private float lastShot = -1000.0f;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.shoot)
        {
            if (Time.time - lastShot >= MinDelay)
            {
                lastShot = Time.time;

                var projectile = Instantiate(Projectile, SpawnLocation.position, transform.rotation);

                projectile.GetComponent<Rigidbody>().velocity = InitialSpeed * Camera.main.transform.forward;
            }
        }
    }


}

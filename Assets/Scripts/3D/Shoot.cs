using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public GameObject Projectile;
    public Transform SpawnLocation;
    private StarterAssetsInputs _input;
    public float MinDelay = 0.25f;
    public float InitialSpeed = 10.0f;

    private float lastShot = -1000.0f;

    private void Awake()
    {
        _input = new StarterAssetsInputs();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        _input.Player.Shoot.started += ShootProjectile;
    }

    private void ShootProjectile(InputAction.CallbackContext obj)
    {
        if (Time.time - lastShot >= MinDelay)
        {
            lastShot = Time.time;

            var projectile = Instantiate(Projectile, SpawnLocation.position, transform.rotation);

            projectile.GetComponent<Rigidbody>().velocity = InitialSpeed * Camera.main.transform.forward;
            Destroy(projectile, 3.0f);
        }
    }

    // Update is called once per frame
}

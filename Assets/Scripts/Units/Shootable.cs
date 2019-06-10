using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shoot only in left side
public class Shootable : MonoBehaviour
{
    [SerializeField]
    private float rate = 2.0F;
    [SerializeField]
    private float projectileSpeed = 8.0f;
    [SerializeField]
    private float projectileLifetime = 1.4f;
    [SerializeField]
    private Color projectileColor = Color.white;
    private Projectile projectileResource;
    private Transform spawnBulletPoint;
    private BasicObject basicObject;

    void Awake()
    {
        projectileResource = Resources.Load<Projectile>("Projectiles/Projectile");
        spawnBulletPoint = transform.Find("SpawnBulletPoint");
        basicObject = GetComponent<BasicObject>();
    }

    void Start()
    {
        InvokeRepeating("Shoot", rate, rate);
    }

    void Shoot()
    {
        Projectile projectile = Instantiate(projectileResource, spawnBulletPoint.position, 
                                               projectileResource.transform.rotation) as Projectile;

        projectile.gameObject.layer = 9;
        projectile.Speed = projectileSpeed;
        projectile.Parent = basicObject;
        projectile.Direction = -transform.right;
        projectile.Color = projectileColor;

        projectile.Activate(projectileLifetime);
    }
}

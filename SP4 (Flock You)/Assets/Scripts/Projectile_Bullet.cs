using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bullet : MonoBehaviour
{
    private Vector3 ShootDir;

    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    public void SetShootDir(Vector3 shootDir)
    {
        ShootDir = shootDir;
    }

    private void Update()
    {
        float moveSpeed = 50f;
        transform.position += ShootDir * moveSpeed * Time.deltaTime;
    }
}

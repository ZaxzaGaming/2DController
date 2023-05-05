using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private Rigidbody2D bulletPrefab;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float timeBetweenAttacks = 2f;
    [SerializeField] private AnimationCurve speedCurve;
    private float shootTimer;
    private Rigidbody2D bulletRB;
    private EnemyProjectile enemyProjectile;
    private Collider2D coll;
    private float speed;
    private float time;
    private void Start()
    {
        coll = GetComponent<Collider2D>();
        
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;
        if(shootTimer >= timeBetweenAttacks)
        {
            shootTimer = 0;
            Shoot();
        }
    }
    private void Shoot()
    {
        bulletRB = Instantiate(bulletPrefab, transform.position, transform.rotation);
        
        bulletRB.transform.right = GetShootDirection();

        speed = speedCurve.Evaluate(time);
        time += Time.fixedDeltaTime;

        bulletRB.velocity = bulletRB.transform.right * speed * bulletSpeed;

        enemyProjectile = bulletRB.gameObject.GetComponent<EnemyProjectile>();

        enemyProjectile.EnemyColl = coll;
    }

    public Vector2 GetShootDirection()
    {
        Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        return (playerTrans.position - transform.position).normalized;
    }
}

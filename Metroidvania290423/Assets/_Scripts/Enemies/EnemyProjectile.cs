using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IDeflectable
{
    private IDamagable iDamagable;
    private Collider2D coll;
    private Rigidbody2D RB;

    public Collider2D EnemyColl { get; set; }
    [field:SerializeField] public float ReturnSpeed { get; set; } = 10f;
    public bool IsDeflecting { get;  set; }

    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private AnimationCurve deflectedSpeedCurve;
    public float projectileLifeSpan = 5f;
    private float projectileLifeTime;

    [SerializeField] private ScreenShakeProfile profile;
    private CinemachineImpulseSource impulseSource;

    private float speed;
    private float time;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        RB = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        IgnoreCollisionWithEnemyToggle();
        projectileLifeTime = 0f;
    }
    private void FixedUpdate()
    {
        if (IsDeflecting)
        {
            speed = deflectedSpeedCurve.Evaluate(time);
            time += Time.fixedDeltaTime;
            projectileLifeTime = 0f;

            RB.velocity = transform.right * speed * ReturnSpeed;
            

        }
        projectileLifeTime += Time.fixedDeltaTime;
        if (projectileLifeTime >= projectileLifeSpan)
        {
            projectileLifeTime = 0f;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        iDamagable = collision.gameObject.GetComponent<IDamagable>(); 
        if(iDamagable != null)
        {
            iDamagable.Damage(damageAmount, Vector2.right, false);
            projectileLifeTime = 0f;
            Destroy(gameObject);
        }
    }
    private void IgnoreCollisionWithEnemyToggle()
    {
        if(!Physics2D.GetIgnoreCollision(coll, EnemyColl))
        {
            Physics2D.IgnoreCollision(coll, EnemyColl, true);
        }
        else
        {
            Physics2D.IgnoreCollision(coll, EnemyColl, false);
        }
    }

    public void Deflect(Vector2 direction)
    {
        IsDeflecting = true;
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);
        IgnoreCollisionWithEnemyToggle();
        if((direction.x > 0 && transform.right.x < 0) || (direction.x < 0 && transform.right.x > 0))
        {
            transform.right = -transform.right;
        }
        RB.velocity = transform.right * ReturnSpeed;
    }
}

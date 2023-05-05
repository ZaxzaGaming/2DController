using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damageAmount = 1.0f;
    [SerializeField] private float timeBetweenAttacks = 0.15f;
    [SerializeField] private float SlowDownOnAttack = 0.3f;
    public bool ShouldBeDamaging { get; private set; } = false;

    private RaycastHit2D[] hits;
    private Player player;
    private Animator anim;
    private float attackTimeCounter;
    private List<IDamagable> iDamagables = new List<IDamagable>();
    private List<IDeflectable> iDeflectables = new List<IDeflectable>();
    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();
        attackTimeCounter = timeBetweenAttacks;
    }
    // Update is called once per frame
    void Update()
    {
        if (InputHandler.instance.controls.Attack1.Attack.WasPressedThisFrame() && attackTimeCounter >= timeBetweenAttacks && player.canAttack)
        {
            attackTimeCounter = 0f;
            anim.SetTrigger("attack");
        }
        attackTimeCounter += Time.deltaTime;
    }
    public IEnumerator DamageWhileSlashIsActive()
    {
        ShouldBeDamaging = true;
        while (ShouldBeDamaging)
        {
            player.Attacking = true;
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                IDamagable iDamagable = hits[i].collider.gameObject.GetComponent<IDamagable>();
                if (iDamagable != null && !iDamagables.Contains(iDamagable))//!iDamagable.HasTakenDamage)
                {
                    iDamagable.Damage(damageAmount, Vector2.right, true); ;
                    Time.timeScale = SlowDownOnAttack;
                    iDamagables.Add(iDamagable);
                }
                IDeflectable iDeflectable = hits[i].collider.gameObject.GetComponent<IDeflectable>();
                if(iDeflectable != null && !iDeflectables.Contains(iDeflectable))
                {
                    Debug.Log("Deflect");
                    iDeflectable.Deflect(transform.right);
                    iDeflectables.Add(iDeflectable);
                }
            }
            yield return null;
        }
        ReturnToDamagableAndDeflectable();
        Time.timeScale = 1f;
    }

    private void ReturnToDamagableAndDeflectable()
    {
        foreach (IDamagable thingThatWasDamaged in iDamagables)
        {
            thingThatWasDamaged.HasTakenDamage = false;
        }
        iDamagables.Clear();
        iDeflectables.Clear();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
    #region Animation Triggers
    public void shouldBeDamagingTrue() => ShouldBeDamaging = true;
    public void shouldBeDamagingFalse() => ShouldBeDamaging = false;
    #endregion
}

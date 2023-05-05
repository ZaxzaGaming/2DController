using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    private Knockback knockback;
    [SerializeField] float maxHealth = 3;
    [SerializeField] private ScreenShakeProfile profile;
    private float currentHealth;

    public bool HasTakenDamage { get; set; }
    private CinemachineImpulseSource impulseSource;

    public void Damage(float damageAmount, Vector2 hitDirection, bool playerAttack)
    {
        if (playerAttack)
        {
            CameraShakeManager.instance.ScreenShakeFromProfile(profile,impulseSource);
        }
        HasTakenDamage = true;
        currentHealth -= damageAmount;

        if (currentHealth <= 0 )
        {
            Die();
        }
        knockback.CallKnockback(hitDirection, Vector2.up, 0f);
    }

    private void Start()
    {
        knockback = GetComponent<Knockback>();
        currentHealth = maxHealth;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}

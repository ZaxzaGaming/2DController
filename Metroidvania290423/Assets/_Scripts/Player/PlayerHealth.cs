using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    private Knockback knockback;
    private SpriteRenderer SR;
    private SpriteColorFlasher SQF;

    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerHealthAdded;
    public static event Action OnPlayerDeath;
    public float health, maxHealth;
    [SerializeField] private ScreenShakeProfile profile;
    private CinemachineImpulseSource impulseSource;

    public bool HasTakenDamage { get; set; }

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SQF = GetComponent<SpriteColorFlasher>();
        knockback = GetComponent<Knockback>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public void Damage(float amount, Vector2 hitDirection, bool playerAttack)
    {
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);
        health -= amount;
        OnPlayerDamaged?.Invoke();
        SQF.FlashColor(SR, 0.25f, Color.red);

        if (health <= 0 )
        {
            health = 0;
            Debug.Log("Your Dead");
            OnPlayerDeath?.Invoke();
        }
        knockback.CallKnockback(hitDirection, Vector2.up, InputHandler.instance.moveInput.x);
    }
    public void AddHealth(float amount)
    {
        health += amount;
        OnPlayerHealthAdded?.Invoke();
        if(health>=maxHealth)
        {
            health = maxHealth;
        }
    }

    public void Damage(float damageAmount)
    {
        throw new NotImplementedException();
    }
}

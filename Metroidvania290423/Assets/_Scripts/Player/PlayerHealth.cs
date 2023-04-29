using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerHealthAdded;
    public static event Action OnPlayerDeath;
    public float health, maxHealth;

    private SpriteRenderer SR;
    private SpriteColorFlasher SQF;
    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SQF = GetComponent<SpriteColorFlasher>();
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        OnPlayerDamaged?.Invoke();
        SQF.FlashColor(SR, 0.25f, Color.red);

        if (health <= 0 )
        {
            health = 0;
            Debug.Log("Your Dead");
            OnPlayerDeath?.Invoke();
        }
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
}

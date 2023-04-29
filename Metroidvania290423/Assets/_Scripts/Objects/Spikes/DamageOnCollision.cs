using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public float damageCounter;
    private void OnTriggerStay2D(Collider2D collision)
    { 
        if (collision.gameObject.CompareTag("Player"))
        {
            damageCounter += Time.deltaTime;

            if (damageCounter >= 0.5)
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
                damageCounter = 0;
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public bool HasTakenDamage {  get; set; }
    public void Damage(float damageAmount, Vector2 hitDirection, bool playerAttack);
}

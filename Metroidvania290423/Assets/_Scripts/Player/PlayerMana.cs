using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    public static event Action OnManaUsed;
    public static event Action OnPlayerManaAdded;
    public float mana, maxMana;
    private void Start()
    {
    }
    public void UseMana(float amount)
    {
        mana -= amount;
        OnManaUsed?.Invoke();

        if (mana <= 0 )
        {
            mana = 0;
            Debug.Log("No Mana");
        }
    }
    public void AddMana(float amount)
    {
        mana += amount;
        OnPlayerManaAdded?.Invoke();
        if(mana>=maxMana)
        {
            mana = maxMana;
        }
    }
}

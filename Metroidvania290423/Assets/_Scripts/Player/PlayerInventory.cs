using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IInventory
{
    public int Money { get => money; set => money = value; }
    public int Mana { get => mana; set => mana = value; }
    public int Health { get => health; set => health = value; }

    public int money = 0;
    public int mana = 0;
    public int health = 0;
}

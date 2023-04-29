using System.Collections;
using UnityEngine;

public interface IInventory
{

    int Money { get; set; }
    int Mana { get; set; }
    int Health { get; set; }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, ICollectable
{
    public static event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData);
    public ItemData itemData;

    public void Collect()
    {
        Destroy(gameObject);
        OnItemCollected?.Invoke(itemData);
    }

}

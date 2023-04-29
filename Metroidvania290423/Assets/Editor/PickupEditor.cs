using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ItemData))]
[CanEditMultipleObjects]

public class PickupEditor : Editor
{
    ItemData itemData;
    bool isDrop;

    void OnEnable()
    {
        itemData = (ItemData)target;
    }

    public override void OnInspectorGUI()
    {
        itemData.pickupType = (ItemData.PickupType)EditorGUILayout.EnumPopup("Pickup Type", itemData.pickupType);
        if (itemData.pickupType == ItemData.PickupType.Upgrade)
            itemData.upgradeType = (ItemData.UpgradeType)EditorGUILayout.EnumPopup("Upgrade Type", itemData.upgradeType);
            
        else if (itemData.pickupType == ItemData.PickupType.Ability)
            itemData.abilityType = (ItemData.AbilityType)EditorGUILayout.EnumPopup("Ability Type", itemData.abilityType);
        if (itemData.isDrop && ((ItemData.PickupType)itemData.pickupType != ItemData.PickupType.Upgrade || (ItemData.PickupType)itemData.pickupType != ItemData.PickupType.Ability))
        {
            itemData.moveSpeed = 7f;
            itemData.Value = 1;
        }
        else
        {
            itemData.moveSpeed = 0f;
            itemData.Value = 20;
        }
        if (itemData.pickupType == ItemData.PickupType.Upgrade && itemData.upgradeType == ItemData.UpgradeType.BiggerMagnet)
        {
            itemData.moveSpeed = 0f;
            itemData.Value = 8;
        }
         
        DrawDefaultInspector();
    }
}

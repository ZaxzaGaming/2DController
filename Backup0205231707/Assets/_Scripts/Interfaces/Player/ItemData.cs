using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public enum PickupType
    {
        Money,
        Mana,
        Health,
        Upgrade,
        Ability,
    };
    public enum UpgradeType
    {
        BiggerMagnet,
        TripleJump,
    };
    public enum AbilityType
    {
        DoubleJump,
        WallJump,
        FastFall,
        Glide,
        Dash,
        Magnet,
    }

    public Sprite icon;
    [HideInInspector]public PickupType pickupType;
    [HideInInspector] public UpgradeType upgradeType;
    [HideInInspector] public AbilityType abilityType;
    public Material material;
    public Mesh mesh;
    [HideInInspector] public float Value;
    public bool isDrop;
    [HideInInspector] public float moveSpeed;
}


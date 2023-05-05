using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    private SafeGroundSaver safeGroundSaver;
    private Player player;
    private PlayerHealth playerHealth;
    private SafeGroundCheckpointSaver safeGroundCheckpointSaver;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerHealth = player.GetComponent<PlayerHealth>();
        safeGroundSaver = player.GetComponent<SafeGroundSaver>();
        safeGroundCheckpointSaver = player.GetComponent<SafeGroundCheckpointSaver>();
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    { 
        if (collision.gameObject.CompareTag("Player"))
        {
            player.isSafe = false;
            playerHealth.TakeDamage(1);
            if (player.checkpointSpawning)
            {
                safeGroundCheckpointSaver.WarpPlayerToSafeGround();
            }
            else
            {
                safeGroundSaver.WarpPlayerToSafeGround();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player.isSafe = true;
    }
}

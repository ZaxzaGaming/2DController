using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleManager : MonoBehaviour
{
    public ParticleSystem JumpDustUp;
    public ParticleSystem LandDust;
    public ParticleSystem MoveParticles;
    private Player player;
    private Vector2 landPoint;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    private void FixedUpdate()
    {
        
        if (player.IsJumping && player.jumpNo <= 1)
        {
            JumpDustUp.Play();
        }
        else
        {
            JumpDustUp.Stop();
        }
        if (player.IsGrounded && player.moveInputX != 0f) MoveParticles.Play();
        else if (!player.IsGrounded || player.moveInputX == 0f) MoveParticles.Stop();
    }
    public void SpawnLandParticles()
    {
        landPoint = transform.position;
        ParticleSystem tmpLandParticles = Instantiate(LandDust, new Vector2(landPoint.x, landPoint.y), LandDust.transform.rotation);
        Destroy(tmpLandParticles, .1f);
    }
}

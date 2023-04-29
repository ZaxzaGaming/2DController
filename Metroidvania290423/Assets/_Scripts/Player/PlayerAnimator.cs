using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Player player;
    private Animator anim;
    private SpriteRenderer spriteRend;

    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }

    private void Start()
    {
        player = GetComponent<Player>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        anim = spriteRend.GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        CheckAnimationState();
    }

    private void CheckAnimationState()
    {
        if (startedJumping)
        {
            anim.SetTrigger("Jump");
            //Jump Particles Instantiate here
            startedJumping = false;
            return;
        }

        if (justLanded)
        {
            anim.SetTrigger("Land");
            //Land Particles Instantiate here
            justLanded = false;
            return;
        }
        if(!player.IsGrounded && player.LastOnWallTime < 0)
        {
            player.InAir = true;
        }
        else
        {
            player.InAir = false;
        }

        anim.SetFloat("Vel Y", player.RB.velocity.y);
        anim.SetFloat("Vel X", player.RB.velocity.x);
        anim.SetBool("isGrounded", player.IsGrounded);
        anim.SetBool("canClimb", player.CanClimb);
        anim.SetBool("isWallSliding", player.IsSliding);
        anim.SetBool("inAir", player.InAir);
    }
}
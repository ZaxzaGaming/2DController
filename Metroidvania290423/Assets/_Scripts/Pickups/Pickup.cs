using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PickupEvent : UnityEvent<float> { }    
public class Pickup : MonoBehaviour
{
    public PickupEvent pickup;

    private MeshRenderer MR;
    private MeshFilter MF;
    private Rigidbody2D rb;
    private Animator animator;
    public ItemData itemData;

    [HideInInspector] public CircleCollider2D circleCollider;
    bool hasTarget;
    Vector3 targetPossition;
    private Player player;

    private void Start()
    {
        MR = GetComponent<MeshRenderer>();
        MF = GetComponent<MeshFilter>();
        animator = GetComponent<Animator>();

        MR.material = itemData.material;
        MF.sharedMesh = itemData.mesh;
        //pickupType = pickupData.pickupType;
        animator.SetBool("IsDrop", itemData.isDrop);
        if (itemData.isDrop)
        {
            gameObject.AddComponent<Rigidbody2D>();
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
            gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            rb = GetComponent<Rigidbody2D>();
        }
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (itemData.isDrop)
        {
            if (hasTarget)
            {
                //Floats towars player if close enough and player has magnet
                float accelRate = (Mathf.Abs(itemData.moveSpeed) > 0.01f) ? 20 : 50;
                float speedDifx = itemData.moveSpeed - rb.velocity.x;
                float speedDify = itemData.moveSpeed - rb.velocity.y;
                float movementx = speedDifx * accelRate;
                float movementy = speedDify * accelRate;
                Vector2 movement = new Vector2(movementx, movementy) * 2;

                Vector2 targetDirection = (targetPossition - transform.position).normalized;
                rb.velocity = new Vector2(targetDirection.x, targetDirection.y);
                rb.AddForce(movement * rb.velocity, ForceMode2D.Force);
               // circleCollider.enabled = !circleCollider.enabled;

            }
            else
            {
                //Drops and stops moving if the player is out of range.
                rb.velocity = new Vector2(0, rb.velocity.y);
                circleCollider.enabled = circleCollider.enabled;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collector"))
        {
            player = collision.GetComponentInParent<Player>();
            //health = collision.GetComponentInParent<PlayerHealth>();
            //mana = collision.GetComponentInParent<PlayerMana>();
            //magnet = player.GetComponentInChildren<Magnet>();
            if (!itemData.isDrop)
            {
                Debug.Log("Up");
                float length = animator.GetCurrentAnimatorClipInfo(0).Length;
                StartCoroutine(PickUp(length));
                animator.SetTrigger("PickedUp");
            }
            else
            {
                SetInActive();
                Destroy(gameObject);
            }

        }
    }
    public void SetInActive()
    {
        pickup.Invoke(itemData.Value);
    }
    public void SetTarget(Vector3 position, bool targetSet)
    {
        targetPossition = position;
        hasTarget = targetSet;
    }

    private IEnumerator PickUp(float time)
    {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = new Vector3(transform.position.x, transform.position.y + 2f);
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

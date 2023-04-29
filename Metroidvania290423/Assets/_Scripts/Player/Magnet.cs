using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    private Player player;
    private bool magnet;
    private CircleCollider2D magnetCollider;
    private void Start()
    {
        player = GetComponentInParent<Player>();
        magnetCollider = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        if (player.hasMagnet) magnet = true;
        else magnet = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (magnet)
        {
            if (collision.gameObject.TryGetComponent<Pickup>(out Pickup pickup))
            {
                if (magnetCollider.radius > 10)
                {
                    pickup.circleCollider.enabled = false;
                }
                else pickup.circleCollider.enabled = true;
                if (pickup.itemData.isDrop)
                {
                    pickup.SetTarget(transform.parent.position, true);
                }
            }
        }
    }
        private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Pickup>(out Pickup pickup))
        {
            pickup.SetTarget(transform.parent.position, false);

        }
    }
    public void SetSize(float size)
    {
        magnetCollider = gameObject.GetComponent<CircleCollider2D>();
        magnetCollider.radius = size;
    }
}


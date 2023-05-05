using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SafeGroundCheckpointSaver : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCheckpoint;
    public Vector2 SafeGroundLocation { get; private set; } = Vector2.zero;
    private void Start()
    {
        SafeGroundLocation = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((whatIsCheckpoint.value & (1 << collision.gameObject.layer)) > 0)
        {
            SafeGroundLocation = new Vector2(collision.bounds.center.x, collision.bounds.min.y);
        }
    }
    public void WarpPlayerToSafeGround()
    {
        transform.position = SafeGroundLocation;
    }
}

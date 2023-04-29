using UnityEngine;
using System.Collections;

public class TrackVelocity : MonoBehaviour
{

    private Rigidbody2D rb;
    private Vector2 velocity = Vector3.zero;

    public Vector2 Velocity { get { return velocity; } }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        velocity = rb.velocity;
    }
}
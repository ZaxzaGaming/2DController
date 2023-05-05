using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;

public class Teleporter : MonoBehaviour
{
    public GameObject destination;
    public bool Transporting = false;
    public bool invertX;
    public bool invertY;
    public bool convertAxisVelocity;
    Vector2 velocity;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = new Vector2(destination.transform.position.x, destination.transform.position.y);
            TrackVelocity velocityScript = collision.GetComponent<TrackVelocity>();
            if (velocityScript != null)
                if (convertAxisVelocity)
                {
                    collision.GetComponent<Rigidbody2D>().velocity = new Vector2(invertX ? -velocityScript.Velocity.y : velocityScript.Velocity.y, invertY ? -velocityScript.Velocity.x : velocityScript.Velocity.x);
                }
                else
                {
                    collision.GetComponent<Rigidbody2D>().velocity = new Vector2(invertX ? -velocityScript.Velocity.x : velocityScript.Velocity.x, invertY ? -velocityScript.Velocity.y : velocityScript.Velocity.y);
                }
        
        }
    }

}

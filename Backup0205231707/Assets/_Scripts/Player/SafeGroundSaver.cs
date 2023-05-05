using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeGroundSaver : MonoBehaviour
{
    [SerializeField] private float saveFrequency = 3f;
    public Vector2 SafeGroundLocation { get; private set; } = Vector2.zero;
    private Coroutine safeGroundCoroutine;
    private Player player;
    private void Start()
    {
        player = GetComponent<Player>();
        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
        SafeGroundLocation = transform.position;
    }
    private IEnumerator SaveGroundLocation()
    {
        float elapsedTime = 0f;
        while(elapsedTime < saveFrequency)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (player.IsGrounded && player.isSafe)
        {
            SafeGroundLocation = transform.position;
        }
        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
    }
    public void WarpPlayerToSafeGround()
    {
        transform.position = SafeGroundLocation;
    }
}

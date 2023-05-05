using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float flipRotationTime = 0.5f;

    private Coroutine turnCoroutine;

    private Player player;
    private bool isFacingRight;
    private void Awake()
    {
        player = playerTransform.GetComponent<Player>();
        isFacingRight = player.IsFacingRight;
    }
    private void Update()
    {
        transform.position = playerTransform.position;
    }
    public void CallTurn()
    {
        //turnCoroutine = StartCoroutine(FlipYLerp());
        LeanTween.rotateY(gameObject, DeterminEndRotation(), flipRotationTime).setEase(LeanTweenType.easeInOutSine);
    }
    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotaionAmount = DeterminEndRotation();
        float yRotation = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flipRotationTime) {
            elapsedTime += Time.deltaTime;
            yRotation += Mathf.Lerp(startRotation, endRotaionAmount, (elapsedTime / flipRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            yield return null;
        }
    }
    public float DeterminEndRotation()
    {
        isFacingRight = !isFacingRight;
        if(isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}

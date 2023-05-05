using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Knockback : MonoBehaviour
{
    public float knockbackTime = 0.2f;
    public float hitDirectionForce = 10f;
    public float constantForce = 5f;
    public float inputForce = 7.5f;
    private Rigidbody2D RB;

    public bool IsBeingKnockedBack { get; private set; }

    public AnimationCurve knockbackForceCurve;

    private Coroutine knockbackCoroutine;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }
    public IEnumerator KnockBackAction(Vector2 hitDirection, Vector2 constForceDirection, float inputDirection)
    {
        IsBeingKnockedBack = true;
        Vector2 hitForce;
        Vector2 constForce;
        Vector2 knockbackForce;
        Vector2 combinedForce;

        float time = 0f;

        constForce = constForceDirection * constantForce;
        
        float elapsedTime = 0f;
        while(elapsedTime < knockbackTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            time += Time.fixedDeltaTime;

            hitForce = hitDirection * hitDirectionForce * knockbackForceCurve.Evaluate(time);
            knockbackForce = hitForce * constForce;

            if(inputDirection != 0 )
            {
                combinedForce = knockbackForce + new Vector2(inputDirection * inputForce, 0f);
            }
            else
            {
                combinedForce = knockbackForce;
            }

            RB.velocity = combinedForce;

            yield return new WaitForFixedUpdate();
        }
        IsBeingKnockedBack = false;
    }
    public void CallKnockback(Vector2 hitDirection, Vector2 constForceDirection, float inputDirection)
    {
        knockbackCoroutine = StartCoroutine(KnockBackAction(hitDirection, constForceDirection, inputDirection));
    }
}

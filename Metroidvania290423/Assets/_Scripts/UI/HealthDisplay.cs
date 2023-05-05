using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 targetEmpty = new Vector2(-100, 0);
    private Vector2 targetHalf = new Vector2(-52, 0);
    private Vector2 targetFull= new Vector2(0, 0);

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void SetHeartImage(HeartStatus status)
    {
        switch(status)
        {
            case HeartStatus.Empty:
                LeanTween.move(rect, targetEmpty, 0.4f);
                //rect.LeanSetLocalPosX(-104.98f); 
                //anim.SetInteger("health", (int)HeartStatus.Empty);
                break;
            case HeartStatus.Half:
                LeanTween.move(rect, targetHalf, 0.4f);
                //rect.LeanSetLocalPosX(-52.82f);
                //anim.SetInteger("health", (int)HeartStatus.Half);
                break;
            case HeartStatus.Full:
                LeanTween.move(rect, targetFull, 0.4f);
                //rect.LeanSetLocalPosX(0f);
                //anim.SetInteger("health", (int)HeartStatus.Full);
                break;
        }
    }
}
public enum HeartStatus
{
    Empty = 0,
    Half = 1,
    Full = 2
}

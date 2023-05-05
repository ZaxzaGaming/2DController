using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaDisplay : MonoBehaviour
{
    public Sprite fullMana, halfMana, emptyMana;

    Image manaImage;
    private void Awake()
    {
        manaImage = GetComponent<Image>();
    }
    public void SetManaImage(ManaStatus status)
    {
        switch(status)
        {
            case ManaStatus.Empty:
                manaImage.sprite = emptyMana;
                break;
            case ManaStatus.Half:
                manaImage.sprite = halfMana;
                break;
            case ManaStatus.Full:
                manaImage.sprite = fullMana;
                break;
        }
    }
}
public enum ManaStatus
{
    Empty = 0,
    Half = 1,
    Full = 2
}

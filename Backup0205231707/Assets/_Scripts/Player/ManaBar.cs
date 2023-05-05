using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public GameObject manaPrefab;
    public PlayerMana playerMana;

    List<ManaDisplay> mana = new List<ManaDisplay>();
    private void OnEnable()
    {
        PlayerMana.OnManaUsed += DrawMana;
        PlayerMana.OnPlayerManaAdded += DrawMana;
    }
    private void OnDisable()
    {
        PlayerMana.OnManaUsed -= DrawMana;
        PlayerMana.OnPlayerManaAdded -= DrawMana;
    }
    private void Start()
    {
        DrawMana();
    }
    public void DrawMana()
    {
        ClearMana();
        float maxManaRemainder = playerMana.maxMana % 2;
        int manaToMake = (int)((playerMana.maxMana / 2) + maxManaRemainder);
        for(int i = 0; i < manaToMake; i++)
        {
            CreateEmptyMana();
        }
        for(int i = 0; i < mana.Count; i++)
        {
            int manaStatusRemainder = (int)Mathf.Clamp(playerMana.mana - (i * 2), 0, 2);
            mana[i].SetManaImage((ManaStatus)manaStatusRemainder);
        }
    }
    public void CreateEmptyMana()
    {
        GameObject newMana = Instantiate(manaPrefab);
        newMana.transform.SetParent(transform);

        ManaDisplay manaComponent = newMana.GetComponent<ManaDisplay>();
        manaComponent.SetManaImage(ManaStatus.Empty);
        mana.Add(manaComponent);
    }
    public void ClearMana()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        mana = new List<ManaDisplay> ();
    }
}

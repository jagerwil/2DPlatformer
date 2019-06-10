using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CoinsBar : MonoBehaviour
{
    private Character character;
    private Text text;

    private void Awake()
    {
        character = FindObjectOfType<Character>();
        text = GetComponentInChildren<Text>();
    }

    public void Refresh(int coinsCount)
    {
        text.text = "x" + coinsCount;
    }

    public void HideBar()
    {
        gameObject.SetActive(false);
    }
}

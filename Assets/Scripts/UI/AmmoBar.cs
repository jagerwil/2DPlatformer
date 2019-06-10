using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AmmoBar : MonoBehaviour
{
    private Character character;
    private Text text;

    private void Awake()
    {
        character = FindObjectOfType<Character>();
        text = GetComponentInChildren<Text>();
    }

    public void Refresh(int ammoCount)
    {
        text.text = "x" + ammoCount;
    }

    public void HideBar()
    {
        gameObject.SetActive(false);
    }
}

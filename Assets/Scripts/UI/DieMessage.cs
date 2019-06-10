using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieMessage : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void ShowMessage()
    {
        text.enabled = true;
    }

    public void HideMessage()
    {
        text.enabled = false;
    }
}

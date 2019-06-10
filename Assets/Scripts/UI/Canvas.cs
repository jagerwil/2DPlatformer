using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Canvas : MonoBehaviour
{
    new public CameraController camera { get; set; }
    public CoinsBar coinsBar { get; set; }
    public AmmoBar ammoBar { get; set; }
    public LivesBar livesBar { get; set; }
    public DieMessage dieMessage { get; set; }

    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject jumpButton;
    public GameObject shootButton;

    private void Awake()
    {
        camera = GetComponentInChildren<CameraController>();
        coinsBar = GetComponentInChildren<CoinsBar>();
        ammoBar = GetComponentInChildren<AmmoBar>();
        livesBar = GetComponentInChildren<LivesBar>();
        dieMessage = GetComponentInChildren<DieMessage>();

        leftButton = GameObject.Find("LeftButton");
        rightButton = GameObject.Find("RightButton");
        jumpButton = GameObject.Find("JumpButton");
        shootButton = GameObject.Find("ShootButton");
    }

    public void HideWidgets()
    {
        livesBar.Refresh(0);
        coinsBar.HideBar();
        ammoBar.HideBar();

        leftButton.SetActive(false);
        rightButton.SetActive(false);
        jumpButton.SetActive(false);
        shootButton.SetActive(false);
    }
}

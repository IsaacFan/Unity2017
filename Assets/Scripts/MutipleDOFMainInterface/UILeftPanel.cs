//using System.Collections;
//using System.Collections.Generic;
//using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UILeftPanel : MonoBehaviour {

    public Button uiPVEButton;
    public Button uiPVPButton;
    public Button uiClubPVPButton;


    void Awake()
    {
        CUIMultipleDOFMainInterface.Instance.setUIReference(this);
    }
    public void init(UnityAction pveButtonCallback, UnityAction pvpButtonCallback, UnityAction clubPVPButtonCallback)
    {
        uiPVEButton.onClick.AddListener(pveButtonCallback);
        uiPVPButton.onClick.AddListener(pvpButtonCallback);
        uiClubPVPButton.onClick.AddListener(clubPVPButtonCallback);
    }

    public void dragPanel(float deltaX)
    {

    }



}

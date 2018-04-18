using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIRightPanel : MonoBehaviour {

    public Button uiPlayerInfoButton;
    public Button uiClubButton;
    public Button uiIllustrationButton;
    public Button uiSettingButton;


    void Awake()
    {
        CUIMultipleDOFMainInterface.Instance.setUIReference(this);
    }
    public void init(UnityAction playerInfoButtonCallback, UnityAction clubButtonCallback,
                     UnityAction illustrationButtonCallback, UnityAction settingButtonCallback)
    {
        uiPlayerInfoButton.onClick.AddListener(playerInfoButtonCallback);
        uiClubButton.onClick.AddListener(clubButtonCallback);
        uiIllustrationButton.onClick.AddListener(illustrationButtonCallback);
        uiSettingButton.onClick.AddListener(settingButtonCallback);
    }

    public void dragPanel(float deltaX)
    {

    }





}

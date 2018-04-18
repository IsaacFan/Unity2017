using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIMiddlePanel : MonoBehaviour {

    public Button uiMailButton;
    public Button uiGuideButton;
    public Button uiMissionButton;
    public Button uiCharacterButton;
    public Button uiFriendButton;
    public Button uiGachaButton;
    public Button uiAcheivementButton;
    public Button uiShopButton;


    void Awake()
    {
        CUIMultipleDOFMainInterface.Instance.setUIReference(this);
    }
    public void init(UnityAction mailButtonCallback, UnityAction guideButtonCallback, UnityAction missionButtonCallback,
                     UnityAction characterButtonCallback, UnityAction friendButtonCallback,
                     UnityAction gachaButtonCallback, UnityAction acheivementButtonCallback, UnityAction shopButtonCallback)
    {
        uiMailButton.onClick.AddListener(mailButtonCallback);
        uiGuideButton.onClick.AddListener(guideButtonCallback);
        uiMissionButton.onClick.AddListener(missionButtonCallback);
        uiCharacterButton.onClick.AddListener(characterButtonCallback);
        uiFriendButton.onClick.AddListener(friendButtonCallback);
        uiGachaButton.onClick.AddListener(gachaButtonCallback);
        uiAcheivementButton.onClick.AddListener(acheivementButtonCallback);
        uiShopButton.onClick.AddListener(shopButtonCallback);

    }

    public void dragPanel(float deltaX)
    {

    }




}

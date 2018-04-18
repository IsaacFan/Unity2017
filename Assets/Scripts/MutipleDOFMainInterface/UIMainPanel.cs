using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIMainPanel : MonoBehaviour {

    public Image[] penalPageStateImages;
    public Color penalSwitchOnColor;
    public Color penalSwitchOffColor;


    void Awake()
    {
        CUIMultipleDOFMainInterface.Instance.setUIReference(this);
    }
    public void init(UnityAction pveButtonCallback, UnityAction pvpButtonCallback, UnityAction clubPVPButtonCallback)
    {
        //uiPVEButton.onClick.AddListener(pveButtonCallback);
        //uiPVPButton.onClick.AddListener(pvpButtonCallback);
        //uiClubPVPButton.onClick.AddListener(clubPVPButtonCallback);
    }

    public void dragPanel(float deltaX)
    {

    }

    private void setPenalPageState(CUIMultipleDOFMainInterface.ePanelPage panelPage)
    {
        for (int i = 0; i < penalPageStateImages.Length; i++)
        {
            if (i == (int)panelPage)
                penalPageStateImages[i].color = penalSwitchOnColor;
            else
                penalPageStateImages[i].color = penalSwitchOffColor;
        }

    }




}

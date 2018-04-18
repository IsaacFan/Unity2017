//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CUIMultipleDOFMainInterface : MonoBehaviour {

    public enum ePanelPage
    {
        PanelPage_Middle = 0,
        PanelPage_Left,
        PanelPage_Right,
        PanelPage_Max,
    }


    private static CUIMultipleDOFMainInterface instance = null;
    public static CUIMultipleDOFMainInterface Instance
    {
        get
        {
            if (instance == null)
                instance = new CUIMultipleDOFMainInterface();

            return instance;
        }
    }


    private UIMainPanel uiMainPanel;
    private UILeftPanel uiLeftPanel;
    private UIMiddlePanel uiMiddlePanel;
    private UIRightPanel uiRightPanel;

    private ePanelPage panelPage;


    private CUIMultipleDOFMainInterface()
    {
        InputListener.Instance.registerDragCallback(onDragCallback);
    }
    ~CUIMultipleDOFMainInterface()
    {
        InputListener.Instance.unregisterDragCallback(onDragCallback);
    }

    public void setUIReference(UIMainPanel uiMainPanel)
    {
        this.uiMainPanel = uiMainPanel;
        uiMainPanel.init(onClickPVEButtonCallback, onClickPVPButtonCallback, onClickClubPVPButtonCallback);
    }
    public void setUIReference(UILeftPanel uiLeftPanel)
    {
        this.uiLeftPanel = uiLeftPanel;
        uiLeftPanel.init(onClickPVEButtonCallback, onClickPVPButtonCallback, onClickClubPVPButtonCallback);
    }
    public void setUIReference(UIMiddlePanel uiMiddlePanel)
    {
        this.uiMiddlePanel = uiMiddlePanel;
        uiMiddlePanel.init(onClickMailButtonCallback, onClickGuildButtonCallback, onClickMissionButtonCallback,
                           onClickCharacterButtonCallback, onClickFriendButtonCallback,
                           onClickGachaButtonCallback, onClickAcheivementButtonCallback, onClickShopButtonCallback);
    }
    public void setUIReference(UIRightPanel uiRightPanel)
    {
        this.uiRightPanel = uiRightPanel;
        uiRightPanel.init(onClickPlayerInfoButtonCallback, onClickClubButtonCallback,
                          onClickIllustrationButtonCallback, onClickSettingButtonCallback);
    }


    private void onDragCallback(float deltaX, float deltaY)
    {
        uiLeftPanel.dragPanel(deltaX);
        uiMiddlePanel.dragPanel(deltaX);
        uiRightPanel.dragPanel(deltaX);
    }

    private void onClickPVEButtonCallback()
    {
    }
    private void onClickPVPButtonCallback()
    {
    }
    private void onClickClubPVPButtonCallback()
    {
    }

    private void onClickMailButtonCallback()
    {
    }
    private void onClickGuildButtonCallback()
    {
    }
    private void onClickMissionButtonCallback()
    {
    }
    private void onClickCharacterButtonCallback()
    {
    }
    private void onClickFriendButtonCallback()
    {
    }
    private void onClickGachaButtonCallback()
    {
    }
    private void onClickAcheivementButtonCallback()
    {
    }
    private void onClickShopButtonCallback()
    {
    }

    private void onClickPlayerInfoButtonCallback()
    {
    }
    private void onClickClubButtonCallback()
    {
    }
    private void onClickIllustrationButtonCallback()
    {
    }
    private void onClickSettingButtonCallback()
    {
    }


}

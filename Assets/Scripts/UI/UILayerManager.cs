using System.Collections.Generic;


public class UILayerManager : BaseSingleton<UILayerManager>
{

    #region Data Members 
    private List<BaseCUI> mainBaseCUIList = new List<BaseCUI>();                    // front index : old open UI, back index : new open UI
    private List<BaseCUI> battleBaseCUIList = new List<BaseCUI>();                  // front index : old open UI, back index : new open UI
    private List<StandaloneCUI> standaloneCUIList = new List<StandaloneCUI>();

    private GameState gameState;
    #endregion


    #region Constructor
    UILayerManager()
    {
    }
    #endregion

    #region Destructor
    ~UILayerManager()
    {
        //GameManager.getInstance().onGameStateSwitchStart -= onGameStateSwitchStart;
		//GameManager.getInstance ().onGameStateClose -= onGameStateClose;
		//GameManager.getInstance ().onGameStateStart -= onGameStateStart;

    }
    #endregion


    #region Funtions
    public override void init()
    {
        //GameManager.getInstance().onGameStateSwitchStart += onGameStateSwitchStart;
		//GameManager.getInstance ().onGameStateClose += onGameStateClose;
		//GameManager.getInstance ().onGameStateStart += onGameStateStart;
	}

	private void onGameStateStart(GameState gameState)
	{
        this.gameState = gameState;
	}

	private void onGameStateClose(GameState state)
    {
		switch (state)
        {
            default:
                mainBaseCUIList.Clear();
                standaloneCUIList.Clear();
                break;
            case GameState.GameState_PVE:
            case GameState.GameState_PVP:
            case GameState.GameState_Co_PVE:
            case GameState.GameState_Co_PVP:
                standaloneCUIList.Clear();
                battleBaseCUIList.Clear();
                break;
        }
    }


    public bool pushBaseCUIStack(BaseCUI newBaseCUI)
    {
        List<BaseCUI> baseCUIList = getBaseCUIList();
        if (baseCUIList.Count > 0)
        {
            if (baseCUIList[baseCUIList.Count - 1] == newBaseCUI)
                return false;
        }

        baseCUIList.Add(newBaseCUI);
        return true;
    }
    public bool popBaseCUIStack(BaseCUI targetBaseCUI)
    {
        List<BaseCUI> baseCUIList = getBaseCUIList();
        if (baseCUIList.Count == 0)
            return false;

        BaseCUI baseCUI = baseCUIList[baseCUIList.Count - 1];
        if (baseCUI != targetBaseCUI)
            return false;

        baseCUI.setUIVisible(false);
        return baseCUIList.Remove(baseCUI);
    }

    public bool pushStandaloneCUI(StandaloneCUI newStandaloneCUI)
    {
        if (standaloneCUIList.IndexOf(newStandaloneCUI) != -1)
            return false;

        standaloneCUIList.Add(newStandaloneCUI);
        return true;
    }
    public bool popStandaloneCUI(StandaloneCUI targetStandaloneCUI)
    {
        targetStandaloneCUI.setUIVisible(false);
        return standaloneCUIList.Remove(targetStandaloneCUI);
    }

    public void closeAllCUI()
    {
        List<BaseCUI> baseCUIList = getBaseCUIList();

        BaseCUI baseCUI;
        for (int i = baseCUIList.Count - 1; i >= 0; i--)
        {
            baseCUI = baseCUIList[i];
            if (baseCUI.getUIVisible() == true)
                baseCUI.notifyAllTimeBeforeCloseUI();
            popBaseCUIStack(baseCUI);
        }
    }
    public void closeTopLayerCUI()
    {
        List<BaseCUI> baseCUIList = getBaseCUIList();
        if (baseCUIList.Count > 0)
        {
            BaseCUI baseCUI = baseCUIList[baseCUIList.Count - 1];
            if ((baseCUI.GetType().BaseType == typeof(TopFullScreenLayerCUI)) ||
                (baseCUI.GetType().BaseType == typeof(TopHalfeScreenLayerCUI)))
            {
                baseCUI.notifyAllTimeBeforeCloseUI();
                popBaseCUIStack(baseCUI);
            }
        }
    }

    public BaseCUI getNowOpenUI()
    {
        List<BaseCUI> baseCUIList = getBaseCUIList();

        if (baseCUIList.Count <= 0)
            return null;
        else
            return baseCUIList[baseCUIList.Count - 1];
    }





    


    private List<BaseCUI> getBaseCUIList()
    {
        switch (gameState)
        {
            default:
                return mainBaseCUIList;
            case GameState.GameState_PVE:
            case GameState.GameState_PVP:
            case GameState.GameState_Co_PVE:
            case GameState.GameState_Co_PVP:
                return battleBaseCUIList;                
        }
    }

    public void refreshBaseCUIDisplay()
    {
        List<BaseCUI> baseCUIList = getBaseCUIList();

        bool halfScreenShowed = false;
        bool funllScreenShowed = false;

        BaseCUI baseCUI;
        for (int i = baseCUIList.Count - 1; i >= 0; i--)
        {
            baseCUI = baseCUIList[i];

            // find full screen UI 
            if (funllScreenShowed == true)
            {
                if (baseCUI.getUIVisible() == true)
                {
                    baseCUI.notifyAllTimeBeforeCloseUI();
                    baseCUI.setUIVisible(false);
                }
                            
                continue;
            }

            if ((baseCUI.GetType().BaseType == typeof(TopFullScreenLayerCUI)) ||
                (baseCUI.GetType().BaseType == typeof(SystemRootLayerCUI)) ||
                (baseCUI.GetType().BaseType == typeof(FullScreenCUI)))
            {        
                if (baseCUI.getUIVisible() == false)
                {
                    baseCUI.notifyAllTimeBeforeOpenUI();
                    baseCUI.setUIVisible(true);
                }

                funllScreenShowed = true;
            }
            else/* if (baseCUI.GetType().BaseType == typeof(HalfScreenCUI))*/
            {
                // only show top HalfScreenCUI 
                if (halfScreenShowed == false)
                {
                    if (baseCUI.getUIVisible() == false)
                    {
                        baseCUI.notifyAllTimeBeforeOpenUI();
                        baseCUI.setUIVisible(true);
                    }

                    halfScreenShowed = true;
                }
                else
                {
                    if (baseCUI.getUIVisible() == true)
                    {
                        baseCUI.notifyAllTimeBeforeCloseUI();
                        baseCUI.setUIVisible(false);
                    }
                }
            }
        }

    }
    private void refreshStandAaloneCUIDisplay()
    {
        StandaloneCUI standaloneCUI;
        for (int i = standaloneCUIList.Count - 1; i >= 0; i--)
        {
            standaloneCUI = standaloneCUIList[i];
            standaloneCUI.setUIVisible(true);
        }
    }




    #endregion

}

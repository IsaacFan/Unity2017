using UnityEngine;
using UnityEngine.UI;

// it must be only one root of every UI prefab architecture
// and will use the root to control the RectTransform to make the visible or unvisible of the UI prefab

public class CUIConstant
{
    public static readonly Vector2 UI_VISIBLE_POSITION = Vector2.zero;
    public static readonly Vector2 UI_UNVISIBLE_POSITION = new Vector2(10000, 10000);
}

// standalone UI - open and close alone, not b affected by other UI
public class StandaloneCUI
{

    #region Data Members
    private RectTransform m_rectTransform;
    private GraphicRaycaster[] m_graphicRaycasters;

    private bool m_uiVisible;
    /*
    protected uint m_playerDataVer;
    protected uint m_bagDataVer;

    protected bool m_isNeedUpdate;
    */
    #endregion


    #region Funtions
    public virtual void setUIReference(GameObject uiGameObject)
    {
        // TODO : change the code into that getting the reference and save int the UI script
        // TODO : write an automatic function which can pre-coding some code depend from UI architecture and get & save reference in script
        m_rectTransform = uiGameObject.GetComponent<RectTransform>();
        m_graphicRaycasters = uiGameObject.GetComponents<GraphicRaycaster>();
    }
    /*
    public virtual bool createUI()
    {
        return false;
    }
    */
    public virtual void destroyUI()
    {
    }
    /*
    public virtual void resetLanguage()
    {
    }
    */
    public virtual bool openUI()
    {
        return UILayerManager.Instance.pushStandaloneCUI(this);
    }
    public virtual void closeUI()
    {
        UILayerManager.Instance.popStandaloneCUI(this);
    }

    public virtual GameObject getGameObject()
    {
        return null;
    }
    public virtual void setUIVisible(bool visible)
    {
        if (m_rectTransform == null)
            return;

        if (visible == true)
            m_rectTransform.anchoredPosition = CUIConstant.UI_VISIBLE_POSITION;
        else
            m_rectTransform.anchoredPosition = CUIConstant.UI_UNVISIBLE_POSITION;
        for (int i = 0; i < m_graphicRaycasters.Length; ++i)
            m_graphicRaycasters[i].enabled = visible;

        m_uiVisible = visible;
    }
    public virtual bool getUIVisible()
    {
        return m_uiVisible;
    }
    public void initUIVisible(bool visible)
    {
        if (visible == true)
            openUI();
        else
            setUIVisible(visible);
    }

    #endregion

    /*
    // use for red point hint
    public virtual void setRefresh()
    {
    }
    */

}


public class BaseCUI
{
    /*
    #region Typedefs and Enums
    public enum eUIEnterAnimationType : byte
    {
        UIENTERANIMATIONTYPE_NONE,
        UIENTERANIMATIONTYPE_FADEIN,
        UIENTERANIMATIONTYPE_MOVEIN,
        UIENTERANIMATIONTYPE_GROWOUT,
    };
    public enum eUIExitAnimationType : byte
    {
        UIEXITANIMATIONTYPE_NONE,
        UIEXITANIMATIONTYPE_FADEOUT,
        UIEXITANIMATIONTYPE_MOVEOUT,
        UIEXITANIMATIONTYPE_SHRINKINTO,
    };
    #endregion
    */

    #region Data Members
    private RectTransform m_rectTransform;
    private GraphicRaycaster[] m_graphicRaycasters;

    private bool m_uiVisible;
    /*
    protected uint m_playerDataVer;
    protected uint m_bagDataVer;

    protected bool m_isNeedUpdate;
    */
    //private eUIEnterAnimationType m_uiEnterAnimationType = eUIEnterAnimationType.UIENTERANIMATIONTYPE_NONE;
    //private eUIExitAnimationType m_uiExitAnimationType = eUIExitAnimationType.UIEXITANIMATIONTYPE_NONE;
    #endregion



    #region Functions
    public virtual void setUIReference(GameObject uiGameObject)
    {
        m_rectTransform = uiGameObject.GetComponent<RectTransform>();
        m_graphicRaycasters = uiGameObject.GetComponents<GraphicRaycaster>();
    }
    /*
    public virtual bool createUI()
    {
        return false;
    }
    */
    public virtual void destroyUI()
    {
    }
    /*
    public virtual void resetLanguage()
    {
    }
    */
    public virtual bool openUI()
    {
        if (UILayerManager.Instance.getNowOpenUI() == this)
            return false;

        UILayerManager.Instance.closeTopLayerCUI();

        if (UILayerManager.Instance.pushBaseCUIStack(this) == true)
        {
            UILayerManager.Instance.refreshBaseCUIDisplay();
            return true;
        }

        return false;
    }
    public virtual void closeUI()
    {
        if (UILayerManager.Instance.getNowOpenUI() != this)
            return;

        if (UILayerManager.Instance.popBaseCUIStack(this) == true)
        {
            notifyAllTimeBeforeCloseUI();
            UILayerManager.Instance.refreshBaseCUIDisplay();
        }
    }

    public virtual GameObject getGameObject()
    {
        return null;
    }
    public virtual void setUIVisible(bool visible)
    {
        if (m_rectTransform == null)
            return;

        if (visible == true)
            m_rectTransform.anchoredPosition = CUIConstant.UI_VISIBLE_POSITION;
        else
            m_rectTransform.anchoredPosition = CUIConstant.UI_UNVISIBLE_POSITION;
        for (int i = 0; i < m_graphicRaycasters.Length; ++i)
            m_graphicRaycasters[i].enabled = visible;

        m_uiVisible = visible;
        /*
        if (visible == false)
            stopActions();
        */
    }
    public virtual bool getUIVisible()
    {
        return m_uiVisible;
    }
    public void initUIVisible(bool visible)
    {
        if (visible == true)
            openUI();
        else
            setUIVisible(visible);
    }

    public virtual void notifyAllTimeBeforeOpenUI()
    {
    }
    public virtual void notifyAllTimeBeforeCloseUI()
    {
    }
    /*
    public virtual void notifyOpenUI()
    {
    }
    public virtual void notifyCloseUI()
    {
    }
    */

    /*
    public virtual void stopActions()
    {
    }
    public virtual void playEnterAnimation()
    {
    }
    public virtual void playExitAnimation()
    {
    }
    */

    /*
    // use for red point
    public virtual void setRefresh()
    {
    }
    */
    /*// use for red point 
    public bool checkNeedUpdate()
    {
        return m_isNeedUpdate;
    }
    */
    #endregion

}


// special use : only display once in Top Layer
// full screen UI & top layer : when open -> hide all the other UI
public class TopFullScreenLayerCUI : BaseCUI
{
}

// special use : only display once in Top Layer
// not full screen & top layer : when open ->  hide other HalfScreenCUI
public class TopHalfeScreenLayerCUI : BaseCUI
{
}

// system first UI page : when open -> close all the other UI
public class SystemRootLayerCUI : BaseCUI
{

    #region Override Functions
    public override bool openUI()
    {
        if (UILayerManager.Instance.getNowOpenUI() == this)
            return false;

        UILayerManager.Instance.closeAllCUI();

        if (UILayerManager.Instance.pushBaseCUIStack(this) == true)
        {
            UILayerManager.Instance.refreshBaseCUIDisplay();
            return true;
        }

        return false;
    }

    #endregion

}

// full screen UI : when open -> hide SystemRootLayerCUI, hide all the other UI
public class FullScreenCUI : BaseCUI
{
}

// not full screen UI : when open -> hide other HalfScreenCUI
public class HalfScreenCUI : BaseCUI
{
}
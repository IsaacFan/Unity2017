using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class UIWordUnit : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private Text uiText;
    public int UIWordID { get; set; }


    void Awake()
    {
        resetTextByUIWordID();
        UIWordUnitsManager.Instance.onChangeLanguage += resetTextByUIWordID;
    }
    void OnDestroy()
    {
        UIWordUnitsManager.Instance.onChangeLanguage -= resetTextByUIWordID;
    }


    public void resetTextByUIWordID()
    { 
        if (uiText == null)
            uiText = GetComponent<Text>();

        string content = "";
        UIWordUnitsManager.Instance.UIWordData.getStringByID(UIWordID, ref content);
        uiText.text = content;
    }

    public void setText(int uiWordID)
    {
        UIWordID = uiWordID;
        resetTextByUIWordID();
    }
    public void setText(string content)
    {
        if (uiText == null)
            uiText = GetComponent<Text>();

        uiText.text = content;
    }
}

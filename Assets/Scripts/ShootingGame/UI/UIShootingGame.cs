//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShootingGame : MonoBehaviour {

    public Image[] heartImages;
    public Image[] skillImages;

    void Awake()
    {
        CUIShootingGame.Instance.setUIReference(this);
    }

    public void setHearNumber(int number)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < number)
                heartImages[i].color = Color.red;
            else
                heartImages[i].color = Color.white;
        }     
    }

    public void setSkillNumber(int number)
    {
        for (int i = 0; i < skillImages.Length; i++)
        {
            if (i < number)
                skillImages[i].color = Color.blue;
            else
                skillImages[i].color = Color.white;
        }
    }

}

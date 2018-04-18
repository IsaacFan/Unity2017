//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class CUIShootingGame {

    private static CUIShootingGame instance = null;
    public static CUIShootingGame Instance
    {
        get
        {
            if (instance == null)
                instance = new CUIShootingGame();

            return instance;
        }
    }

    private UIShootingGame uiShootingGame;


    public void setUIReference(UIShootingGame uiShootingGame)
    {
        this.uiShootingGame = uiShootingGame;
    }

    public void setHeartNumber(int number)
    {
        uiShootingGame.setHearNumber(number);
    }
    public void setSkillNumber(int number)
    {
        uiShootingGame.setSkillNumber(number);
    }

}

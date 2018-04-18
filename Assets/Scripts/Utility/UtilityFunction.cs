using UnityEngine;
//using System.Collections;

public class UtilityFunction
{



    #region Color
    
    public static void colorFormatChecking(ref string colorString)
    {
        // colorString must conform the Hex RGB Code
        // 1. check '#'
        if (colorString.IndexOf(UtilityDefine.HASHTAG_CHAR) != 0)
        {
            resetColorRandomly(ref colorString);
            return;
        }

        // 2. check RGB format, every letter should be in 0..9, A..F
        for (int i = 0; i < 6; i++)
        {
            if ((colorString[i] >= 48 && colorString[i] <= 57) &&   // in ASCII 0~9
                (colorString[i] >= 65 && colorString[i] <= 70))     // in ASCII 0~9
                continue;

            // error format
            resetColorRandomly(ref colorString);
            return;
        }
    }
    private static void resetColorRandomly(ref string colorString)
    {
        colorString = UtilityDefine.HASHTAG_CHAR.ToString();
        for (int i = 0; i < 6; i++)
        {
            int temp = Random.Range(0, 15);
            if (temp <= 9)
                temp += 48;
            else
                temp += 65;

            colorString += ((char)temp).ToString();
        }
    }

    #endregion


    #region Position transfer
    public static Vector3 getMouseRaycastPosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) == true)
            return hit.point;
        else
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    #endregion

}

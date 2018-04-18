//using UnityEngine;
using UnityEditor;

public class MenuItemFunctions {

    [MenuItem("CustomTools/Create/TestScriptableObjectAsset")]
    public static void createScriptableObjectAsset()
    {
        ScriptableObjectUtility.createScriptableObjectAsset<ScriptableObjectClass>();
    }

}





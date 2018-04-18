using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility {

    public static void createScriptableObjectAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        /*
        // save the asset in the selected folder
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
            path = "Assets";
        else if (Path.GetExtension(path) != "")     // get path without file name
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        */

        // save the asset in the fixed folder
        string path = @"Assets/ScripableObjectAssets";
        // folder checking
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(T).ToString() + ".asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
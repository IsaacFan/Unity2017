using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UIWordUnit))]
public class UIWordUnitEditor : Editor
{
    private UIWordUnit uiWordUnit;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying == false)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Input UI Word ID : ");

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID");
            uiWordUnit.UIWordID = EditorGUILayout.IntField(uiWordUnit.UIWordID, GUILayout.Width(50));
            if (uiWordUnit.UIWordID < 0)
                uiWordUnit.UIWordID = 0;

            uiWordUnit.resetTextByUIWordID();

            EditorUtility.SetDirty(target);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("English"))
            {
                UIWordUnitsManager.Instance.changeGameLanguage(UIWordUnitsManager.LanguageType.LanguageType_English);
            }
            if (GUILayout.Button("Taiwan"))
            {
                UIWordUnitsManager.Instance.changeGameLanguage(UIWordUnitsManager.LanguageType.LanguageType_TraditionalChinese);
            }
            if (GUILayout.Button("Japan"))
            {
                UIWordUnitsManager.Instance.changeGameLanguage(UIWordUnitsManager.LanguageType.LanguageType_Japanese);
            }
            if (GUILayout.Button("China"))
            {
                UIWordUnitsManager.Instance.changeGameLanguage(UIWordUnitsManager.LanguageType.LanguageType_SimplifiedChinese);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }


    private void Reset()
    {
        if (EditorApplication.isPlaying == false)
        {
            uiWordUnit = target as UIWordUnit;
            uiWordUnit.resetTextByUIWordID();
        }
    }

}
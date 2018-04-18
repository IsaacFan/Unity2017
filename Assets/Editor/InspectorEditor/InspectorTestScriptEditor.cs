using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InspectorTestScript))]
public class InspectorTestScriptEditor : Editor {

    public override void OnInspectorGUI()
    {
        InspectorTestScript myInspectorTestScript = (InspectorTestScript)target;

        myInspectorTestScript.intData = EditorGUILayout.IntField("Int Data", myInspectorTestScript.intData);
        myInspectorTestScript.floatData = EditorGUILayout.Slider("Flaot Data", myInspectorTestScript.floatData, 0f, 10f);

    }

}

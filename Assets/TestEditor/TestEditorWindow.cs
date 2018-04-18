//using System.Collections;
//using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class TestEditorWindow : EditorWindow {

    public GameObject nodePrefabGameObject;
    private string insertIndexString;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [MenuItem("CustomTools/Window/TestEditorWindow")]
    private static void openWindow()
    {
        // get existing open window or if none, make a new one:
        TestEditorWindow window = (TestEditorWindow)EditorWindow.GetWindow(typeof(TestEditorWindow));
        window.Show();
    }

    void OnGUI()
    {
        //if (insertIndexString == null)
        //    return;

        //GUILayout.Label("Map: " + TestEditor.Instance.mapName, EditorStyles.boldLabel);
        if (GUI.Button(new Rect(10, 10, 150, 20), "Load another map") == true)
        {

            return;
        }

        if (GUI.Button(new Rect(10, 40, 200, 20), "Add a new node(in the tail)") == true)
        {

            return;
        }

        GUI.Label(new Rect(10, 70, 50, 20), "Index: ");
        insertIndexString = Regex.Replace(insertIndexString, "[^0-9]", "");
        insertIndexString = GUI.TextField(new Rect(55, 70, 30, 20), insertIndexString);

        if (GUI.Button(new Rect(100, 70, 150, 20), "Insert a new node") == true)
        {

            return;
        }
        if (GUI.Button(new Rect(260, 70, 130, 20), "Delete a node") == true)
        {


            return;
        }

        //if (GUILayout.Button("Delete selected node") == true)
        if (GUI.Button(new Rect(10, 100, 165, 20), "Delete selected node") == true)
        {
            if (Selection.activeGameObject == null ||
                Selection.activeGameObject.layer != LayerMask.NameToLayer("Node"))
                return;

            Undo.DestroyObjectImmediate(Selection.activeGameObject);
            return;
        }

        //if (GUILayout.Button("Save map") == true)
        if (GUI.Button(new Rect(10, 130, 103, 20), "Save map") == true)
        {

            return;
        }


        // 熱鍵監聽 delete/backspace drag
        Event e = Event.current;
        switch (e.type)
        {
            default:
                break;
            case EventType.KeyDown:
                if (e.keyCode == KeyCode.Delete)
                {
                    // 等刪除gameobject後 更新資料
                }
                break;


        }


    }


}

//using System.Collections;
//using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TestEditor : MonoBehaviour {

    /*
    private TestEditor instance;
    public TestEditor Instance
    {
        get
        {
            if (instance == null)
                instance = new TestEditor();

            return instance;
        }
    }
    */


    //public TestEditor Instance { get; private set; }

    public string mapName;


    // Use this for initialization
    void Start ()
    {
        //Instance = this;
        openWindow();

    }

    private void openWindow()
    {
        //TestEditorWindow window = (TestEditorWindow)EditorWindow.GetWindow(typeof(TestEditorWindow));
        //window.Show();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnGUI()
    {



    }


}

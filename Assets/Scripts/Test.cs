using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public Transform testTarget;

    private System.Action<float, float> testCallback;


    // Use this for initialization
    void Start () {

        InputListener.Instance.init();

        /*
        JsonClass testClass = new JsonClass();
        testClass.floatData = 99.2f;
        testClass.ints = new int[5];
        JsonFileManager.saveJsonData(testClass, "abc");
        */
        /*
        JsonClass testClass = JsonFileManager.loadJsonData<JsonClass>("abc");
        Debug.Log(testClass);
        */


        testCallback = CameraManager.Instance.setCameraOperationToScreenControl(testTarget);
        InputListener.Instance.registerMouseMoveCallback(onMouseMoveCallback);

    }
	
	// Update is called once per frame
	void Update () {
        InputListener.Instance.update();
	}


    private void onMouseMoveCallback(float x, float y)
    {

        testCallback(x, y);
    }

}

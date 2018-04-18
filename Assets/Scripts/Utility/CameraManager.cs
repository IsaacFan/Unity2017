//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public enum CameraOperation
    {
        CameraOperation_Hide = 0,
        CameraOperation_LookAtPosition,
        CameraOperation_LookAtTarget,

        CameraOperation_FollowTarget,
        CameraOperation_ScreenControl,

    }


    private static CameraManager instance;
    public static CameraManager Instance
    {
        get
        {
            if (instance != null)
                return instance;

            GameObject gameObject = GameObject.Find("Camera");
            if (gameObject == null)
            {
                gameObject = new GameObject("Camera");
                gameObject.AddComponent<Camera>();
                gameObject.AddComponent<FlareLayer>();
                gameObject.AddComponent<AudioListener>();
            }

            instance = gameObject.GetComponent<CameraManager>();
            if (instance == null)
                instance = gameObject.AddComponent<CameraManager>();

            instance.init();
            return instance;
        }
    }

    private Camera mainCamera;
    private CameraOperation cameraOperation;
    private Transform targetLooked;
    public float screenControlMovingRate;



    private void init()
    {
        mainCamera = gameObject.GetComponent<Camera>();
        if (mainCamera == null)
            mainCamera = gameObject.AddComponent<Camera>();


    }
    /*
    // Use this for initialization
    void Start () {
		
	}
	*/
	// Update is called once per frame
	void Update () {

        switch (cameraOperation)
        {
            case CameraOperation.CameraOperation_Hide:
                break;
            case CameraOperation.CameraOperation_LookAtPosition:
                break;
            case CameraOperation.CameraOperation_LookAtTarget:
                break;
            case CameraOperation.CameraOperation_FollowTarget:
                break;
            case CameraOperation.CameraOperation_ScreenControl:
                break;
            default:
                break;
        }

    }
    

    public void setCameraOperationToHide()
    {
        switchCameraOperation(CameraOperation.CameraOperation_Hide);
    }
    public void setCameraOperationToLookAtPosition(Vector3 position)
    {
        switchCameraOperation(CameraOperation.CameraOperation_LookAtPosition);
        mainCamera.transform.LookAt(position);
    }
    public Action setCameraOperationToLookAtTarget(Transform transform, Action targetMovedCallback)
    {
        switchCameraOperation(CameraOperation.CameraOperation_LookAtTarget);
        targetLooked = transform;
        targetLookedMovedCallback();
        return targetLookedMovedCallback;
    }
    public void setCameraOperationToFollowTarget(Transform parentTransform)
    {
        switchCameraOperation(CameraOperation.CameraOperation_FollowTarget);
        mainCamera.transform.SetParent(parentTransform, false);
    }
    public Action<float, float> setCameraOperationToScreenControl(Transform transform)
    {
        switchCameraOperation(CameraOperation.CameraOperation_ScreenControl);
        return screenDragCallback;
    }
    private void switchCameraOperation(CameraOperation cameraOperation)
    {

        switch (this.cameraOperation)
        {
            case CameraOperation.CameraOperation_Hide:
                mainCamera.enabled = true;
                break;
            case CameraOperation.CameraOperation_LookAtPosition:
                break;
            case CameraOperation.CameraOperation_LookAtTarget:
                mainCamera.transform.SetParent(null);
                break;
            case CameraOperation.CameraOperation_FollowTarget:
                break;
            case CameraOperation.CameraOperation_ScreenControl:
                break;
            default:
                break;
        }

        switch (cameraOperation)
        {
            case CameraOperation.CameraOperation_Hide:
                mainCamera.enabled = false;
                break;
            case CameraOperation.CameraOperation_LookAtPosition:
                break;
            case CameraOperation.CameraOperation_LookAtTarget:
                break;
            case CameraOperation.CameraOperation_FollowTarget:
                break;
            case CameraOperation.CameraOperation_ScreenControl:
                break;
            default:
                break;
        }

        this.cameraOperation = cameraOperation;
    }




    private void targetLookedMovedCallback()
    {
        mainCamera.transform.LookAt(targetLooked);
    }

    private void screenDragCallback(float x, float y)
    {
        Vector3 nowPosition = mainCamera.transform.position;
        mainCamera.transform.position = new Vector3(nowPosition.x + -x * screenControlMovingRate,
                                                    nowPosition.y, 
                                                    nowPosition.z + y * screenControlMovingRate);
    }

    /*
    public void playAnimation()
    {


    }
    */


}

using System.Collections;
//using System.Linq;
using UnityEngine;
//using UnityEngine.XR.WSA.WebCam;

public class CameraDeviceCapture : MonoBehaviour
{

    private WebCamTexture physicalWebCamTexture;

    public Renderer displayRenderer;


    // Use this for initialization
    void Start()
    {

        WebCamDevice[] webCamDevices = WebCamTexture.devices;
        if (webCamDevices.Length == 0)
            return;

        StartCoroutine(openCamera(webCamDevices[0].name));
    }

    private IEnumerator openCamera(string webCamDeviceName)
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam) == true)
        {
            physicalWebCamTexture = new WebCamTexture(webCamDeviceName);
            displayRenderer.material.mainTexture = physicalWebCamTexture;
            physicalWebCamTexture.Play();
        }
    }

    private void onClickSavePictureButton()
    {
        Texture2D texture2D = new Texture2D(physicalWebCamTexture.width, physicalWebCamTexture.height);
        texture2D.SetPixels(physicalWebCamTexture.GetPixels());
        texture2D.Apply();
        //System.IO.File.WriteAllBytes("image.png", texture2D.EncodeToPNG());
    }

}


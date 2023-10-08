using UnityEngine;
using NRKernal;
using System.Linq;
using UnityEngine.UI;

public class JakesNRManager : MonoBehaviour
{
    private GameObject NRInputGO;
    private GameObject NRCameraRigGO;
    private NRVirtualDisplayer mNRVirtualDisplayer;
    private GameObject VirtualControllerGO;
    private Camera PhoneCamera;
    private RenderTexture originalTargetTexture;
    private GameObject CanvasScalerGO;
    private JakesRemoteController mJakesRemoteController;

    private void Start()
    {
        UpdateRefs();

        InitPhoneCamera();
    }

    [Button]
    public void InitPhoneCamera()
    {
        // ensure they're deactivated by default
        NRInputGO.SetActive(false);
        NRCameraRigGO.SetActive(false);
        mNRVirtualDisplayer.enabled = false;
        mJakesRemoteController.enabled = false;
        if (CanvasScalerGO?.GetComponent<CanvasScaler>() is CanvasScaler scaler)
        {
            scaler.enabled = true;
        }

        Debug.Log("SetPhoneCameraActive!");
        SetPhoneCameraActive();
    }

    // <summary>
    // Initialize NRInput and NRCameraRig on demand instead of automatically at start
    // Enables debugging the app without having to put on the headset
    // </summary>
    public void InitNR()
    {
        try
        {
            NRInputGO.SetActive(true);
            NRCameraRigGO.SetActive(true);
            VirtualControllerGO.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error initializing NR: " + e.Message);
        }
    }

    private void SetPhoneCameraActive()
    {
        // remove the target texture from the camera
        PhoneCamera.targetTexture = null;
        // set the PhoneCamera tag to MainCamera
        PhoneCamera.tag = "MainCamera";
    }

    private void UpdateRefs()
    {
        mNRVirtualDisplayer = FindObjectOfType<NRVirtualDisplayer>();
        if(mNRVirtualDisplayer == null)
        {
            Debug.LogError("No NRVirtualDisplayer found in scene");
        }

        NRInputGO = FindObjectOfType<NRInput>()?.gameObject;
        if (NRInputGO == null)
        {
            Debug.LogError("No NRInput found in scene");
        }

        NRCameraRigGO = FindObjectOfType<NRSessionBehaviour>()?.gameObject;
        if (NRCameraRigGO == null)
        {
            Debug.LogError("No NRSessionBehaviour found in scene");
        }

        VirtualControllerGO = FindObjectOfType<MultiScreenController>()?.gameObject;
        if (VirtualControllerGO == null)
        {
            Debug.LogError("No MultiScreenController found in scene");
        }

        var Canvases = FindObjectsOfType<Canvas>();
        // find the one on the "Canvas" gameobject
        CanvasScalerGO = Canvases.First(c => c.gameObject.name == "Canvas" && c.gameObject.activeInHierarchy)?.gameObject;
        if (CanvasScalerGO == null)
        {
            Debug.LogError("No CanvasScaler found in scene");
        }

        mJakesRemoteController = FindObjectOfType<JakesRemoteController>();
        if (mJakesRemoteController == null)
        {
            Debug.LogError("No JakesRemoteController found in scene");
        }

        // get the Camera component from the PhoneCamera gameobject
        var PhoneCameras = FindObjectsOfType<Camera>();
        // find the one to the PhoneCamera gameobject
        PhoneCamera = PhoneCameras.First(c => c.gameObject.name == "PhoneCamera");
        if(PhoneCamera == null)
        {
            Debug.LogError("No PhoneCamera found in scene");
        }
        // store the original target texture
        if(originalTargetTexture == null)
        {
            originalTargetTexture = PhoneCamera?.targetTexture;
        }
    }
}
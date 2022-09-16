using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

public class ARController : MonoBehaviour
{
    public ARSession arSession;
    public ARPlaneManager arPlaneManager;
    public PlaceOnPlane placeOnPlane;
    public Camera arCamera;
    public ARCameraManager arCameraManager;
    private void OnDisable()
    {
        arCameraManager.enabled = false;
        arCamera.enabled = false;
        placeOnPlane.enabled = false;
        foreach (var item in arPlaneManager.trackables)
        {
            DestroyImmediate(item.gameObject);
        }
        arPlaneManager.enabled = false;
        arSession.enabled = false;
        Application.targetFrameRate = -1;

    }

    private void OnEnable()
    {
        arSession.enabled = true;
        arPlaneManager.enabled = true;
        placeOnPlane.enabled = true;
        arCamera.enabled = true;
        arCameraManager.enabled = true;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

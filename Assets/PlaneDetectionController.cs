using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneDetectionController : MonoBehaviour
{
    ARPlaneManager arPlaneManager;
    // Start is called before the first frame update
    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    public void clearPlanes()
    {
        foreach (var item in arPlaneManager.trackables)
        {
            DestroyImmediate(item);
        }
    }
}

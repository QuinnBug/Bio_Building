using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCanvasController : MonoBehaviour
{

    private enum ARState {  SEARCHING, WAITING, VIEWING};

    private ARState currentARState = ARState.SEARCHING;

    public GameObject searchingCanvasObjects;
    public GameObject waitingCanvasObjects;
    public GameObject viewingCanvasObjects;

    public ARPlaneManager planeManager;
    // Start is called before the first frame update
    void Awake()
    {
        SwitchARState((int)currentARState);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentARState)
        {
            case ARState.SEARCHING:
                if (planeManager.trackables.count > 0)
                    SwitchARState(1);
                break;
            case ARState.WAITING:
                break;
            case ARState.VIEWING:
                break;
            default:
                break;
        }
    }

    public void SwitchARState(int _stateToSwitchTo)
    {
        currentARState = (ARState)_stateToSwitchTo;

        searchingCanvasObjects.SetActive(false);
        waitingCanvasObjects.SetActive(false);
        viewingCanvasObjects.SetActive(false);

        switch (currentARState)
        {
            case ARState.SEARCHING:
                searchingCanvasObjects.SetActive(true);
                break;
            case ARState.WAITING:
                waitingCanvasObjects.SetActive(true);
                break;
            case ARState.VIEWING:
                viewingCanvasObjects.SetActive(true);
                break;
            default:
                break;
        }
    }
}

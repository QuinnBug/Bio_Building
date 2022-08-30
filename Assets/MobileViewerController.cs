using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileViewerController : MonoBehaviour
{

    private List<GameObject> availableObjects = new List<GameObject>();
    public ObjectController objectController;
    private int currentlyLoadedObjectIndex;
    // Start is called before the first frame update
    void Start()
    {
        availableObjects = new List<GameObject>( Resources.LoadAll<GameObject>("Active_Prefabs/"));
    }

    public void LoadSpecificObject(int _objectToLoad)
    {
        currentlyLoadedObjectIndex = _objectToLoad;
        Debug.Log("Loading object " + currentlyLoadedObjectIndex + " which is " + availableObjects[currentlyLoadedObjectIndex].name);
        objectController.ChangeObject(availableObjects[currentlyLoadedObjectIndex]);
    }

    public void LoadByIncrementingObject(int _objectToIncrementBy)
    {
        currentlyLoadedObjectIndex = (availableObjects.Count + currentlyLoadedObjectIndex + _objectToIncrementBy) % availableObjects.Count;
        objectController.ChangeObject(availableObjects[currentlyLoadedObjectIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) LoadByIncrementingObject(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) LoadByIncrementingObject(1);
    }
}

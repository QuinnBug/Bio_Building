using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MobileViewerController : MonoBehaviour
{
    public bool AR = false; 
    private List<GameObject> availableObjects = new List<GameObject>();
    public ObjectController objectController;
    public ObjectController ARObjectController;
    public LoadedObjectDetails objectDetails;
    public TextMeshProUGUI ARTitle;
    private int currentlyLoadedObjectIndex;
    // Start is called before the first frame update
    void Start()
    {
        availableObjects = new List<GameObject>( Resources.LoadAll<GameObject>("Active_Prefabs/"));
    }

    public void LoadSpecificObject(int _objectToLoad)
    {
        currentlyLoadedObjectIndex = _objectToLoad;
        if (AR)
        {
            ARObjectController.ChangeObject(availableObjects[currentlyLoadedObjectIndex]);
            ARTitle.text = availableObjects[currentlyLoadedObjectIndex].GetComponent<Metadata_Plus>().name;
        }

        else
        {
            objectController.ChangeObject(availableObjects[currentlyLoadedObjectIndex]);        
            objectDetails.SetNewObjectData(availableObjects[currentlyLoadedObjectIndex].GetComponent<Metadata_Plus>());
        }

        MetadataEfficencyAnalyser.instance.SetShapeValues(availableObjects[currentlyLoadedObjectIndex].GetComponent<Metadata_Plus>());
    }

    public void LoadByIncrementingObject(int _objectToIncrementBy)
    {
        currentlyLoadedObjectIndex = (availableObjects.Count + currentlyLoadedObjectIndex + _objectToIncrementBy) % availableObjects.Count;
        LoadSpecificObject(currentlyLoadedObjectIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) LoadByIncrementingObject(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) LoadByIncrementingObject(1);
    }

    public void SetAR(bool _value)
    {
        AR = _value;
    }    
}

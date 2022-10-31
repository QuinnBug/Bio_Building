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

    public SaveManager saveManager;
    public Transform buildingHolder;
    private List<GameObject> loadedBuildings = new List<GameObject>();
    bool buildingViewer;
    // Start is called before the first frame update
    void Start()
    {
        availableObjects = new List<GameObject>( Resources.LoadAll<GameObject>("Active_Prefabs/"));
    }

    public void InitLoad(bool _buildingViewer)
    {
        buildingViewer = _buildingViewer;
        if(buildingViewer)
        {
            foreach (var item in saveManager.allRoomData)
            {
                GameObject buildingTemplate = new GameObject();
                buildingTemplate.transform.parent = buildingHolder.transform;
                buildingTemplate.transform.localPosition = Vector3.zero;
                buildingTemplate.name = item.name; 
                buildingTemplate.AddComponent<Metadata_Plus>();
                Metadata_Plus buildingMetadata = buildingTemplate.GetComponent<Metadata_Plus>();
                foreach (SelectableData itemData in item.selectables)
                {
                    BuildBuilding(itemData, buildingTemplate.transform, buildingMetadata);
                }
                loadedBuildings.Add(buildingTemplate);
            } 
        }
        LoadSpecificObject(0);
    }

    private void BuildBuilding(SelectableData _data, Transform _buildingTemplate, Metadata_Plus _buildingMetadata)
    {
        GameObject obj = Instantiate(ResourceManager.Instance.GetPrefab(_data.prefabName), _buildingTemplate);

        obj.name = _data.prefabName + "_" + _data.id.ToString();
        obj.transform.localPosition = _data.position;
        obj.transform.rotation = Quaternion.Euler(0, _data.yRotation, 0);

        if(_buildingMetadata.parameters.ContainsKey(_data.prefabName))
        {
            int newValue = int.Parse( _buildingMetadata.parameters[_data.prefabName]);
            newValue += 1;
            _buildingMetadata.parameters[_data.prefabName] = newValue.ToString();
        }
        else
        {
            _buildingMetadata.parameters.Add(_data.prefabName, "1");
        }


    }
    public void LoadSpecificObject(int _objectToLoad)
    {
        currentlyLoadedObjectIndex = _objectToLoad;
        GameObject objectToUse= buildingViewer ? loadedBuildings[currentlyLoadedObjectIndex] : availableObjects[currentlyLoadedObjectIndex];
        string objectName = buildingViewer ? objectToUse.name : objectToUse.GetComponent<Metadata_Plus>().name;
        if (AR)
        {
            ARObjectController.ChangeObject(objectToUse, buildingViewer);
            ARTitle.text = objectName;
        }

        else
        {
            objectController.ChangeObject(objectToUse, buildingViewer);        
            objectDetails.SetNewObjectData(objectToUse.GetComponent<Metadata_Plus>());
        }

        MetadataEfficencyAnalyser.instance.SetShapeValues(availableObjects[currentlyLoadedObjectIndex].GetComponent<Metadata_Plus>());
    }

    public void LoadByIncrementingObject(int _objectToIncrementBy)
    {

        currentlyLoadedObjectIndex = buildingViewer? (loadedBuildings.Count + currentlyLoadedObjectIndex + _objectToIncrementBy) % loadedBuildings.Count :
                (availableObjects.Count + currentlyLoadedObjectIndex + _objectToIncrementBy) % availableObjects.Count;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadedObjectDetails : MonoBehaviour
{
    [SerializeField] GameObject contentHolder;
    [SerializeField] GameObject metadataInfoPrefab;
    [SerializeField] TextMeshProUGUI objectTitle;

    public void SetNewObjectData(Metadata_Plus _objectMetadata)
    {
        foreach (Transform item in contentHolder.transform)
        {
            Destroy(item.gameObject);
        }
        foreach(var data in _objectMetadata.parameters)
        {
            GameObject _metadataInfoInstance = Instantiate(metadataInfoPrefab, contentHolder.transform);
            _metadataInfoInstance.GetComponent<MetadataUIInfoController>().SetValues(data.Key, data.Value);
        }
        objectTitle.text = _objectMetadata.name;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : BaseSelectable
{
    internal MeshFilter mFilter;
    internal MeshRenderer mRenderer;
    internal MeshCollider mCollider;

    public SelectableData data;

    public bool Init(GameObject prefab, int id)
    {
        if (data == null)
        {
            data = new SelectableData();
            data.prefabName = prefab.name;
            data.id = id;
            data.position = transform.position;
            data.yRotation = transform.rotation.eulerAngles.y;
        }

        return true;
    }

    internal void UpdatePrefab(GameObject prefab)
    {
        data.prefabName = prefab.name;
        gameObject.name = prefab.name + "_" + data.id;

        Metadata_Plus tmd = GetComponent<Metadata_Plus>();
        Metadata_Plus pfmd = prefab.GetComponent<Metadata_Plus>();
        tmd.parameters = pfmd.parameters;
        tmd.tag = pfmd.tag;

        Destroy(GetComponentInChildren<MeshRenderer>().gameObject);
        GameObject child = Instantiate(prefab.GetComponentInChildren<MeshRenderer>().gameObject, transform);
        child.layer = LayerMask.NameToLayer("Selectable");
    }

    public void DestroySelectable() 
    {
        Destroy(gameObject);
        EventManager.Instance.objectDestroyed.Invoke();
    }

    internal void UpdateMesh()
    {
        //mFilter.sharedMesh = ResourceManager.Instance.GetMesh(data.meshName);
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        //mRenderer.material = ResourceManager.Instance.GetMaterial(data.materialName);
    }
}

[System.Serializable]
public class SelectableData 
{
    public int id;
    public string prefabName;
    public float yRotation;
    public Vector3 position;
    public Metadata_Plus revitData;
}


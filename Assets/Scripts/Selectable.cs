using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;

public class Selectable : BaseSelectable
{
    internal MeshFilter mFilter;
    internal MeshRenderer mRenderer;
    internal MeshCollider mCollider;

    public SelectableData data;

    public bool Init(GameObject prefab)
    {
        //if (!TryGetComponent(out mFilter)) mFilter = gameObject.AddComponent<MeshFilter>();
        //if (!TryGetComponent(out mCollider)) mCollider = gameObject.AddComponent<MeshCollider>();
        //if (!TryGetComponent(out mRenderer)) mRenderer = gameObject.AddComponent<MeshRenderer>();

        if (data == null)
        {
            data = new SelectableData();
            //data.meshName = mesh.name;
            //data.materialName = mat.name;
            data.prefabName = prefab.name;
            data.position = transform.position;
            data.yRotation = transform.rotation.eulerAngles.y;
        }

        //mFilter.sharedMesh = mesh;
        //mCollider.sharedMesh = mesh;
        //mCollider.convex = true;
        //mRenderer.material = mat;

        return true;
    }

    internal void UpdatePrefab(GameObject prefab)
    {
        data.prefabName = prefab.name;
        gameObject.name = prefab.name + "_" + data.id;

        Metadata tmd = GetComponent<Metadata>();
        Metadata pfmd = prefab.GetComponent<Metadata>();
        tmd.parameters = pfmd.parameters;
        tmd.tag = pfmd.tag;

        Destroy(GetComponentInChildren<MeshRenderer>().gameObject);
        GameObject child = Instantiate(prefab.GetComponentInChildren<MeshRenderer>().gameObject, transform);
        child.tag = "Selectable";
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
    //public string meshName;
    //public string materialName;
    public int id;
    public string prefabName;
    public float yRotation;
    public Vector3 position;
    public Metadata revitData;
}


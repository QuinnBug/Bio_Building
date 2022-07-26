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

    public bool Init(Mesh mesh, Material mat)
    {
        if (!TryGetComponent(out mFilter)) mFilter = gameObject.AddComponent<MeshFilter>();

        if (!TryGetComponent(out mCollider)) mCollider = gameObject.AddComponent<MeshCollider>();

        if (!TryGetComponent(out mRenderer)) mRenderer = gameObject.AddComponent<MeshRenderer>();

        if (data == null)
        {
            data = new SelectableData();
            data.meshName = mesh.name;
            data.materialName = mat.name;
            data.position = transform.position;
            data.yRotation = transform.rotation.eulerAngles.y;
        }

        mFilter.sharedMesh = mesh;
        mCollider.sharedMesh = mesh;
        mCollider.convex = true;

        mRenderer.material = mat;

        return true;
    }

    internal void UpdateMesh()
    {
        mFilter.sharedMesh = ResourceManager.Instance.GetMesh(data.meshName);
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        mRenderer.material = ResourceManager.Instance.GetMaterial(data.materialName);
    }
}

[System.Serializable]
public class SelectableData 
{
    public int id;
    public string meshName;
    public string materialName;
    public float yRotation;
    public Vector3 position;
    //public MetaData revitData
}


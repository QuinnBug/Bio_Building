using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : BaseSelectable
{
    internal MeshFilter mFilter;
    internal MeshRenderer mRenderer;
    internal MeshCollider mCollider;

    public SelectableData data;

    public void Init(Mesh mesh, Material mat)
    {
        if (!TryGetComponent(out mFilter)) mFilter = gameObject.AddComponent<MeshFilter>();

        if (!TryGetComponent(out mCollider)) mCollider = gameObject.AddComponent<MeshCollider>();

        if (!TryGetComponent(out mRenderer)) mRenderer = gameObject.AddComponent<MeshRenderer>();

        data.meshName = mesh.name;
        data.materialName = mat.name;

        mFilter.mesh = mesh;
        mRenderer.material = mat;
    }
}

[System.Serializable]
public class SelectableData 
{
    public string meshName;
    public string materialName;
    public float yRotation;
    public Vector3 position;
    //public MetaData revitData
}


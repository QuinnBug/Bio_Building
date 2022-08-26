using QuinnMeshes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum PlacementState 
{
    VALID,
    INVALID,
    TOO_FAR,
    OVERLAPPING,
    NO_MESH
}

public class PlacementManager : Singleton<PlacementManager>
{
    public bool active = false;
    public bool editing = false;
    [Space]
    public bool snapToGrid = true;
    public float gridSpacing = 0.5f;
    [Space]
    public float yRotation;
    public float yRotationIncrements = 90;
    [Space]
    public bool showGrids;
    public bool continuousPlacement;
    [Space]
    public MeshRenderer placementCursor;
    public Material[] placementColours = new Material[5];
    public PlacementState currentState;
    [Space]
    public Transform selectableParent;

    private MeshCollider placementCollider;
    private MeshFilter placementCursorFilter;

    private int nextId = 0;
    private Vector3 currentPos = Vector3.zero;

    private Selectable editTarget;
    private Selectable overlapTarget;

    internal GameObject selectedPrefab;
    internal Mesh prefabMesh;
    internal Vector3 prefabMeshOffset;

    //internal Mesh selectedMesh;
    //internal Material selectedMaterial;

    private void Start()
    {
        placementCursorFilter = placementCursor.GetComponent<MeshFilter>();
        placementCollider = placementCursor.GetComponent<MeshCollider>();
    }

    void Update()
    {
        placementCursor.enabled = active && selectedPrefab != null;
        if (!active) return;

        UpdateCurrentPos();
        UpdateDisplayElements();
    }

    private void UpdateDisplayElements()
    {
        if (selectedPrefab != null)
        {
            placementCollider.sharedMesh = placementCursorFilter.sharedMesh = prefabMesh;

            if (prefabMesh.subMeshCount > 0)
            {
                Material[] mats = new Material[prefabMesh.subMeshCount];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = placementColours[(int)currentState];
                }

                placementCursor.materials = mats;
            }

            placementCursor.material = placementColours[(int)currentState];
        }

        placementCursor.transform.position = currentPos + (placementCursor.transform.rotation * prefabMeshOffset);
        placementCursor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void UpdateCurrentPos()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        LayerMask layerMask = 1 << LayerMask.NameToLayer("Floor");

        if (Physics.Raycast(ray, out hit, 25, layerMask) && !EventSystem.current.IsPointerOverGameObject() && selectedPrefab != null)
        {
            currentPos = hit.point;
            currentState = CheckValidity();
        }
        else 
        {
            currentState = PlacementState.TOO_FAR;
            return;
        }

        if (snapToGrid)
        {
            if (currentPos.x % gridSpacing != 0)
            {
                currentPos.x = Mathf.Round(currentPos.x / gridSpacing) * gridSpacing;
            }

            if (currentPos.y % gridSpacing != 0)
            {
                currentPos.y = Mathf.Round(currentPos.y / gridSpacing) * gridSpacing;
            }

            if (currentPos.z % gridSpacing != 0)
            {
                currentPos.z = Mathf.Round(currentPos.z / gridSpacing) * gridSpacing;
            }
        }
    }

    internal void ClearPlacement()
    {
        selectedPrefab = null;
        prefabMesh = null;
        prefabMeshOffset = Vector2.zero;
    }

    internal void PlacePoint()
    {
        if (currentState == PlacementState.VALID)
        {
            GameObject obj = CreateObject();

            if (obj == null)
            {
                ClearPlacement();
                return;
            }

            if (editing)
            {
                EndEdit();
                SelectionManager.Instance.hoveredObject = obj.GetComponent<Selectable>();
                SelectionManager.Instance.SelectHovered();
                Destroy(editTarget);
                return;
            }

            if (!continuousPlacement)
            {
                ClearPlacement();
            }
        }
        else if (currentState == PlacementState.OVERLAPPING && overlapTarget != null) 
        {
            overlapTarget.UpdatePrefab(selectedPrefab);

            EventManager.Instance.modelChanged.Invoke();
            //overlapTarget.data.meshName = selectedMesh.name;
            //overlapTarget.UpdateMesh();


            //if (selectedMaterial != null)
            //{
            //    overlapTarget.data.materialName = selectedMaterial.name;
            //    overlapTarget.UpdateMaterial();
            //}
        }
    }

    private GameObject CreateObject()
    {
        //GameObject obj = new GameObject(selectedMesh.name + "_" + nextId++);
        GameObject obj = Instantiate(selectedPrefab, selectableParent);
        obj.name = selectedPrefab.name + "_" + nextId++;

        obj.layer = LayerMask.NameToLayer("Selectable");
        foreach (Transform item in obj.GetComponentInChildren<Transform>())
        {
            item.gameObject.layer = obj.layer;
        }

        obj.transform.position = currentPos;
        obj.transform.rotation = Quaternion.Euler(0,yRotation,0);

        
        Selectable objSelect = obj.AddComponent<Selectable>();
        objSelect.Init(selectedPrefab);
        //objSelect.Init(selectedMesh, selectedMaterial != null ? selectedMaterial : ResourceManager.Instance.materials[0]);

        EventManager.Instance.objectPlaced.Invoke();
        return obj;
    }

    private PlacementState CheckValidity() 
    {
        if (selectedPrefab == null) return PlacementState.NO_MESH;

        //Check for overlapping other selectables
        LayerMask mask = 1 << LayerMask.NameToLayer("Selectable");

        Vector3 collCenter = placementCollider.bounds.center;

        Collider[] colliders = Physics.OverlapBox(
            collCenter,
            placementCollider.bounds.size / 2.5f,
            Quaternion.identity,
            mask);

        //Debug.DrawLine(collCenter - (placementCollider.bounds.size / 2.5f), collCenter + (placementCollider.bounds.size / 2.5f), Color.red, 1);

        //if (colliders.Length == 1 && Mathf.Abs(Vector3.Dot(placementCollider.transform.forward, colliders[0].transform.forward)) > 0.1f)
        if (colliders.Length == 1)
        {
            overlapTarget = colliders[0].GetComponent<Selectable>();

            if(overlapTarget == null) 
            {
                overlapTarget = colliders[0].transform.parent.GetComponent<Selectable>();
            }
            //Debug.Log("Overlap Target == " + overlapTarget == null ? "NULL" : overlapTarget.name);

            return PlacementState.OVERLAPPING;
        }
        else if (colliders.Length != 0) return PlacementState.INVALID;

        return PlacementState.VALID;
    }

    internal bool RecreateObject(SelectableData data)
    {
        //GameObject obj = new GameObject();
        //obj.layer = LayerMask.NameToLayer("Selectable");

        //obj.name = data.meshName + "_" + data.id.ToString();
        //obj.transform.position = data.position;
        //obj.transform.rotation = Quaternion.Euler(0, data.yRotation, 0);

        //Selectable selectable = obj.AddComponent<Selectable>();
        //selectable.data = data;

        //Mesh mesh = ResourceManager.Instance.GetMesh(selectable.data.meshName);
        //Material mat = ResourceManager.Instance.GetMaterial(selectable.data.materialName);

        //bool success = selectable.Init(mesh, mat);
        //return success;
        return false;
    }

    public void BeginEditing(Vector3 startPoint, Selectable target) 
    {
        active = true;
        editing = true;
        editTarget = target;
    }

    public void EndEdit() 
    {
        active = false;
        editing = false;
        ClearPlacement();
    }

    internal void RotatePlacement(int change)
    {
        yRotation += yRotationIncrements * change;
    }
}

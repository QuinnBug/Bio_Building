using QuinnMeshes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementManager : Singleton<PlacementManager>
{
    public bool showGrids;
    [Space]
    public bool active = false;
    public bool editing = false;
    [Space]
    public bool snapToGrid = true;
    public float gridSpacing = 0.5f;
    [Space]
    public float yRotation;
    public float yRotationIncrements = 90;
    [Space]
    public bool continuousPlacement;
    [Space]
    public MeshRenderer placementCursor;
    public Material[] placementColours = new Material[2];

    internal bool validCurrent;
    private int nextId = 0;
    private Vector3 currentPos = Vector3.zero;

    private Selectable editTarget;
    private Mesh selectedMesh;

    void Update()
    {
        placementCursor.enabled = active && selectedMesh != null;
        if (!active) return;

        UpdateCurrentPos();
        UpdateDisplayElements();
    }

    private void UpdateDisplayElements()
    {
        placementCursor.material = validCurrent ? (editing ? placementColours[2] : placementColours[1]) :  placementColours[0];
        placementCursor.transform.position = currentPos;
    }

    private void UpdateCurrentPos()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        LayerMask layerMask = 1 << LayerMask.NameToLayer("Floor");

        if (Physics.Raycast(ray, out hit, 25, layerMask) && !EventSystem.current.IsPointerOverGameObject())
        {
            currentPos = hit.point;
            validCurrent = true;
        }
        else 
        {
            validCurrent = false;
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
        selectedMesh = null;
    }

    internal void PlacePoint()
    {
        if (!validCurrent) return;

        GameObject wall = CreateObject();

        if (wall == null)
        {
            ClearPlacement();
            return;
        }

        if (editing)
        {
            EndEdit();
            SelectionManager.Instance.hoveredObject = wall.GetComponent<WallMeshComponent>();
            SelectionManager.Instance.SelectHovered();
            Destroy(editTarget);
            return;
        }

        if (!continuousPlacement)
        {
            ClearPlacement();
        }
    }

    private GameObject CreateObject()
    {
        GameObject obj = new GameObject(selectedMesh.name + "_" + nextId++);
        obj.layer = LayerMask.NameToLayer("Selectable");
        obj.transform.position = currentPos;

        Selectable objSelect = obj.AddComponent<Selectable>();
        objSelect.Init(selectedMesh, ResourceManager.Instance.materials[0]);


        return obj;
    }

    public GameObject CreateWall(Vector3 firstPos, Vector3 secondPos, float height, bool canUndo = true)
    {
        GameObject wall = new GameObject("Wall " + nextId++);
        wall.layer = LayerMask.NameToLayer("Wall");

        return wall;
    }

    internal bool RecreateWall(WallMeshData data)
    {
        GameObject wall = new GameObject();
        wall.layer = LayerMask.NameToLayer("Wall");
        WallMeshComponent wmc = wall.AddComponent<WallMeshComponent>();

        wmc.data = data;
        wall.name = "Wall " + wmc.data.id;

        bool success = wmc.Init();
        wmc.UpdateMaterial();

        return success;
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
}

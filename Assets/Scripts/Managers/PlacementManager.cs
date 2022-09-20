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
    public PlacementState placementState;
    [Space]
    public Transform selectableParent;

    private MeshCollider placementCollider;
    private MeshFilter placementCursorFilter;

    private int nextId = 0;
    private Vector3 currentPos = Vector3.zero;

    private Selectable overlapTarget;

    internal MeshCollider editCollider;
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
        if(StateManager.Instance.currentState == State.BUILD) UpdateDisplayElements();
        else if(StateManager.Instance.currentState == State.EDITING) UpdateEditObject();
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
                    mats[i] = placementColours[(int)placementState];
                }

                placementCursor.materials = mats;
            }

            placementCursor.material = placementColours[(int)placementState];
        }

        placementCursor.transform.position = currentPos + (placementCursor.transform.rotation * prefabMeshOffset);
        placementCursor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void UpdateEditObject() 
    {
        SelectionManager.Instance.editTarget.transform.position = currentPos;
        SelectionManager.Instance.editTarget.transform.rotation = Quaternion.Euler(0,yRotation,0);
    }

    private void UpdateCurrentPos()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        LayerMask layerMask = 1 << LayerMask.NameToLayer("Floor");

        if (Physics.Raycast(ray, out hit, 25, layerMask) && !EventSystem.current.IsPointerOverGameObject())
        {
            currentPos = hit.point;
            if(selectedPrefab != null) placementState = CheckValidity(placementCollider);
        }
        else 
        {
            placementState = PlacementState.TOO_FAR;
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

    public void ClearPlacement()
    {
        selectedPrefab = null;
        prefabMesh = null;
        prefabMeshOffset = Vector2.zero;
    }

    public void PlacePoint()
    {
        if (placementState == PlacementState.VALID)
        {
            GameObject obj = CreateObject();

            if (obj == null)
            {
                ClearPlacement();
                return;
            }

            if (!continuousPlacement)
            {
                ClearPlacement();
            }
        }
        else if (placementState == PlacementState.OVERLAPPING && overlapTarget != null) 
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

    public void ConfirmEdit() 
    {
        Debug.Log("Start Confirm");

        PlacementState ps = CheckValidity(editCollider);
        Debug.Log(ps);
        if (ps != PlacementState.VALID) return;

        Debug.Log("Valid Placement");

        if (!StateManager.Instance.UnlockState(State.EDITING)) return;
        StateManager.Instance.ChangeState(State.SELECT);

        SelectionManager.Instance.editTarget.data.position = currentPos;
        SelectionManager.Instance.editTarget.data.yRotation = yRotation;

        editCollider = null;
    }

    public void CancelEdit() 
    {
        if (!StateManager.Instance.UnlockState(State.EDITING)) return;
        StateManager.Instance.ChangeState(State.SELECT);

        SelectionManager.Instance.editTarget.transform.position = SelectionManager.Instance.editTarget.data.position;
        SelectionManager.Instance.editTarget.transform.rotation = Quaternion.Euler(0, SelectionManager.Instance.editTarget.data.yRotation, 0);

        editCollider = null;
    }

    private GameObject CreateObject()
    {
        GameObject obj = Instantiate(selectedPrefab, selectableParent);

        obj.layer = LayerMask.NameToLayer("Selectable");
        foreach (Transform item in obj.GetComponentInChildren<Transform>())
        {
            item.gameObject.layer = obj.layer;
        }

        obj.transform.position = currentPos;
        obj.transform.rotation = Quaternion.Euler(0,yRotation,0);

        Selectable objSelect = obj.AddComponent<Selectable>();
        objSelect.Init(selectedPrefab, nextId++);
        obj.name = selectedPrefab.name + "_" + nextId++;

        EventManager.Instance.objectPlaced.Invoke();
        return obj;
    }

    private PlacementState CheckValidity(Collider testCollider) 
    {
        if (selectedPrefab == null && editCollider == null) return PlacementState.NO_MESH;

        //Check for overlapping other selectables
        LayerMask mask = 1 << LayerMask.NameToLayer("Selectable");

        Vector3 collCenter = testCollider.bounds.center;

        Collider[] colliders = Physics.OverlapBox(
            collCenter,
            testCollider.bounds.size / 2.5f,
            Quaternion.identity,
            mask);

        Debug.DrawLine(collCenter - (testCollider.bounds.size / 2.5f), collCenter + (testCollider.bounds.size / 2.5f), Color.red, 1);

        //if (colliders.Length == 1 && Mathf.Abs(Vector3.Dot(testCollider.transform.forward, colliders[0].transform.forward)) > 0.1f)
        if (colliders.Length == 1)
        {
            if (colliders[0] == editCollider) return PlacementState.VALID;

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
        GameObject obj = Instantiate(ResourceManager.Instance.GetPrefab(data.prefabName), selectableParent);

        obj.layer = LayerMask.NameToLayer("Selectable");
        foreach (Transform item in obj.GetComponentInChildren<Transform>())
        {
            item.gameObject.layer = obj.layer;
        }

        obj.name = data.prefabName + "_" + data.id.ToString();
        obj.transform.position = data.position;
        obj.transform.rotation = Quaternion.Euler(0, data.yRotation, 0);

        Selectable selectable = obj.AddComponent<Selectable>();
        selectable.data = data;

        return true;
    }

    internal void RotatePlacement(int change)
    {
        yRotation += yRotationIncrements * change;
    }
}

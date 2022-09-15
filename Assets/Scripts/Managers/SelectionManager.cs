using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionManager : Singleton<SelectionManager>
{
    public bool active;
    [Space]
    public List<Selectable> selectedObjects;
    [Space]
    public Selectable hoveredObject;
    [Space]
    public Material selectedMat;
    public MeshFilter selectionDisplayMesh;
    public MeshRenderer selectionDisplayRenderer;
    public EditNode[] editNodes = new EditNode[3];
    [Space]
    public bool ignoreWalls = false;
    public bool ignoreFurniture = false;

    internal Selectable editTarget;

    internal SelectedType SelectedType
    {
        get
        {
            if (selectedObjects.Count > 0)
            {
                return selectedObjects[0].type;
            }
            return SelectedType.NONE;
        }

        set 
        { }
    }

    private void Start()
    {
        EventManager.Instance.stateChanged.AddListener(ClearSelection);
        selectionDisplayMesh.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (!active)
        {
            selectedObjects.Clear();
            return;
        }

        HoverUpdate();
    }

    //Called when the state changes to clear the selected item - the state change event needs a state output.
    public void ClearSelection(State state = State.SELECT) 
    {
        hoveredObject = null;
        Deselect(true);
    }

    internal void DestroySelected()
    {
        if (selectedObjects.Count <= 0) return;

        foreach (Selectable item in selectedObjects)
        {
            item.DestroySelectable();
        }

        selectedObjects.Clear();

        Deselect(true);
    }

    internal void EditSelected()
    {
        if (selectedObjects.Count != 1) return;

        editTarget = selectedObjects[0];
        MeshCollider coll = editTarget.GetComponentInChildren<MeshCollider>();
        if (coll == null) Debug.Log("No Collider");

        //some inital edit adjustments
        PlacementManager.Instance.editCollider = coll;
        PlacementManager.Instance.yRotation = editTarget.data.yRotation;

        //this will clear your selection, so make sure to do it at the end
        StateManager.Instance.ChangeState(State.EDITING, true);
    }

    public void SelectHovered(bool clearSelection = true) 
    {

        if(hoveredObject == null) return;
        //Debug.Log("Starting Selection " + clearSelection);

        if (clearSelection) Deselect(true);

        if (selectedObjects.Contains(hoveredObject))
        {
            //Debug.Log("Deselection " + hoveredObject.name);
            Deselect(false, hoveredObject);
        }
        else if (hoveredObject.type == SelectedType || SelectedType == SelectedType.NONE)
        {
            //Debug.Log("Add Selection " + hoveredObject.name);
            selectedObjects.Add(hoveredObject);
        }
        else return;

        UpdateSelectDisplay();
    }

    private void UpdateSelectDisplay()
    {
        if (selectedObjects.Count == 0)
        {
            selectionDisplayMesh.gameObject.SetActive(false);
            selectionDisplayMesh.sharedMesh = null;
            return;
        }

        selectionDisplayMesh.gameObject.SetActive(true);

        if (selectedObjects.Count > 1)
        {
            selectionDisplayMesh.transform.position = Vector3.zero;
            selectionDisplayMesh.transform.rotation = Quaternion.Euler(0,0,0);

            List<CombineInstance> combine = new List<CombineInstance>();
            CombineInstance c = new CombineInstance();

            foreach (BaseSelectable item in selectedObjects)
            {
                MeshFilter _mFilter;
                if (!item.TryGetComponent(out _mFilter))
                {
                    _mFilter = item.GetComponentInChildren<MeshFilter>();
                }

                if (_mFilter != null)
                {
                    c.mesh = _mFilter.sharedMesh;
                    c.transform = _mFilter.transform.localToWorldMatrix;
                    combine.Add(c);
                }
            }

            if (combine.Count > 0)
            {
                selectionDisplayMesh.sharedMesh = new Mesh();
                selectionDisplayMesh.sharedMesh.CombineMeshes(combine.ToArray(), true, true);
            }
        }
        else
        {
            MeshFilter _mFilter;

            if (!selectedObjects[0].TryGetComponent(out _mFilter))
            {
                _mFilter = selectedObjects[0].GetComponentInChildren<MeshFilter>();
            }

            if (_mFilter != null)
            {
                selectionDisplayMesh.sharedMesh = _mFilter.sharedMesh;
                selectionDisplayMesh.transform.position = _mFilter.transform.position;
                selectionDisplayMesh.transform.rotation = _mFilter.transform.rotation;
            }
        }

        selectionDisplayMesh.transform.localScale = selectedObjects[0].transform.localScale;

        if (selectionDisplayMesh != null)
        {

            if (selectionDisplayMesh.sharedMesh.subMeshCount > 0)
            {
                Material[] mats = new Material[selectionDisplayMesh.sharedMesh.subMeshCount];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = selectedMat;
                }

                selectionDisplayRenderer.materials = mats;
            }

            selectionDisplayRenderer.material = selectedMat;
        }
    }

    internal void Deselect(bool all = false, Selectable item = null)
    {
        if (all)
        {
            selectedObjects.Clear();
        }
        else
        {
            if (selectedObjects.Count == 0) return;
            if (item == null) item = selectedObjects[0];
            selectedObjects.Remove(item);
        }

        UpdateSelectDisplay();
    }

    public void HoverUpdate() 
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Selectable newHoveredObj = null;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        LayerMask layerMask = 1 << LayerMask.NameToLayer("Selectable");

        if (Physics.Raycast(ray, out hit, 25, layerMask))
        {
            if(!hit.collider.gameObject.TryGetComponent(out newHoveredObj)) 
            {
                hit.collider.transform.parent.TryGetComponent(out newHoveredObj);
            }
        }
        
        if (newHoveredObj != hoveredObject)
        {
            //update the hovered ui, update the outline, etc
            hoveredObject = newHoveredObj;
        }

    }

    public void OnDrawGizmos()
    {
        if (hoveredObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(hoveredObject.transform.position, Vector3.one * 0.25f);
        }
    }
}



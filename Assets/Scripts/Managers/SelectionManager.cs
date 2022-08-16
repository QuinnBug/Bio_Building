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
    public MeshFilter selectionDisplayMesh;
    public EditNode[] editNodes = new EditNode[3];
    [Space]
    public bool ignoreWalls = false;
    public bool ignoreFurniture = false;
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
            item.Destroy();
        }

        selectedObjects.Clear();

        Deselect(true);
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

        UpdateEditNodes();
        UpdateSelectDisplay();
    }

    private void UpdateSelectDisplay()
    {
        if (selectedObjects.Count == 0)
        {
            selectionDisplayMesh.gameObject.SetActive(false);
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
                if (item.TryGetComponent(out MeshFilter _mFilter))
                {
                    c.mesh = _mFilter.sharedMesh;
                    c.transform = _mFilter.transform.localToWorldMatrix;
                    combine.Add(c);
                }
            }

            if (combine.Count > 0)
            {
                selectionDisplayMesh.sharedMesh = new Mesh();
                selectionDisplayMesh.sharedMesh.CombineMeshes(combine.ToArray());
            }
        }
        else
        {
            if(selectedObjects[0].TryGetComponent(out MeshFilter _mFilter)) selectionDisplayMesh.sharedMesh = _mFilter.sharedMesh;
            selectionDisplayMesh.transform.position = selectedObjects[0].transform.position;
            selectionDisplayMesh.transform.rotation = selectedObjects[0].transform.rotation;
        }

        selectionDisplayMesh.transform.localScale = selectedObjects[0].transform.localScale;
    }

    private void UpdateEditNodes()
    {
        foreach (EditNode node in editNodes)
        {
            node.gameObject.SetActive(false);
        }

        if (selectedObjects.Count != 1) return;

        switch (SelectedType)
        {

            case SelectedType.WALL:
            case SelectedType.COLUMN:
                //just need the move node in the center of the object
                editNodes[0].gameObject.SetActive(true);
                editNodes[0].transform.position = selectedObjects[0].transform.position;
                break;

            default:
                return;
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

        UpdateEditNodes();
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
            hit.collider.gameObject.TryGetComponent(out newHoveredObj);
        }
        
        if (newHoveredObj != hoveredObject)
        {
            //update the hovered ui, update the outline, etc
            hoveredObject = newHoveredObj;
        }

    }

    internal void MoveSelectable()
    {
        switch (SelectedType)
        {
            case SelectedType.WALL:
            case SelectedType.COLUMN:
                break;
            default:
                break;
        }
    }

    internal void EditSelectable(Vector3 position)
    {
        switch (SelectedType)
        {
            case SelectedType.WALL:
                //used to be able to change length, in the new system not sure if this function matters anymore
                break;

            default:
                return;
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



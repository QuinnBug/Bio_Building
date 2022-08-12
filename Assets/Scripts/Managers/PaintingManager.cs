using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PaintState 
{
    SINGLE,
    ADJACENT,
    ALL_CONNECTED
}

public class PaintingManager : Singleton<PaintingManager>
{
    public bool active;
    public PaintState paintState;
    public TMP_Dropdown stateSelector;
    [Space]
    public Material selectedMat;

    public Selectable hoveredObject;
    public List<Selectable> connectedObjects = new List<Selectable>();

    // Update is called once per frame
    void Update()
    {
        active = StateManager.Instance.currentState == State.DECORATE;

        if (!active) return;

        selectedMat = PlacementManager.Instance.selectedMaterial;
        paintState = (PaintState)stateSelector.value;
        TargetUpdates();
    }

    public void PaintTargets() 
    {
        if (hoveredObject == null || selectedMat == null) return;

        hoveredObject.data.materialName = selectedMat.name;
        hoveredObject.UpdateMaterial();
        EventManager.Instance.objectPainted.Invoke();

        if (connectedObjects.Count > 0)
        {
            foreach (Selectable item in connectedObjects)
            {
                item.data.materialName = selectedMat.name;
                item.UpdateMaterial();
            }
        }
    }

    public void TargetUpdates() 
    {
        SelectionManager.Instance.HoverUpdate();

        if (hoveredObject == SelectionManager.Instance.hoveredObject) return;

        hoveredObject = SelectionManager.Instance.hoveredObject;

        if (hoveredObject == null) return;

        switch (paintState)
        {
            case PaintState.ADJACENT:
                AdjacentSelectablesUpdate();
                break;

            case PaintState.ALL_CONNECTED:
                connectedObjects = GetAllConnectedToHovered();
                break;

            case PaintState.SINGLE:
            default:
                connectedObjects.Clear();
                return;
        }
    }

    private List<Selectable> GetAllConnectedToHovered()
    {
        List<Selectable> connecteds = new List<Selectable>();
        
        //checked list, unchecked list, connecteds list, pathfinding logic

        return connecteds;
    }

    private void AdjacentSelectablesUpdate()
    {
        connectedObjects.Clear();

        if (hoveredObject == null) return;

        int adjacentCount = 0;

        //raycast to the left
        //if valid hit, make hit.collider the current and loop
        Selectable current = hoveredObject;
        while (FetchAdjacentsInDirection(-hoveredObject.transform.right, current, out current)) { adjacentCount++; }

        //raycast to the right
        //if valid hit, make hit.collider the current and loop
        current = hoveredObject;
        while (FetchAdjacentsInDirection(hoveredObject.transform.right, current, out current)) { adjacentCount++; }

        //Debug.Log("Adjacent Count = " + adjacentCount);
    }

    bool FetchAdjacentsInDirection(Vector3 direction, Selectable current, out Selectable newCurrent) 
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        float maxDist = PlacementManager.Instance.gridSpacing * 1.5f;
        LayerMask mask = 1 << LayerMask.NameToLayer("Selectable");

        //Debug.DrawLine(current.mCollider.bounds.center, current.mCollider.bounds.center + direction * maxDist, Color.red, 15);

        hits.Clear();
        hits.AddRange(Physics.RaycastAll(current.mCollider.bounds.center, direction, maxDist, mask));
        if (hits.Count > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                //if it's not the same collider && the colliders are the same orientation
                if (hit.collider != current.mCollider && Mathf.Abs(Vector3.Dot(current.transform.forward, hit.collider.transform.forward)) > 0.1f)
                {
                    newCurrent = hit.collider.GetComponent<Selectable>();
                    connectedObjects.Add(newCurrent);
                    return true;
                }
            }

        }
        newCurrent = null;
        return false;
    }
}

using QuinnMeshes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WallPlacementManager : Singleton<WallPlacementManager>
{
    public bool showGrids;
    public bool allowHitWall;
    [Space]
    public bool active = false;
    public bool editing = false;
    [Space]
    public bool lockToAngle = true;
    public float angleSnap = 45.0f;
    [Space]
    public bool snapToWall = true;
    public float snapDistance = 0.5f;
    [Space]
    public bool snapToGrid = true;
    public float gridSpacing = 0.25f;
    [Space]
    public bool continuousPlacement;

    

    [Space]
    public LineRenderer drawingLine;
    public SpriteRenderer placementCursor;
    public Color[] placementColours = new Color[2];
    [Space]
    public float currentHeight;
    public float maxHeight;

    internal bool validCurrent;
    private bool placingSecond;
    private int nextId = 0;
    private Vector3 currentPos = Vector3.zero;
    private Vector3 prevCurrentPos = Vector3.zero;
    private Vector3 firstPos = Vector3.zero;
    private Vector3 secondPos = Vector3.zero;

    private WallMeshComponent wallInEdit;

    void Update()
    {
        placementCursor.enabled = active;
        drawingLine.enabled = active;
        if (!active) return;

        UpdateCurrentPos();
        UpdateDisplayElements();
    }

    private void UpdateDisplayElements()
    {
        placementCursor.color = validCurrent ? (editing ? placementColours[2] : placementColours[1]) :  placementColours[0];
        placementCursor.transform.position = currentPos + (Vector3.up * 0.001f);

        Vector3 wallHeight = Vector3.up * currentHeight;

        drawingLine.positionCount = placingSecond ? 5 : 2;

        if (placingSecond)
        {
            drawingLine.SetPosition(0, firstPos + wallHeight);
            drawingLine.SetPosition(1, firstPos);
            drawingLine.SetPosition(2, placementCursor.transform.position);
            drawingLine.SetPosition(3, placementCursor.transform.position + wallHeight);
            drawingLine.SetPosition(4, drawingLine.GetPosition(0));
        }
        else
        {
            drawingLine.SetPosition(0, placementCursor.transform.position);
            drawingLine.SetPosition(1, placementCursor.transform.position + wallHeight);
        }

        drawingLine.startColor = placementCursor.color;
        drawingLine.endColor = placementCursor.color;
    }

    private void UpdateCurrentPos()
    {
        prevCurrentPos = currentPos;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        LayerMask layerMask = 1 << LayerMask.NameToLayer("Floor");
        if(allowHitWall) layerMask |= 1 << LayerMask.NameToLayer("Wall");

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

        if (snapToWall)
        {
            Vector3 closestPoint = Vector3.positiveInfinity;
            float closestDistance = 999;

            Collider[] colliders = Physics.OverlapSphere(currentPos, snapDistance);
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out WallMeshComponent wmc))
                {
                    Vector3 wallClosestPoint = wmc.GetClosestVertexWorldPos(currentPos);

                    if (Vector3.Distance(wallClosestPoint, currentPos) < closestDistance)
                    {
                        closestDistance = Vector3.Distance(wallClosestPoint, currentPos);
                        closestPoint = wallClosestPoint;
                    }
                }
            }

            if (closestDistance < snapDistance)
            {
                currentPos = closestPoint;
            }
        }

        if (lockToAngle && placingSecond)
        {
            float angle = Vector3.SignedAngle(Vector3.forward, currentPos - firstPos, Vector3.up);
            float excessAngle = angle % angleSnap;
            float distance = Vector3.Distance(currentPos, firstPos);

            float targetAngle = Mathf.Round(angle/angleSnap) * angleSnap;
            targetAngle += 90;
            targetAngle *= Mathf.Deg2Rad;

            currentPos.x = firstPos.x + (Mathf.Cos(targetAngle) * distance * -1);
            currentPos.z = firstPos.z + (Mathf.Sin(targetAngle) * distance);
        }

        if (snapToGrid)
        {
            if (currentPos.x % gridSpacing != 0)
            {
                currentPos.x = Mathf.Round(currentPos.x / gridSpacing) * gridSpacing;
            }

            if (currentPos.z % gridSpacing != 0)
            {
                currentPos.y = Mathf.Round(currentPos.y / gridSpacing) * gridSpacing;
            }

            if (currentPos.z % gridSpacing != 0)
            {
                currentPos.z = Mathf.Round(currentPos.z / gridSpacing) * gridSpacing;
            }
        }

        if (allowHitWall && placingSecond) currentPos.y = firstPos.y; 
    }

    internal void ClearPlacement()
    {
        placingSecond = false;
        firstPos = Vector3.zero;
        secondPos = Vector3.zero;
    }

    internal void PlacePoint()
    {
        if (!validCurrent) return;

        if (placingSecond)
        {
            secondPos = currentPos;

            GameObject wall = CreateWall(firstPos, secondPos, !editing ? currentHeight : wallInEdit.data.height);

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
                Destroy(wallInEdit.gameObject);
                return;
            }

            if (continuousPlacement)
            {
                firstPos = secondPos;
            }
            else
            {
                ClearPlacement();
            }
        }
        else
        {
            firstPos = currentPos;
            placingSecond = true;
        }
    }

    public void ChangeHeight(float change) 
    {
        currentHeight = Mathf.Clamp(currentHeight + change, 1, maxHeight);
    }

    public GameObject CreateWall(Vector3 firstPos, Vector3 secondPos, float height, bool canUndo = true)
    {
        GameObject wall = new GameObject();
        wall.layer = LayerMask.NameToLayer("Wall");
        WallMeshComponent wmc = wall.AddComponent<WallMeshComponent>();

        wmc.SetValues(firstPos, secondPos, height, !editing ? nextId : wallInEdit.data.id);
        wall.name = "Wall " + wmc.data.id;

        if (wmc.Init()) 
        {
            if (!editing) nextId++;
            else wmc.OverrideExtraData(wallInEdit.data);

            if (canUndo) ActionManager.Instance.actionEvent.Invoke(ActionType.PLACE_WALL, wall);
            return wall;
        }

        return null;
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

    public void BeginEditing(Vector3 startPoint, WallMeshComponent wmc) 
    {
        active = true;
        editing = true;
        firstPos = startPoint;
        placingSecond = true;
        wallInEdit = wmc;
    }

    public void EndEdit() 
    {
        active = false;
        editing = false;
        ClearPlacement();
    }
}

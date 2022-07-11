using QuinnMeshes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementManager : Singleton<PlacementManager>
{
    public bool validCurrent;
    [Space]
    public bool lockToAngle = true;
    public float angleSnap = 45.0f;
    [Space]
    public bool snapToWall = true;
    public float snapDistance = 0.5f;
    [Space]
    public LineRenderer drawingLine;
    public SpriteRenderer placementCursor;
    public Color[] placementColours = new Color[2];
    [Space]
    public float currentHeight;
    public float maxHeight;

    private bool placingSecond;
    private Vector3 currentPos;
    private Vector3 prevCurrentPos;
    private Vector3 firstPos = Vector3.zero;
    private Vector3 secondPos = Vector3.zero;


    void Update()
    {
        UpdateCurrentPos();
        UpdateDisplayElements();
    }

    private void UpdateDisplayElements()
    {
        placementCursor.color = validCurrent ? placementColours[0] : placementColours[1];
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

        if (Physics.Raycast(ray, out hit, 10, layerMask))
        {
            currentPos = hit.point;
        }
        else 
        {
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
                        Debug.Log("Successful Snap attempt - " + wallClosestPoint);
                        closestDistance = Vector3.Distance(wallClosestPoint, currentPos);
                        closestPoint = wallClosestPoint;
                    }
                }
            }

            if (closestDistance < snapDistance)
            {
                Debug.Log("Snap successful");
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
            Debug.Log(angle + " % " + excessAngle + " == " + targetAngle);
            targetAngle *= Mathf.Deg2Rad;

            currentPos.x = firstPos.x + (Mathf.Cos(targetAngle) * distance * -1);
            currentPos.z = firstPos.z + (Mathf.Sin(targetAngle) * distance);
        }

        validCurrent = currentPos != Vector3.up * -1;
    }

    internal void PlacePoint()
    {
        if (!validCurrent) return;

        if (placingSecond)
        {
            secondPos = currentPos;

            

            CreateWall(firstPos, secondPos, currentHeight);
            placingSecond = false;
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

    private void CreateWall(Vector3 firstPos, Vector3 secondPos, float height)
    {
        GameObject wall = new GameObject("Wall");
        WallMeshComponent wmc = wall.AddComponent<WallMeshComponent>();
        wmc.SetValues(firstPos, secondPos, height);
        wmc.Init();
    }
}

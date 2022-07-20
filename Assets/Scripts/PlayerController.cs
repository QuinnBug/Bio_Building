using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Command 
{
    NONE,
    SELECT,
    MULTI_SELECT,
    CANCEL
}
public class PlayerController : Singleton<PlayerController>
{
    public Camera cam;
    public float speed;
    public float sprintMod;
    public float heightChangeStep;

    Vector3 moveInput;
    public Command latestCommand = Command.NONE;
    bool processInput;

    Vector3 perspCamRot;

    bool orthographicMode = false;

    private void Start()
    {
        perspCamRot = cam.transform.rotation.eulerAngles;
    }

    void Update()
    {
        MovementUpdate();
        if (processInput) InputProcessing();
    }

    private void InputProcessing()
    {
        switch (latestCommand)
        {
            case Command.SELECT:
                switch (StateManager.Instance.currentState)
                {
                    case State.ROOM_BUILD:
                        WallPlacementManager.Instance.PlacePoint();
                        break;

                    case State.FURNITURE_BUILD:
                        //place furniture
                        break;

                    default:
                        if (WallPlacementManager.Instance.editing)
                        {
                            WallPlacementManager.Instance.PlacePoint();
                        }
                        else
                        {
                            if (!CheckEditNodeHit()) SelectionManager.Instance.SelectHovered();
                        }
                        break;
                }
                break;

            case Command.MULTI_SELECT:
                switch (StateManager.Instance.currentState)
                {
                    case State.ROOM_BUILD:

                        break;

                    case State.FURNITURE_BUILD:

                        break;

                    default:
                        if (WallPlacementManager.Instance.editing)
                        {
                            WallPlacementManager.Instance.PlacePoint();
                        }
                        else
                        {
                            SelectionManager.Instance.SelectHovered(false);
                        }
                        break;
                }
                break;

            case Command.CANCEL:
                switch (StateManager.Instance.currentState)
                {
                    case State.ROOM_BUILD:
                        WallPlacementManager.Instance.ClearPlacement();
                        break;
                    case State.FURNITURE_BUILD:
                        break;
                    default:
                        if (WallPlacementManager.Instance.editing) WallPlacementManager.Instance.EndEdit();
                        else SelectionManager.Instance.Deselect(true);
                        break;
                }
                break;
            default:
                break;
        }

        latestCommand = Command.NONE;
    }

    private bool CheckEditNodeHit()
    {
        if (SelectionManager.Instance.selectedObjects.Count == 0) return false;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        LayerMask layerMask = 1 << LayerMask.NameToLayer("EditNode");

        if (Physics.Raycast(ray, out hit, 25, layerMask) && hit.collider.gameObject.TryGetComponent(out EditNode node))
        {
            node.OnClick();
            return true;
        }

        return false;
    }

    void MovementUpdate() 
    {
        Vector3 movement = moveInput;

        if (orthographicMode)
        {
            cam.orthographicSize += movement.y * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1, 20);
            movement.y = 0;
        }

        transform.Translate(movement * Time.deltaTime);
    }

    void OrthoToggle() 
    {
        orthographicMode = !orthographicMode;

        cam.orthographic = orthographicMode;

        //change to be -> set cam TARGET pos and setup a Cam update that moves it towards it's target local pos & rot

        cam.transform.rotation = orthographicMode ? Quaternion.Euler(90, 0, 0) : Quaternion.Euler(perspCamRot);

        if (orthographicMode)
        {
            Vector3 pos = cam.transform.position;
            pos.y = 8;
            cam.transform.position = pos;
        }
    }

    #region Input Functions
    public void MovementInput(InputAction.CallbackContext context) 
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput.x = input.x;
        moveInput.z = input.y;
    }

    public void FlyInput(InputAction.CallbackContext context) 
    {
        moveInput.y = context.ReadValue<float>();
    }

    public void CamChangeInput(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            OrthoToggle();
        }
    }

    public void MultiSelectInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started) 
        {
            latestCommand = Command.MULTI_SELECT;
            processInput = true;
        }
    }

    public void SelectInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && latestCommand != Command.MULTI_SELECT)
        {
            latestCommand = Command.SELECT;
            processInput = true;
        }
    }

    public void CancelInput(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            latestCommand = Command.CANCEL;
            processInput = true;
        }
    }

    public void HeightInput(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            int change = (int)context.ReadValue<float>();
            WallPlacementManager.Instance.ChangeHeight(change * heightChangeStep);
        }
    }
    #endregion
}

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
    CANCEL,
    DELETE,
}
public class PlayerController : Singleton<PlayerController>
{
    public Camera cam;
    public float speed;
    public float rotSpeed;
    public float lookSpeed;
    [Space]
    public Vector2 nonOrthoOffset;
    public float orthoOffset;
    public float camChangeDistance;

    Vector3 moveInput;
    public Command latestCommand = Command.NONE;
    bool processInput;

    bool orthographicMode = false;

    Vector3 camLookAtPosition;
    Vector3 camTargetPosition;

    bool transitioning = false;

    private void Start()
    {
        OrthoToggle();
    }

    void Update()
    {
        if (transitioning) 
        {
            transform.position = Vector3.MoveTowards(transform.position, camTargetPosition, rotSpeed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(camLookAtPosition - camTargetPosition, Vector3.up);
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, targetRot, lookSpeed * Time.deltaTime);
            //cam.transform.rotation = targetRot;

            if(Quaternion.Angle(cam.transform.rotation, targetRot) < 0.1f  && Vector3.Distance(transform.position, camTargetPosition) < 0.1f)
            {
                //cam.orthographic = orthographicMode;
                transitioning = false;
            }

            return;
        }

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
                    case State.BUILD:
                        PlacementManager.Instance.PlacePoint();
                        break;

                    case State.DECORATE:
                        PaintingManager.Instance.PaintTargets();
                        break;

                    default:
                        if (PlacementManager.Instance.editing)
                        {
                            PlacementManager.Instance.PlacePoint();
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
                    case State.BUILD:

                        break;

                    case State.DECORATE:

                        break;

                    default:
                        if (PlacementManager.Instance.editing)
                        {
                            PlacementManager.Instance.PlacePoint();
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
                    case State.BUILD:
                        PlacementManager.Instance.ClearPlacement();
                        break;
                    case State.DECORATE:
                        break;
                    default:
                        if (PlacementManager.Instance.editing) PlacementManager.Instance.EndEdit();
                        else SelectionManager.Instance.Deselect(true);
                        break;
                }
                break;

            case Command.DELETE:
                switch (StateManager.Instance.currentState)
                {
                    case State.BUILD:
                        SelectionManager.Instance.HoverUpdate();
                        if (SelectionManager.Instance.hoveredObject != null) Destroy(SelectionManager.Instance.hoveredObject.gameObject);
                        break;

                    case State.DECORATE:
                        break;

                    case State.SELECT:
                    default:
                        
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

        if (cam.orthographic)
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
        transitioning = true;

        //change to be -> set cam TARGET pos and setup a Cam update that moves it towards it's target local pos & rot

        camLookAtPosition = cam.transform.position + (cam.transform.forward * camChangeDistance);
        camLookAtPosition.y = 0;

        //cam.transform.rotation = orthographicMode ? Quaternion.Euler(90, 0, 0) : Quaternion.Euler(perspCamRot);

        if (orthographicMode)
        {
            camTargetPosition = camLookAtPosition + (Vector3.up * orthoOffset);
        }
        else
        {
            camTargetPosition = camLookAtPosition + (Vector3.up * nonOrthoOffset.y) + (transform.forward * nonOrthoOffset.x);
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

    public void DeleteInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            latestCommand = Command.DELETE;
            processInput = true;
        }
    }

    public void RotationInput(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            int change = (int)context.ReadValue<float>();
            PlacementManager.Instance.RotatePlacement(change);
        }
    }
    #endregion
}

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
    public float sprintSpeed;
    private bool sprinting;
    [Space]
    public float rotSpeed;
    public float lookSpeed;
    [Space]
    public Vector3 topDownViewPos;
    public Vector3 standardPos;
    [Space]
    public Bounds topDownBounds;
    public Bounds standardBounds;
    //public float camChangeDistance;
    //public Vector2 nonOrthoOffset;
    //public float orthoOffset;

    Vector3 moveInput;
    public Command latestCommand = Command.NONE;
    bool processInput;

    bool orthographicMode = false;

    Vector3 camLookAtPosition;
    Vector3 camTargetPosition;

    bool transitioning = false;

    private void Start()
    {
        orthographicMode = true;
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

            if(Quaternion.Angle(cam.transform.rotation, targetRot) < 0.01f  && Vector3.Distance(transform.position, camTargetPosition) < 0.01f)
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

                    case State.EDITING:
                        PlacementManager.Instance.ConfirmEdit();
                        break;

                    case State.SELECT:
                        SelectionManager.Instance.SelectHovered();
                        break;
                }
                break;

            case Command.MULTI_SELECT:
                switch (StateManager.Instance.currentState)
                {
                    case State.SELECT:
                        SelectionManager.Instance.SelectHovered(false);
                        break;

                    default:
                        break;
                }
                break;

            case Command.CANCEL:
                switch (StateManager.Instance.currentState)
                {
                    case State.BUILD:
                        PlacementManager.Instance.ClearPlacement();
                        break;

                    case State.EDITING:
                        PlacementManager.Instance.CancelEdit();
                        break;

                    case State.SELECT:
                        SelectionManager.Instance.Deselect(true);
                        break;

                    default:
                        break;
                }
                break;

            case Command.DELETE:
                switch (StateManager.Instance.currentState)
                {
                    case State.BUILD:
                        SelectionManager.Instance.HoverUpdate();
                        if (SelectionManager.Instance.hoveredObject != null) SelectionManager.Instance.hoveredObject.DestroySelectable();
                        break;

                    case State.EDITING:
                        //Destroy the edit target
                        Debug.Log("Quick Edit Delete Not Implemented");
                        break;

                    case State.SELECT:
                        SelectionManager.Instance.DestroySelected();
                        break;

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
        Vector3 movement = moveInput * (sprinting ? sprintSpeed : speed);

        if (cam.orthographic)
        {
            cam.orthographicSize += movement.y * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1, 20);
            movement.y = 0;
        }

        Bounds currentBounds = orthographicMode ? topDownBounds : standardBounds;

        movement *= Time.deltaTime;

        if (!currentBounds.Contains(transform.position + (movement.x * Vector3.right))) movement.x = 0;
        if (!currentBounds.Contains(transform.position + (movement.y * Vector3.up))) movement.y = 0;
        if (!currentBounds.Contains(transform.position + (movement.z * Vector3.forward))) movement.z = 0;

        transform.Translate(movement);

        float x = Mathf.Clamp(transform.position.x,
            (currentBounds.center.x - currentBounds.extents.x) + 0.05f,
            (currentBounds.center.x + currentBounds.extents.x) - 0.05f);

        float y = Mathf.Clamp(transform.position.y,
            (currentBounds.center.y - currentBounds.extents.y) + 0.05f,
            (currentBounds.center.y + currentBounds.extents.y) - 0.05f);

        float z = Mathf.Clamp(transform.position.z,
            (currentBounds.center.z - currentBounds.extents.z) + 0.05f,
            (currentBounds.center.z + currentBounds.extents.z) - 0.05f);

        transform.position = new Vector3(x, y, z);
        
    }

    void OrthoToggle() 
    {
        if (transitioning) return;
        orthographicMode = !orthographicMode;
        transitioning = true;

        //change to be -> set cam TARGET pos and setup a Cam update that moves it towards it's target local pos & rot

        //camLookAtPosition = cam.transform.position + (cam.transform.forward * camChangeDistance);
        //camLookAtPosition.y = 0;

        //cam.transform.rotation = orthographicMode ? Quaternion.Euler(90, 0, 0) : Quaternion.Euler(perspCamRot);

        //if (orthographicMode)
        //{
        //    camTargetPosition = camLookAtPosition + (Vector3.up * orthoOffset);
        //}
        //else
        //{
        //    camTargetPosition = camLookAtPosition + (Vector3.up * nonOrthoOffset.y) + (transform.forward * nonOrthoOffset.x);
        //}

        camTargetPosition = orthographicMode ? topDownViewPos : standardPos;
        camLookAtPosition = Vector3.zero;

        EventManager.Instance.orthoToggle.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(topDownBounds.center, topDownBounds.extents * 2);

        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(standardBounds.center, standardBounds.extents * 2);
    }

    #region Input Functions
    public void MovementInput(InputAction.CallbackContext context) 
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput.x = input.x;
        moveInput.z = input.y;
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        sprinting = context.phase != InputActionPhase.Canceled;
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

    public void NumberInput(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Performed) 
        {
            float num = context.ReadValue<float>();
            int flatNum = (int)num;
            //probably use this to select actions for selected objects.

            //State newState = (State)flatNum - 1;
            //if (newState == State.SELECT || newState == State.BUILD) StateManager.Instance.ChangeState(newState);
        }
    }
    #endregion
}

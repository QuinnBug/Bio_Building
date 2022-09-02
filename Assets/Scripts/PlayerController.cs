using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


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
    private CinemachineVirtualCamera cam;
    private CinemachineTrackedDolly dolly;
    public float speed;
    public float sprintSpeed;
    private bool sprinting;
    [Space]
    public float lookSpeed;
    public float minDiff;
    [Space]
    public Bounds topDownBounds;
    public Bounds standardBounds;

    Vector3 moveInput;
    public Command latestCommand = Command.NONE;
    bool processInput;

    bool orthographicMode = false;

    float pathPosTarget;

    bool transitioning = false;

    private void Start()
    {
        cam = CamManager.Instance.playCam;
        dolly = cam.GetCinemachineComponent<CinemachineTrackedDolly>();

        orthographicMode = true;
        OrthoToggle();

    }

    void Update()
    {
        if (transitioning) 
        {
            dolly.m_PathPosition = Mathf.Lerp(dolly.m_PathPosition, pathPosTarget, lookSpeed * Time.deltaTime);
            if (Mathf.Abs(dolly.m_PathPosition - pathPosTarget) <= minDiff)
            {
                dolly.m_PathPosition = pathPosTarget;
                transitioning = false;
            }
            else return;
        }

        if (StateManager.Instance.currentState != State.EVALUATE)
        {
            MovementUpdate();
            if (processInput) InputProcessing();
        }
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

        //if (cam.orthographic)
        //{
        //    cam.orthographicSize += movement.y * Time.deltaTime;
        //    cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1, 20);
        //    movement.y = 0;
        //}

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

        dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
        pathPosTarget = orthographicMode ? 0 : 1;

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
        if (StateManager.Instance.currentState == State.EVALUATE) return;

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
        if (StateManager.Instance.currentState == State.EVALUATE) return;

        if (context.phase == InputActionPhase.Started)
        {
            int change = (int)context.ReadValue<float>();
            PlacementManager.Instance.RotatePlacement(change);
        }
    }

    public void NumberInput(InputAction.CallbackContext context) 
    {
        if (StateManager.Instance.currentState == State.EVALUATE) return;

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

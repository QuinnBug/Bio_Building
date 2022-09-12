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
    public CinemachineVirtualCamera cam;
    public CinemachineSmoothPath basePath;
    public CinemachineSmoothPath topPath;
    [Space]
    public Range heightRange;
    public Range zoomRange;
    public Vector2 cursorSpeed;
    public float moveSpeed;
    public float scrollSpeed;
    public float zoomSpeed;
    [Space]
    public Command latestCommand = Command.NONE;

    private CinemachineTrackedDolly dolly;
    [SerializeField] float zoomDistance = 1;
    [SerializeField] float height = 3;
    [SerializeField] float normalisedDollyVal;
    private Vector2 lastClickPos;
    bool processInput;
    bool orthographicMode = false;

    private void Start()
    {
        //cam2 = CamManager.Instance.playCam;
        //dolly2 = cam2.GetCinemachineComponent<CinemachineTrackedDolly>();

        //orthographicMode = true;
        //OrthoToggle();

        cam = CamManager.Instance.playCam;
        dolly = cam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
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

    void MovementUpdate() 
    {
        if (Mouse.current.leftButton.ReadValue() > 0)
        {
            Vector2 diff = Mouse.current.position.ReadValue() - lastClickPos;

            if(!orthographicMode) height -= cursorSpeed.y * diff.y * Time.deltaTime;
            height = heightRange.Clamp(height);

            normalisedDollyVal += cursorSpeed.x * diff.x * Time.deltaTime;
            //if (normalisedDollyVal > 1) normalisedDollyVal -= 1;
            //if (normalisedDollyVal < 0) normalisedDollyVal += 1;

            lastClickPos = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.scroll.ReadValue().y != 0)
        {
            if (!orthographicMode) zoomDistance -= scrollSpeed * Mouse.current.scroll.ReadValue().y * Time.deltaTime;
            zoomDistance = zoomRange.Clamp(zoomDistance);
        }

        dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
        dolly.m_PathPosition = Mathf.Lerp(dolly.m_PathPosition, normalisedDollyVal, moveSpeed * Time.deltaTime);

        basePath.transform.position = Vector3.Lerp(basePath.transform.position, height * Vector3.up, moveSpeed * Time.deltaTime);
        basePath.transform.localScale = Vector3.Lerp(basePath.transform.localScale, zoomDistance * Vector3.one, zoomSpeed * Time.deltaTime);
    }

    void OrthoToggle() 
    {
        orthographicMode = !orthographicMode;

        dolly.m_Path = orthographicMode ? topPath : basePath;

        EventManager.Instance.orthoToggle.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawCube(topDownBounds.center, topDownBounds.extents * 2);

        //Gizmos.color = Color.magenta;
        //Gizmos.DrawCube(standardBounds.center, standardBounds.extents * 2);
    }

    #region Input Functions
    public void MovementInput(InputAction.CallbackContext context) 
    {
        Vector2 input = context.ReadValue<Vector2>();
        //moveInput.x = input.x;
        //moveInput.z = input.y;
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        //sprinting = context.phase != InputActionPhase.Canceled;
    }

    public void FlyInput(InputAction.CallbackContext context) 
    {
        //moveInput.y = context.ReadValue<float>();
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
            lastClickPos = Mouse.current.position.ReadValue();

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

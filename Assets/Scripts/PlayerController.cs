using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public GameObject camGo;
    public float speed;
    public float sprintMod;
    public float heightChangeStep;

    Vector3 moveInput;
    bool controlInput;

    void Update()
    {
        MovementUpdate();
    }

    void MovementUpdate() 
    {
        Vector3 movement = moveInput;
        movement *= controlInput ? sprintMod : 1;

        transform.Translate(movement * Time.deltaTime);
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

    public void ControlInput(InputAction.CallbackContext context) 
    {
        controlInput = context.ReadValue<bool>();
    }

    public void SelectInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            PlacementManager.Instance.PlacePoint();
        }
    }

    public void HeightInput(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            int change = (int)context.ReadValue<float>();
            PlacementManager.Instance.ChangeHeight(change * heightChangeStep);
        }
    }
    #endregion
}

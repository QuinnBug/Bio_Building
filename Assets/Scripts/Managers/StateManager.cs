using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum State 
{
    NULL = -1,
    SELECT = 0,
    BUILD = 1,
    EVALUATE = 2,
    EDITING = 3
}

public class StateManager : Singleton<StateManager>
{
    public List<ObjectList> uiList = new List<ObjectList>();
    [Space]
    public State currentState = State.NULL;

    public State stateLocked = State.NULL;

    private void Start()
    {
        EventManager.Instance.stateChanged.AddListener(UpdateUI);
        ChangeState(State.SELECT);
        stateLocked = State.NULL;
    }

    public void Update()
    {
        //Build and select are only changed here Editing and evaluation are set elsewhere
        if(currentState == State.BUILD || currentState == State.SELECT) ChangeState(PlacementManager.Instance.selectedPrefab != null ? State.BUILD : State.SELECT);
    }

    public void UpdateUI(State newState)
    {
        //do this in 2 loops so that one object can be in multiple ui states, but gets set correctly without having to track each dupe
        foreach (ObjectList ol in uiList)
        {
            foreach (GameObject obj in ol.objects)
            {
                obj.SetActive(false);
            }
        }

        foreach (GameObject obj in uiList[(int)currentState].objects) 
        {
            obj.SetActive(true);
        }
    }

    public void ChangeState(State newState, bool lockState = false) 
    {
        //Don't change to the same state, && Don't change if the state lock is locked
        if (currentState == newState || stateLocked != State.NULL) return;

        if (lockState) stateLocked = newState;

        currentState = newState;

        PlacementManager.Instance.active = currentState == State.BUILD || currentState == State.EDITING;
        SelectionManager.Instance.active = currentState == State.SELECT || currentState == State.EDITING;

        EventManager.Instance.stateChanged.Invoke(newState);
    }

    /// <summary>
    /// Mostly used as a failsafe to avoid changing state during a delicate operation.
    /// </summary>
    /// <param name="keyState">Which state we're expecting to unlock from.</param>
    /// <returns>If the state ends up unlocked.</returns>
    internal bool UnlockState(State keyState)
    {
        if (keyState == stateLocked) stateLocked = State.NULL;

        return stateLocked == State.NULL;
    }
}

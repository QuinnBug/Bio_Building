using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum State 
{
    DEFAULT = 0,
    ROOM_BUILD = 1,
    FURNITURE_BUILD = 2
}

public class StateManager : Singleton<StateManager>
{
    public GameObject[] uiObjects = new GameObject[0];
    [Space]
    public State currentState = State.DEFAULT;

    internal StateEvent stateChanged = new StateEvent();

    private void Start()
    {
        ChangeState(State.DEFAULT);
    }

    public void ChangeState(State newState) 
    {
        if (currentState == newState) return;

        currentState = newState;

        WallPlacementManager.Instance.active = currentState == State.ROOM_BUILD;
        SelectionManager.Instance.active = currentState == State.DEFAULT;

        stateChanged.Invoke(newState);
    }
}

[System.Serializable]
public class StateEvent : UnityEvent<State> { }

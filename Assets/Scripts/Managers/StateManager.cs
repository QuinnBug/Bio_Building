using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum State 
{
    SELECT = 0,
    BUILD = 1,
    DECORATE = 2
}

public class StateManager : Singleton<StateManager>
{
    public List<ObjectList> uiList = new List<ObjectList>();
    [Space]
    public State currentState = State.SELECT;

    internal StateEvent stateChanged = new StateEvent();

    private void Start()
    {
        ChangeState(currentState);
        stateChanged.AddListener(UpdateUI);
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

    public void ChangeState(State newState) 
    {
        if (currentState == newState) return;

        currentState = newState;

        PlacementManager.Instance.active = currentState == State.BUILD;
        SelectionManager.Instance.active = currentState == State.SELECT;

        stateChanged.Invoke(newState);
    }
}

[System.Serializable]
public class StateEvent : UnityEvent<State> { }

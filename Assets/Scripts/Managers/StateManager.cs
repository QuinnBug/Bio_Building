using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum State 
{
    NULL = -1,
    SELECT = 0,
    BUILD = 1,
    EVALUATE = 2
}

public class StateManager : Singleton<StateManager>
{
    public List<ObjectList> uiList = new List<ObjectList>();
    [Space]
    public State currentState = State.NULL;

    public bool stateLocked = false;

    private void Start()
    {
        EventManager.Instance.stateChanged.AddListener(UpdateUI);
        ChangeState(State.SELECT);
        stateLocked = false;
    }

    public void Update()
    {
        if(currentState != State.EVALUATE) ChangeState(PlacementManager.Instance.selectedPrefab != null ? State.BUILD : State.SELECT);
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
        if (currentState == newState || stateLocked) return;

        currentState = newState;

        PlacementManager.Instance.active = currentState == State.BUILD;
        SelectionManager.Instance.active = currentState == State.SELECT;

        EventManager.Instance.stateChanged.Invoke(newState);
    }
}

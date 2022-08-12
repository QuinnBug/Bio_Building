using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum State 
{
    NULL = -1,
    SELECT = 0,
    BUILD = 1,
    DECORATE = 2
}

public class StateManager : Singleton<StateManager>
{
    public List<ObjectList> uiList = new List<ObjectList>();
    [Space]
    public State currentState = State.SELECT;

    private void Start()
    {
        ChangeState(currentState);
        EventManager.Instance.stateChanged.AddListener(UpdateUI);
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
        PaintingManager.Instance.active = currentState == State.DECORATE;
        SelectionManager.Instance.active = currentState == State.SELECT;

        EventManager.Instance.stateChanged.Invoke(newState);
    }
}

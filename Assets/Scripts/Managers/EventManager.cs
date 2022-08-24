using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    internal StateEvent stateChanged = new StateEvent();

    internal UnityEvent objectPlaced = new UnityEvent();
    internal UnityEvent objectDestroyed = new UnityEvent();
    internal UnityEvent objectPainted = new UnityEvent();
    internal UnityEvent modelChanged = new UnityEvent();    
    internal UnityEvent orthoToggle = new UnityEvent();    
}



[System.Serializable]
public class StateEvent : UnityEvent<State> { }

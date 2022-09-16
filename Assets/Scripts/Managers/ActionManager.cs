using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionManager : Singleton<ActionManager>
{
    Stack<Action> actionStack = new Stack<Action>();
    public ActionEvent actionEvent = new ActionEvent();

    private void Start()
    {
        actionEvent.AddListener(ReadAction);
    }

    private void ReadAction(ActionType type, GameObject target)
    {
        if (target.TryGetComponent(out WallMeshComponent wmc)) 
        {
            actionStack.Push(new WallAction(type, wmc));
        }
        //else if(target.TryGetComponent(out FurnitureComponent fc))
    }

    public bool UndoAction() 
    {
        if (actionStack.Count < 1) return false;

        Action action = actionStack.Pop();

        switch (action.type)
        {
            case ActionType.PLACE_WALL:
                {
                    WallAction wa = (WallAction)action;
                    if (wa.wmc != null)
                    {
                        Destroy(wa.wmc.gameObject);
                    }
                    else return false;
                }
                break;

            case ActionType.DESTROY_WALL:
                {
                    WallAction wa = (WallAction)action;
                    WallPlacementManager.Instance.CreateWall(wa.wallData.startPoint, wa.wallData.endPoint, wa.wallData.height, false);
                }
                break;

            default:
                Debug.LogWarning("INVALID ACTION TYPE - UNDO DID NOTHING");
                break;
        }

        return true;
    }
}

public class Action 
{
    public ActionType type;
   
    public Action(ActionType _type) 
    {
        type = _type;
    }
}

public class WallAction : Action
{
    public WallMeshComponent wmc;
    public WallMeshData wallData;

    public WallAction(ActionType _type, WallMeshComponent _wmc) : base(_type)
    {
        wmc = _wmc;
        wallData = wmc.data;
    }
}

public enum ActionType 
{
    PLACE_WALL = 0,
    DESTROY_WALL = 1,
    PLACE_FURN = 2,
    DESTROY_FURN = 3,
}

[System.Serializable]
public class ActionEvent : UnityEvent<ActionType, GameObject> { }

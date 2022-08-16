using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialStage 
{
    NULL = -1,
    START,
    BUILD_STATE,
    PLACE_WALLS,
    MESH_CHANGE,
    QUICK_DELETE,
    DECORATE_STATE,
    PAINT_WALL,
    SELECT_STATE,
    DELETE_GROUP,
    END,
    COMPLETE_FLAG
}

public class TutorialHandler : MonoBehaviour
{
    public List<ObjectList> tutorialObjects = new List<ObjectList>();

    public TutorialStage stage = TutorialStage.NULL;
    private TutorialStage nextStage = TutorialStage.NULL;
    private State targetState = State.NULL;

    private int steps = 0;
    private int stepsToAdvance = 0;

    public void Start()
    {
        if (!SaveManager.Instance.dataLoaded) SaveManager.Instance.Start();

        if (!SaveManager.Instance.currentUserData.tutorialCompleted) 
        {
            StartTutorial();
        }
        else
        {
            UpdateUI();
        }
    }

    private void StartTutorial()
    {
        ChangeStage(TutorialStage.START);
    }

    public void ProgressTutorial() 
    {
        switch (stage)
        {
            case TutorialStage.START:
                ChangeStage(TutorialStage.BUILD_STATE);
                break;

            case TutorialStage.END:
                ChangeStage(TutorialStage.COMPLETE_FLAG);
                break;

            default:
                return;
        }
    }

    public void ChangeStage(TutorialStage newStage) 
    {
        if (stage == TutorialStage.NULL && newStage != TutorialStage.START) return;

        if(stepsToAdvance > 0) 
        {
            steps++;
            if(steps < stepsToAdvance) return;
            //Debug.Log("Steps = " + steps + "/" + stepsToAdvance);
        }

        stepsToAdvance = steps = 0;
        stage = newStage;

        if(stage == TutorialStage.COMPLETE_FLAG) 
        {
            stage = TutorialStage.NULL;
            SaveManager.Instance.currentUserData.tutorialCompleted = true;
        }

        UpdateUI();
        UpdateVariables();
        nextStage = stage + 1;
    }

    public void ChangeStage() 
    {
        ChangeStage(nextStage);
    }

    public void ChangeStage(State state)
    {
        if (state == targetState)
        {
            ChangeStage(nextStage);
        }
    }

    public void UpdateVariables() 
    {
        //Add the necessary listen to advance, and remove the listener for the previous advancement
        switch (stage)
        {
            case TutorialStage.BUILD_STATE:
                //The listener for this stage to advance
                EventManager.Instance.stateChanged.AddListener(ChangeStage);
                targetState = State.BUILD;
                break;

            case TutorialStage.PLACE_WALLS:
                EventManager.Instance.stateChanged.RemoveListener(ChangeStage);
                EventManager.Instance.objectPlaced.AddListener(ChangeStage);
                stepsToAdvance = 3;
                targetState = State.NULL;
                break;

            case TutorialStage.MESH_CHANGE:
                EventManager.Instance.objectPlaced.RemoveListener(ChangeStage);
                EventManager.Instance.modelChanged.AddListener(ChangeStage);
                break;

            case TutorialStage.QUICK_DELETE:
                EventManager.Instance.modelChanged.RemoveListener(ChangeStage);
                EventManager.Instance.objectDestroyed.AddListener(ChangeStage);
                break;

            case TutorialStage.DECORATE_STATE:
                EventManager.Instance.objectDestroyed.RemoveListener(ChangeStage);
                EventManager.Instance.stateChanged.AddListener(ChangeStage);
                targetState = State.DECORATE;
                break;

            case TutorialStage.PAINT_WALL:
                targetState = State.NULL;
                EventManager.Instance.stateChanged.RemoveListener(ChangeStage);
                EventManager.Instance.objectPainted.AddListener(ChangeStage);
                break;

            case TutorialStage.SELECT_STATE:
                EventManager.Instance.objectPainted.RemoveListener(ChangeStage);
                EventManager.Instance.stateChanged.AddListener(ChangeStage);
                targetState = State.SELECT;
                break;

            case TutorialStage.DELETE_GROUP:
                targetState = State.NULL;
                EventManager.Instance.stateChanged.RemoveListener(ChangeStage);
                EventManager.Instance.objectDestroyed.AddListener(ChangeStage);
                break;

            case TutorialStage.END:
                EventManager.Instance.objectDestroyed.RemoveListener(ChangeStage);
                break;

            case TutorialStage.START:
            case TutorialStage.COMPLETE_FLAG:
            case TutorialStage.NULL:
            default:
                return;
        }
    }

    public void UpdateUI()
    {
        //do this in 2 loops so that one object can be in multiple ui states, but gets set correctly without having to track each dupe
        foreach (ObjectList ol in tutorialObjects)
        {
            foreach (GameObject obj in ol.objects)
            {
                obj.SetActive(false);
            }
        }

        if (stage == TutorialStage.NULL) return;

        foreach (GameObject obj in tutorialObjects[(int)stage].objects)
        {
            obj.SetActive(true);
        }
    }
}

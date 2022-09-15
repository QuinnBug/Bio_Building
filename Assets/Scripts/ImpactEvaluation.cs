using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEvaluation : MonoBehaviour
{
    public float camSwitchDelay = 2;
    public float animStartDelay = 2;
    public EnvSummoning environment;
    public GameObject evaluationScreen;
    public AdjustableShape currentShape;
    [Space]
    public Metadata_Plus[] allData;
    bool watchForEnd = false;
    float[] levels;

    private void Start()
    {
        evaluationScreen.SetActive(false);
    }

    public void StartEvaluation() 
    {
        StartCoroutine(Evaluate());
    }

    public IEnumerator Evaluate()
    {
        allData = FindObjectsOfType<Metadata_Plus>();
        if (allData.Length == 0) yield return null;

        StateManager.Instance.ChangeState(State.EVALUATE, true);

        //Debug.Log("Started Evaluation");

        ClimateManager.Instance.ResetLevels();
        levels = new float[] { 0, 0, 0, 0, 0 };

        foreach (Metadata_Plus data in allData)
        {
            //Debug.Log(data.gameObject.name);
            if (data.parameters.TryGetValue("id", out string idString))
            {

                for (int i = 0; i < 5; i++)
                {
                    int value = 5;

                    #region Metadata calculation
                    if (!int.TryParse(idString[i].ToString(), out value)) Debug.Log("Failed to read id # ");

                    levels[i] += (value - 5) * 25;
                    #endregion
                }
            }
            else
            {
                Debug.Log("No id key");
            }
        }

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] /= allData.Length;
            levels[i] = Mathf.Clamp(levels[i], -50, 50);
            levels[i] += 50;
        }

        CamManager.Instance.StartEvaluationCam();

        yield return new WaitForSeconds(camSwitchDelay);

        //this means that the environment should always appear just before the animation starts
        environment.UpdateSpeed(animStartDelay * 0.5f);
        environment.active = true;

        yield return new WaitForSeconds(animStartDelay);

        ClimateManager.Instance.StartAnimation(levels);
        watchForEnd = true;

    }

    public void EndEvaluation(bool clearPreviousAttempt)
    {
        if (!StateManager.Instance.UnlockState(State.EVALUATE)) return;
        StateManager.Instance.ChangeState(State.SELECT);

        //Debug.Log("End Evaluation");

        ClimateManager.Instance.ResetLevels();

        if (clearPreviousAttempt && allData != null)
        {
            for (int i = 0; i < allData.Length; i++)
            {
                Destroy(allData[i].gameObject);
            }
        }

        environment.active = false;
        evaluationScreen.SetActive(false);

        watchForEnd = false;
    }

    private void Update()
    {
        if (watchForEnd)
        {
            if (!ClimateManager.Instance.animating) DisplayEvaluationDetails();
        }

        if (evaluationScreen.activeInHierarchy)
        {
            for (int i = 0; i < currentShape.values.Count; i++)
            {
                currentShape.values[i] = Mathf.Lerp(currentShape.values[i], levels[i]/10, 0.1f);
            }
        }
    }

    private void DisplayEvaluationDetails()
    {
        for (int i = 0; i < currentShape.values.Count; i++)
        {
            currentShape.values[i] = 0;
        }

        evaluationScreen.SetActive(true);
    }
}

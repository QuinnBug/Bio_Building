using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;

public class ImpactEvaluation : MonoBehaviour
{
    public Metadata[] allData;
    bool watchForEnd = false;

    public void Evaluate()
    {
        Debug.Log("Started Evaluation");
        allData = FindObjectsOfType<Metadata>();

        ClimateManager.Instance.ResetLevels();
        float[] levels = new float[] { 0, 0, 0, 0, 0 };

        foreach (Metadata data in allData)
        {
            //Debug.Log(data.gameObject.name);
            if (data.parameters.TryGetValue("Id", out Metadata.Parameter idParameter))
            {
                string idString = idParameter.value;

                for (int i = 0; i < 5; i++)
                {
                    int value = 0;

                    if (int.TryParse(idString[i].ToString(), out value)) Debug.Log((value -5) * 2);
                    else Debug.Log("Failed to read id # ");

                    levels[i] += (value - 5) * 5;
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

        ClimateManager.Instance.StartAnimation(levels);

        StateManager.Instance.ChangeState(State.EVALUATE);
        StateManager.Instance.stateLocked = true;
        watchForEnd = true;

    }

    void EndEvaluation(bool clearPreviousAttempt)
    {
        Debug.Log("End Evaluation");

        ClimateManager.Instance.ResetLevels();

        if (clearPreviousAttempt && allData != null)
        {
            for (int i = 0; i < allData.Length; i++)
            {
                Destroy(allData[i].gameObject);
            }
        }

        watchForEnd = false;
        StateManager.Instance.stateLocked = false;
        StateManager.Instance.ChangeState(State.SELECT);
    }

    private void Update()
    {
        if (watchForEnd)
        {
            if (!ClimateManager.Instance.animating) EndEvaluation(true);
        }
    }
}

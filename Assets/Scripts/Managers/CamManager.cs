using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamManager : Singleton<CamManager>
{
    public CinemachineVirtualCamera playCam;
    public CinemachineVirtualCamera cinematicCam;
    [Space]
    public Transform cinematicFocus;
    public CinemachineSmoothPath focusPath;

    int currentFocusPoint;

    internal bool evaluating = false;
    CinemachineTrackedDolly cinematicDolly;

    public void Start()
    {
        cinematicDolly = cinematicCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        cinematicDolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
    }

    private void Update()
    {
        if (evaluating)
        {
            cinematicFocus.position = focusPath.EvaluatePositionAtUnit(
                Mathf.MoveTowards(0, 1, ClimateManager.Instance.animTimer / ClimateManager.Instance.animDuration),
                CinemachinePathBase.PositionUnits.Normalized);

            cinematicDolly.m_PathPosition = Mathf.MoveTowards(0, 1, ClimateManager.Instance.animTimer / ClimateManager.Instance.animDuration);
        }
    }

    public void StartEvaluationCam() 
    {
        cinematicFocus.position = focusPath.EvaluatePositionAtUnit(0, CinemachinePathBase.PositionUnits.Normalized);
        cinematicDolly.m_PathPosition = 0;
        cinematicCam.transform.position = cinematicDolly.m_Path.EvaluatePositionAtUnit(0, CinemachinePathBase.PositionUnits.Normalized);

        cinematicCam.Priority = 1;
        playCam.Priority = 0;

        evaluating = true;
    }

    public void EndEvaluationCam() 
    {
        //cinematicFocus.position = focusPath.EvaluatePositionAtUnit(0, CinemachinePathBase.PositionUnits.Normalized);
        //cinematicDolly.m_PathPosition = 0;

        cinematicCam.Priority = 0;
        playCam.Priority = 1;

        evaluating = false;
    }
}

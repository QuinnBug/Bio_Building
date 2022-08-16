using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMSettingsUI : MonoBehaviour
{
    public Toggle[] toggles = new Toggle[3];

    private void Start() 
    {
        EventManager.Instance.stateChanged.AddListener(UpdateToggleFromState);
    }

    private void UpdateToggleFromState(State newState)
    {
        int stateNum = (int)newState;

        toggles[stateNum].isOn = true;
    }

    public void UpdateStateFromToggle() 
    {
        int activeToggle = -1;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                activeToggle = i;
                break;
            }
        }

        StateManager.Instance.ChangeState((State)activeToggle);
    }
}

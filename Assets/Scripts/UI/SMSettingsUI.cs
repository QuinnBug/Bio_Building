using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMSettingsUI : MonoBehaviour
{
    public Toggle[] toggles = new Toggle[3];

    private void Update()
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

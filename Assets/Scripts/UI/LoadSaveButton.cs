using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadSaveButton : MonoBehaviour
{
    public LoadSaveButtons parent;
    public TMP_Text nameplate;

    public void SelectRoom() 
    {
        parent.selectionName = nameplate.text;
    }
}

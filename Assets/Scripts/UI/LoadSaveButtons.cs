using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadSaveButtons : MonoBehaviour
{
    public TMP_InputField input;

    public void Save() 
    {
        if (input.text.Length <= 0) return;

        SaveManager.Instance.SaveIntoJson(input.text);
    }

    public void Load()
    {
        if (input.text.Length <= 0) return;

        SaveManager.Instance.LoadFromJson(input.text);
    }
}

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

        SaveManager.Instance.SaveRoomData(input.text);
        input.text = "";
    }

    public void Load()
    {
        if (input.text.Length <= 0) return;

        SaveManager.Instance.CreateFromSave(input.text);
    }
}

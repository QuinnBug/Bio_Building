using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadSaveButtons : MonoBehaviour
{
    public TMP_Text selectionDisplay;
    public string selectionName = "";
    //public GameObject loadCover;
    //public GameObject saveCover;
    [Space]
    public TMP_InputField[] inputs;
    public Transform buttonHolder;
    public GameObject buttonPrefab;
    public List<GameObject> buttons;
    bool fileNamesLoaded;

    public void Update()
    {
        if (!fileNamesLoaded && SaveManager.Instance.allRoomData.Count != 0) PopulateDropdown();

        selectionDisplay.text = selectionName == "" ? "-" : "Load " + selectionName;
        //loadCover.SetActive(selectionName == "");
    }

    public void PopulateDropdown()
    {
        foreach (RoomData data in SaveManager.Instance.allRoomData)
        {
            bool loaded = false;
            foreach (GameObject btn in buttons)
            {
                if (btn.name == data.name) { loaded = true; break; }
            }
            if (loaded) continue;

            GameObject newBtnGO = Instantiate(buttonPrefab, buttonHolder);
            LoadSaveButton LSB = newBtnGO.GetComponent<LoadSaveButton>();
            LSB.parent = this;
            LSB.nameplate.text = data.name;

            newBtnGO.name = data.name;
            buttons.Add(newBtnGO);
        }

        fileNamesLoaded = true;
    }

    public void InputSave(int inputNum) 
    {
        Save(inputs[inputNum].text);
        inputs[inputNum].text = "";
    }

    public void Save(string saveName)
    {
        if (saveName.Length <= 0) return;

        foreach (GameObject btn in buttons)
        {
            if (btn.name == saveName) return;
        }

        SaveManager.Instance.SaveRoomData(saveName);

        PopulateDropdown();
    }

    public void Load()
    {
        if (selectionName == "") return;
        SaveManager.Instance.CreateFromSave(selectionName);
        selectionName = "";

        //MessageManager.Instance.AddMessage("Loaded " + dropdown.options[dropdown.value].text);
    }
}

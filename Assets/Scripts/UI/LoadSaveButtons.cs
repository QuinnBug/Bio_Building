using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadSaveButtons : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Dropdown dropdown;
    bool fileNamesLoaded;

    public void Update()
    {
        if (!fileNamesLoaded && SaveManager.Instance.allRoomData.Count != 0) PopulateDropdown();

        if (fileNamesLoaded) { Debug.Log(dropdown.value - 1); }

        input.gameObject.SetActive(dropdown.value == 0);
    }

    public void PopulateDropdown()
    {
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData> ();


        newOptions.Add(new TMP_Dropdown.OptionData("New Room"));
        foreach (RoomData data in SaveManager.Instance.allRoomData)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(data.name));
        }

        dropdown.AddOptions(newOptions);
        fileNamesLoaded = true;
    }

    public void Save() 
    {
        if(dropdown.value != 0) 
        {
            SaveManager.Instance.SaveRoomData(dropdown.options[dropdown.value].text);
            MessageManager.Instance.AddMessage("Overwrite not set up for" + dropdown.options[dropdown.value].text);
        }
        else 
        {
            if (input.text.Length <= 0)
            {
                MessageManager.Instance.AddMessage("Enter a name for the room");
                return;
            }

            foreach (TMP_Dropdown.OptionData item in dropdown.options)
            {
                if (item.text == input.text)
                {
                    MessageManager.Instance.AddMessage("There is already a room with that name");
                    return;
                }
            }

            SaveManager.Instance.SaveRoomData(input.text);
            MessageManager.Instance.AddMessage("Saved " + input.text);
            input.text = "";
        }

        PopulateDropdown();
    }

    public void Load()
    {
        //the first entry is "New Room" so shouldn't be used for loading
        if (dropdown.value == 0) return;

        SaveManager.Instance.CreateFromSave(dropdown.options[dropdown.value].text);
        MessageManager.Instance.AddMessage("Loaded " + dropdown.options[dropdown.value].text);
    }
}

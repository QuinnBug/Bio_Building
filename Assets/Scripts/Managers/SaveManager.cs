using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;

public class SaveManager : Singleton<SaveManager>
{
    public string filePath = "/SavedRooms/";
    public SaveFileData currentUserData = null;
    public List<RoomData> allRoomData = new List<RoomData>();
    private bool firebaseSaveExists = false;

    public bool dataLoaded;

    public void Start()
    {
        if (dataLoaded) return;

        if (!Application.isEditor)
            LoadJSONFromFirebase();
        else
            LoadJson();

    }

    public void SaveRoomData(string roomName) 
    {
        RoomData roomData = new RoomData();

        roomData.name = roomName;

        foreach (Selectable select in FindObjectsOfType<Selectable>())
        {
            roomData.selectables.Add(select.data);
        }

        allRoomData.Add(roomData);
        UpdateJson();
    }

    public void UpdateJson()
    {
        SaveFileData sfd = new SaveFileData();
        sfd.rooms = allRoomData;

        string saveStr = JsonUtility.ToJson(sfd);
        if (Application.isEditor)
        {
            if (System.IO.File.Exists(Application.dataPath + filePath + ".json"))
            {
                //overwriting file
            }
            else
            {
                //create and close the file, ready to be written to.
                System.IO.File.Create(Application.dataPath + filePath + ".json").Close();
            }

            System.IO.File.WriteAllText(Application.dataPath + filePath + ".json", saveStr);
        }
        else
        {
            if (!firebaseSaveExists)
            { 
                FirebaseDatabase.PostJSON(FirebaseController.Instance.userData.uid, saveStr,
                    gameObject.name, callback: "OnWriteToJSONSuccess", fallback: "OnWriteToJSONFailed");
            }
            else
            {
                FirebaseDatabase.UpdateJSON(FirebaseController.Instance.userData.uid, saveStr,
                    gameObject.name, callback: "OnWriteToJSONSuccess", fallback: "OnWriteToJSONFailed");
            }
        }
    }

    private void OnWriteToJSONSuccess(string _data)
    {
        firebaseSaveExists = true;
        FirebaseController.Instance.UpdateText(_data);
        //JSON Write Success
    }

    private void OnWriteToJSONFailed(string _error)
    {
        firebaseSaveExists = false;
        FirebaseController.Instance.UpdateText(_error, Color.red);
        //JSON Write Failed
    }

    public void CreateFromSave(string roomName)
    {
        foreach (Selectable select in FindObjectsOfType<Selectable>())
        {
            Destroy(select.gameObject);
        }

        RoomData data = null;
        foreach (RoomData roomData in allRoomData)
        {
            if (roomData.name == roomName) { data = roomData; break; }
        }

        if (data == null) return;

        foreach (SelectableData itemData in data.selectables)
        {
            PlacementManager.Instance.RecreateObject(itemData);
        }
    }

    public void LoadJson() 
    {
        if (!System.IO.File.Exists(Application.dataPath + filePath + ".json")) return;

        string json = System.IO.File.ReadAllText(Application.dataPath + filePath + ".json");

        SaveFileData sfd = JsonUtility.FromJson<SaveFileData>(json);

        if (sfd == null) { Debug.Log("FAILED TO LOAD " + filePath); return; }

        allRoomData = sfd.rooms;
        dataLoaded = true;
    }

    public void LoadJSONFromFirebase()
    {
        FirebaseDatabase.GetJSON(FirebaseController.Instance.userData.uid, 
            gameObject.name, callback: "OnGetJSONSuccess", fallback: "OnGetJSONFailed");
    }

    private void OnGetJSONSuccess(string _data)
    {
        firebaseSaveExists = true;
        currentUserData = JsonUtility.FromJson<SaveFileData>(_data);

        allRoomData = currentUserData.rooms;
        FirebaseController.Instance.UpdateText(_data);

        dataLoaded = true;
    }

    private void OnGetJSONFailed(string _error)
    {
        firebaseSaveExists = false;
        FirebaseController.Instance.UpdateText(_error, Color.red);
        //No Data found
    }
}

[Serializable]
public class RoomData
{
    public string name;
    public List<SelectableData> selectables = new List<SelectableData>();
}

[Serializable]
public class SaveFileData 
{
    public bool tutorialCompleted = false;
    public List<RoomData> rooms = new List<RoomData>();
}

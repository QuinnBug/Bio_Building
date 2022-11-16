using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using UnityEngine.Events;
using System.Threading.Tasks;
using Firebase.Database;

public class SaveManager : Singleton<SaveManager>
{
    public string filePath = "/SavedRooms/";
    public SaveFileData currentUserData = null;
    public List<RoomData> allRoomData = new List<RoomData>();
    private bool firebaseSaveExists = false;

    public bool dataLoaded;
    UnityEvent m_JSONReadSuccessEvent;
    private Firebase.Database.FirebaseDatabase database;

    public void Start()
    {
        if (m_JSONReadSuccessEvent == null)
            m_JSONReadSuccessEvent = new UnityEvent();

        m_JSONReadSuccessEvent.AddListener(OnMobileJSONReadSuccess);
        if (dataLoaded || Application.platform == RuntimePlatform.Android) return;

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
                FirebaseWebGL.Scripts.FirebaseBridge.FirebaseDatabase.PostJSON(FirebaseController.Instance.userData.uid + "/BuildingSaves", saveStr,
                    gameObject.name, callback: "OnWriteToJSONSuccess", fallback: "OnWriteToJSONFailed");
            }
            else
            {
                FirebaseWebGL.Scripts.FirebaseBridge.FirebaseDatabase.UpdateJSON(FirebaseController.Instance.userData.uid + "/BuildingSaves", saveStr,
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
        Debug.Log("7");
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("8");
            //database = Firebase.Database.FirebaseDatabase.DefaultInstance;
            Debug.Log("8.01");
            GetJSONOnMobile();
            Debug.Log("9");
        }

        else
        {
            FirebaseWebGL.Scripts.FirebaseBridge.FirebaseDatabase.GetJSON(FirebaseController.Instance.userData.uid + "/BuildingSaves", 
                gameObject.name, callback: "OnGetJSONSuccess", fallback: "OnGetJSONFailed");
        }
    }
    private void GetJSONOnMobile()
    {
        Debug.Log("Uid is " + FirebaseController.Instance.userData.uid);
        Firebase.Database.FirebaseDatabase.GetInstance("https://bio-construction-default-rtdb.europe-west1.firebasedatabase.app/").GetReference(Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId + "/BuildingSaves").GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("8.2");
            DataSnapshot snapshot = task.Result;
            Debug.Log("8.3");
            string _data = snapshot.GetRawJsonValue();
            Debug.Log("8.4");
            firebaseSaveExists = true;
            currentUserData = JsonUtility.FromJson<SaveFileData>(_data);
            Debug.Log("8.5");
            allRoomData = currentUserData.rooms;
            FirebaseController.Instance.UpdateText(_data);
            Debug.Log("8.6");
            dataLoaded = true;
            Debug.Log("8.7");
            Debug.Log(_data);

            //OnGetJSONSuccess(snapshot.GetRawJsonValue());
        });
    }
    //private IEnumerator GetJSONOnMobile()
    //{
    //    var loadDataTask = LoadData();
    //    yield return new WaitUntil(predicate: () => loadDataTask.IsCompleted);
    //    if(loadDataTask.Result)
    //    {        
    //        //firebaseSaveExists = true;
    //        //currentUserData = JsonUtility.FromJson<SaveFileData>(dataSnapshot.GetRawJsonValue());

    //        //allRoomData = currentUserData.rooms;
    //        //FirebaseController.Instance.UpdateText(dataSnapshot.GetRawJsonValue());

    //        //dataLoaded = true;
    //        OnGetJSONSuccess(loadDataTask.Result);
    //    }
    //    else
    //    {
    //        OnGetJSONFailed("No data found");
    //    }
    //}

    //public async Task<SaveFileData> LoadData()
    //{
    //    var dataSnapshot = await database.GetReference(
    //        FirebaseController.Instance.userData.uid).GetValueAsync();
    //    if (!dataSnapshot.Exists)
    //    {
    //        return null;
    //    }

    //    return JsonUtility.FromJson < SaveFileData > (dataSnapshot.GetRawJsonValue());
    //}

    private void OnGetJSONSuccess(string _data)
    {
        firebaseSaveExists = true;
        currentUserData = JsonUtility.FromJson<SaveFileData>(_data);

        allRoomData = currentUserData.rooms;
        FirebaseController.Instance.UpdateText(_data);

        dataLoaded = true;

        Debug.Log(_data);
    }
    private void OnGetJSONSuccess(SaveFileData _data)
    {
        firebaseSaveExists = true;
        currentUserData = _data;

        allRoomData = currentUserData.rooms;
        FirebaseController.Instance.UpdateText(_data.ToString());

        dataLoaded = true;

        Debug.Log(_data);
    }


    private void OnGetJSONFailed(string _error)
    {
        firebaseSaveExists = false;
        FirebaseController.Instance.UpdateText(_error, Color.red);
        //No Data found
    }

    private void OnMobileJSONReadSuccess()
    {

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

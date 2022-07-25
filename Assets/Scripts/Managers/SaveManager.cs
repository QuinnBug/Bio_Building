using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public string folderPath = "/SavedRooms/";

    public void SaveIntoJson(string roomName)
    {
        RoomData roomData = new RoomData();

        foreach (WallMeshComponent wmc in FindObjectsOfType<WallMeshComponent>())
        {
            roomData.walls.Add(wmc.data);
        }

        foreach (FurnitureComponent fc in FindObjectsOfType<FurnitureComponent>())
        {
            roomData.furnitures.Add(fc.data);
        }

        string roomDataStr = JsonUtility.ToJson(roomData);

        if(System.IO.File.Exists(Application.dataPath + folderPath + roomName + ".json")) 
        {
            //overwriting file
        }
        else 
        {
            System.IO.File.Create(Application.dataPath + folderPath + roomName + ".json").Close();
        }

        System.IO.File.WriteAllText(Application.dataPath + folderPath + roomName + ".json", roomDataStr);
    }

    public void LoadFromJson(string roomName) 
    {
        if (!System.IO.File.Exists(Application.dataPath + folderPath + roomName + ".json")) return;

        string json = System.IO.File.ReadAllText(Application.dataPath + folderPath + roomName + ".json");

        RoomData room = JsonUtility.FromJson<RoomData>(json);

        if (room == null) { Debug.Log("FAILED TO LOAD " + roomName); return; }

        //create walls from each of the walls in roomdata
        foreach (WallMeshData data in room.walls)
        {
            if (!WallPlacementManager.Instance.RecreateWall(data)) Debug.Log("FAILED TO CREATE WALL " + data.id);
        }

        //create furniture... you get it
        foreach (FurnitureData data in room.furnitures)
        {
            Debug.Log("SET UP FURNITURE LOADING");
        }
    }
}

public class RoomData
{
    public List<WallMeshData> walls = new List<WallMeshData>();
    public List<FurnitureData> furnitures = new List<FurnitureData>();
}

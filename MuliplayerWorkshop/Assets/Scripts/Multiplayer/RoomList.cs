using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject prefabBtRooms;
    public Transform contentParent;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private List<GameObject> allRoomsButtons = new List<GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"[RoomList] Ohoton update received. Rooms here: {roomList.Count}");
        //Update cache
        UpdateCachedRoomList(roomList);
        //Clean buttons
        ClearRoomButtons();
        //Regenerate UI
        GenerateRoomButtons();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                    Debug.Log($"[RoomList] Room erased from cache: {info.Name}");
                }
            }
            else
            {
                //If the room exist, we updated, if it doesn't, we add it
                cachedRoomList[info.Name] = info;
                Debug.Log($"[RoomList] Room added/updated in cache: {info.Name} (Players: {info.PlayerCount}/{info.MaxPlayers})");
            }
        }
        Debug.Log($"[RoomList] Valid rooms in cache {cachedRoomList.Count}");
    }

    private void ClearRoomButtons()
    {
        int count = allRoomsButtons.Count;
        foreach (GameObject button in allRoomsButtons)
        {
            if (button != null) Destroy(button);
        }
        allRoomsButtons.Clear();
        if (count > 0) Debug.Log($"[RoomList] Cleaning: {count} destroyed buttons.");
    }

    private void GenerateRoomButtons()
    {
        int createdCount = 0;
        foreach (var roomEntry in cachedRoomList)
        {
            RoomInfo roomInfo = roomEntry.Value;

            // Filtro de visibilidad
            if (roomInfo.IsOpen && roomInfo.IsVisible)
            {
                GameObject roomButton = Instantiate(prefabBtRooms, contentParent);

                // Intentamos obtener el script Room para asignar el nombre
                Room roomScript = roomButton.GetComponent<Room>();
                if (roomScript != null)
                {
                    roomScript.roomName.text = roomInfo.Name;
                    allRoomsButtons.Add(roomButton);
                    createdCount++;
                }
                else
                {
                    Debug.LogError("[RoomList] ¡Error! button prefab doesn't have component 'Room.cs'.");
                }
            }
        }
        Debug.Log($"[RoomList] UI Updated: {createdCount} buttons created.");
    }
}
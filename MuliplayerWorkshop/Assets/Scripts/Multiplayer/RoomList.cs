using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject prefabBtRooms;
    //public GameObject[] Allrooms;
    public Transform contentParent;
    private List<GameObject> allRoomsButtons = new List<GameObject>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject button in allRoomsButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        allRoomsButtons.Clear();
        foreach (RoomInfo roomInfo in roomList)
        {
            if (!roomInfo.RemovedFromList && roomInfo.IsOpen && roomInfo.IsVisible)
            {
                GameObject roomButton = Instantiate(prefabBtRooms, contentParent);
                roomButton.GetComponent<Room>().roomName.text = roomInfo.Name;
                allRoomsButtons.Add(roomButton);
            }
        }
    }
}

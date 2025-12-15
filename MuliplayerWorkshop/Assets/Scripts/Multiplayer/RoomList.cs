using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject prefabBtRooms;
    public GameObject[] Allrooms;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < Allrooms.Length; i++)
        {
            if (Allrooms[i] != null)
            {
                Destroy(Allrooms[i]);
            }
        }
        Allrooms = new GameObject[roomList.Count];
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].IsOpen && roomList[i].IsVisible && roomList[i].PlayerCount >= 1)
            {
                GameObject room = Instantiate(prefabBtRooms, Vector3.zero, Quaternion.identity, GameObject.Find("Content").transform);
                room.GetComponent<Room>().roomName.text = roomList[i].Name;
                Allrooms[i] = room;
            }
        }
    }
}

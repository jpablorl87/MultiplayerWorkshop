using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    public TMP_Text roomName;
    public void JoinRoom()
    {
        GameObject.Find("CreateAndJoin").GetComponent<CreateAndJoin>().JoinRoomInList(roomName.text);
    }
}

using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputCreate;
    public TMP_InputField inputJoin;
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(inputCreate.text);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputJoin.text);
    }
    public void JoinRoomInList(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(3);//Should be escene 3 = PhotonNetwork.LoadLevel(3)
    }
    //Network logic
    public override void OnLeftRoom()
    {
        //Go back to the lobby
        SceneManager.LoadScene(2);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[CreateandJoin] Player {otherPlayer.NickName} has left the room");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log($"[CreateandJoin] There is only one player in the room, game can be stopped");
        }
    }
}

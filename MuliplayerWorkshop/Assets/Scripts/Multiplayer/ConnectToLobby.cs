using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ConnectToLobby : MonoBehaviourPunCallbacks
{
    public void ConnectLobby()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(2);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"[ConnectToLobby] Disconected: {cause} Trying to reconnect");
        //If there is a big problem we go back to scene 0 (loading...)
        SceneManager.LoadScene(0);
    }
}

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string playerPrefabName = "Player";
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Color localPlayerColor = Color.green;
    [SerializeField] private Color remotePlayerColor = Color.magenta;
    [SerializeField] private GameObject playerPrefab;
    private bool allPlayersReady = false;
    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("[PlayerSpawnManager] Offline mode detected");
            SpawnPlayerOffline();
        }
    }
    public override void OnJoinedRoom()
    {
        SpawnLocalPlayer();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount ==2 && PhotonNetwork.IsMasterClient)
        {
            //Game start here
        }
    }
    private void SpawnLocalPlayer()
    {
        //ID and position
        int playerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int spawnIndex = playerActorNumber - 1;
        Vector3 spawnPos = spawnPoints[0].position;
        //Instantiation in network
        GameObject playerGo = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
        PlayerController pc = playerGo.GetComponent<PlayerController>();
        if (pc!= null)
        {
            //ID
            pc.playerID = playerActorNumber;
            //Color
            PlayerView pv = playerGo.GetComponent<PlayerView>();
            if (pv != null)
            {
                pv.SetPlayerColor(PhotonNetwork.IsMasterClient ? localPlayerColor : remotePlayerColor);
            }
        }
        allPlayersReady = true;
    }
    private void SpawnPlayerOffline()
    {
        GameObject playerGo = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
        PlayerController pc = playerGo.GetComponent<PlayerController>();
        PlayerInputController pic = playerGo.GetComponent<PlayerInputController>();
        if (pc != null)
        {
            //ID
            pc.playerID = 1;
            if (pic != null)
            {
                pic.playerID = 1;
            }
            //Color
            PlayerView pv = playerGo.GetComponent<PlayerView>();
            if (pv != null)
            {
                pv.SetPlayerColor(localPlayerColor);
            }
        }
        allPlayersReady = true;
    }
}

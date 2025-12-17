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
        if (PhotonNetwork.InRoom)
        {
            SpawnLocalPlayer();
            CheckPlayersAndStart();
        }
    }
    public override void OnJoinedRoom()
    {
        if (GameObject.FindWithTag("Player") == null)
        {
            SpawnLocalPlayer();
        }
        CheckPlayersAndStart();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PlayerSpawnManager] Player {newPlayer.ActorNumber} joined");
        CheckPlayersAndStart();
    }
    private void CheckPlayersAndStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log($"[PlayerSpawnManager] Checking players: {currentPlayers}/2");

            if (currentPlayers >= 2 && allPlayersReady)
            {
                if (photonView != null)
                {
                    Debug.Log("[PlayerSpawnManager] Condition met. Sending RPC_StartGameSync");
                    photonView.RPC("RPC_StartGameSync", RpcTarget.All);
                }
                else
                {
                    Debug.LogError("[PlayerSpawnManager] ERROR: No Photonview in this object");
                }
            }
        }
    }
    [PunRPC]
    public void RPC_StartGameSync()
    {
        EventManager.TriggerGameStart(PhotonNetwork.MasterClient.ActorNumber);
        Debug.Log("[PlayerSpawnManager] Event start Triggered");
    }
    private void SpawnLocalPlayer()
    {
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        Vector3 spawnPos = spawnPoints[0].position;
        if (myActorNumber > 1 && spawnPoints.Length > 1) spawnPos = spawnPoints[1].position;

        GameObject playerGo = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);

        if (playerGo == null)
        {
            Debug.LogError($"[PlayerSpawnManager] Could not instantiate {playerPrefabName}. Watch resources folder.");
            return;
        }

        PlayerController pc = playerGo.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.playerID = myActorNumber;

            PlayerView pv = playerGo.GetComponent<PlayerView>();
            if (pv != null)
            {
                Color colorAAsignar = (myActorNumber == 1) ? localPlayerColor : remotePlayerColor;
                pv.SetPlayerColor(colorAAsignar);
            }
        }

        allPlayersReady = true;
        Debug.Log($"[PlayerSpawnManager] Player {myActorNumber} correctly instantiate.");
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

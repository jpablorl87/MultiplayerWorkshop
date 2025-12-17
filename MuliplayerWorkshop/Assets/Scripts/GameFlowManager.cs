using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameFlowManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI endGameText;
    private int playersAlive = 0;
    private bool isExiting = false;
    public override void OnEnable()
    {
        base.OnEnable();
        EventManager.OnPlayerDied += HandlePlayerDeath;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        EventManager.OnPlayerDied -= HandlePlayerDeath;
    }
    /*private void SimulateGameStart()
    {
        EventManager.TriggerGameStart(1);
    }*/
    private void Start()
    {
        if (endGameText != null) endGameText.gameObject.SetActive(false);
        playersAlive = PhotonNetwork.IsConnected ? PhotonNetwork.CurrentRoom.PlayerCount : 1;
        Debug.Log($"[GameFlowManager] Game started with {playersAlive} players.");
    }
    private void HandlePlayerDeath(int playerId)
    {
        if (isExiting) return;

        playersAlive--;

        //TExt win/lose
        if (endGameText != null)
        {
            endGameText.gameObject.SetActive(true);

            if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
            {
                endGameText.text = "YOU LOSE";
                endGameText.color = Color.magenta;
            }
            else
            {
                endGameText.text = "YOU WIN!";
                endGameText.color = Color.green;
            }
        }

        if (playersAlive <= 1)
        {
            isExiting = true;
            StartCoroutine(LeaveRoutine());
        }
    }

    private IEnumerator LeaveRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("[GameFlowManager] Loading escene 0...");
 
        SceneManager.LoadScene(0);
    }
}

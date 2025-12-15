using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    private int playersAlive = 0;
    private const int TOTAL_PLAYERS = 2;
    private void OnEnable()
    {
        EventManager.OnPlayerDied += HandlePlayerDeath;
    }
    private void OnDisable()
    {
        EventManager.OnPlayerDied -= HandlePlayerDeath;
    }
    private void Start()
    {
        playersAlive = TOTAL_PLAYERS;
        Invoke(nameof(SimulateGameStart), 2f);
    }
    private void SimulateGameStart()
    {
        EventManager.TriggerGameStart(1);
    }
    private void HandlePlayerDeath(int playerId)
    {
        playersAlive--;
        Debug.Log($"[GameFlowManager] Player {playerId} id dead");
        if (playersAlive <= 1)
        {
            int winnerId = (playersAlive == 1) ? FindWinnerID() : 0;
            EventManager.TriggerGameOver(winnerId);
        }
    }
    private int FindWinnerID()
    {
        return 1;
    }
}

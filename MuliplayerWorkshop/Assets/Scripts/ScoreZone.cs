using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    private bool scored = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (scored || !other.gameObject.CompareTag("Player")) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null )
        {
            EventManager.TriggerScoreIncreased(player.playerID);
            scored = true;
            gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        scored = false;
    }
}

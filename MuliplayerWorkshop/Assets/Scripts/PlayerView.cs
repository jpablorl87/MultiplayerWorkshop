using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private PlayerController controller;
    //How tall is the player
    [SerializeField] private Vector2 standingSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 slidingSize = new Vector2(1.5f, 0.5f);
    [SerializeField] private Vector2 slidingOffset = new Vector2(0, -0.75f);
    [SerializeField] private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        if (playerCollider == null) playerCollider = GetComponent<BoxCollider2D>();
        SetStandingCollider();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetState(bool isSliding, bool isJumping)
    {
        if (isSliding)
        {
            playerCollider.size = slidingSize;
            playerCollider.offset = slidingOffset;
        }
        else if (isJumping)
        {
            SetStandingCollider();
        }
        else
        {
            SetStandingCollider();
        }
    }
    private void SetStandingCollider()
    {
        playerCollider.size = standingSize;
        playerCollider.offset = Vector2.zero;
    }
    public void SetPlayerColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}

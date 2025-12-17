using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float verticalMoveSpeed = 8f;
    //ID of the player
    [HideInInspector] public int playerID = -1;
    //View
    [SerializeField] private PlayerView view;
    private PhotonView pv;
    //State of the player
    private bool isSliding = false;
    private bool isAlive = true;
    private bool isJumping = false;
    private static bool isGameOverTriggered = false;

    private Vector2 networkPosition;
    private float networkJumpTime;
    private Collider2D playerCollider;
    private void Awake()
    {
        isGameOverTriggered = false;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        //playerID = 1;//That's for now, we'll assign a real id with photon
        if (pv == null) pv = GetComponent<PhotonView>();
        if (playerCollider == null) playerCollider = GetComponent<Collider2D>();
        if (!PhotonNetwork.IsConnectedAndReady) playerID = 1;
    }
    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady && pv != null && pv.Owner != null)
        {
            playerID = pv.Owner.ActorNumber;
        }
        PlayerInputController pic = GetComponent<PlayerInputController>();
        if (pic != null)
        {
            pic.playerID = playerID;
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        EventManager.OnPlayerJump += HandleJump;/*
        EventManager.OnPlayerSlide += HandleSlide;*/
        EventManager.OnPlayerDied += HandlePlayerDeath;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        EventManager.OnPlayerJump -= HandleJump;/*
        EventManager.OnPlayerSlide -= HandleSlide;*/
        EventManager.OnPlayerDied -= HandlePlayerDeath;
    }
    private void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady && pv != null)
        {
            if (!pv.IsMine)
            {
                rb.position = Vector2.MoveTowards(rb.position, networkPosition, 10f * Time.deltaTime);
            }
        }
    }
    private void HandleJump(int id)
    {
        //bool isControllable = true;
        bool isControllable = PhotonNetwork.IsConnectedAndReady ? pv.IsMine : true;
        /*if (PhotonNetwork.IsConnectedAndReady)
        {
            isControllable = pv.IsMine;
        }*/
        if (!isControllable) return;
        if (id != playerID || !isAlive || isJumping) return;
        ExecuteJumpLogic();
        //We call the rcp in all clients
        if (PhotonNetwork.IsConnectedAndReady)
        {
            pv.RPC("RPCSyncJump", RpcTarget.Others, playerID);
            Debug.Log($"[PlayerController] RPC sended");
        }
    }
    private void ExecuteJumpLogic()
    {
        isJumping = true;
        //Notify to the view
        view.SetState(isSliding, isJumping);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Debug.Log($"[PlayerController] Player {playerID} has jumped");
    }
    [PunRPC]
    public void RPCSyncJump(int id)
    {
        if (pv.IsMine) return;
        if (id == playerID)
        {
            isJumping = true;
            view.SetState(isSliding, isJumping);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.8f);
            Debug.Log($"[PlayerController] Remote player sync jump");
        }
    }
    /*
    private void HandleSlide(int id)
    {
        if (id != playerID || !isAlive || !isSliding) return;
        isSliding = true;
        //Notify to the view
        view.SetState(isSliding, isJumping);
        StartCoroutine(SlideRoutine(slideDuration));
        Debug.Log($"[PlayerController] Player {playerID} has started sliding");
    }
    private IEnumerator SlideRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndSlide();
    }
    private void EndSlide()
    {
        isSliding = false;
        view.SetState(isSliding, isJumping);
        Debug.Log($"[PlayerController] Player {playerID} has ended sliding");
    }*/
    private void HandlePlayerDeath(int id)
    {
        if (playerID ==  id && isAlive)
        {
            isAlive = false;
            //Stop movement
            if (rb != null) rb.linearVelocity = Vector2.zero;
            //Deactivate the input system
            GetComponent<PlayerInputController>().enabled = false;
            //Deactivate the collider
            if (playerCollider != null) playerCollider.enabled = false;
            Debug.Log($"[PlayerController] Player {playerID} is dead");
        }
    }
    //Collision with the ground and score
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isJumping)
            {
                isJumping = false;
                view.SetState(isSliding, isJumping);
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (PhotonNetwork.IsConnectedAndReady && !pv.IsMine) return;

            Debug.Log($"[PlayerController] LOCAL: I hit an obstacle!");

            if (PhotonNetwork.IsConnectedAndReady)
            {
                pv.RPC("RPC_HandlePlayerDeath", RpcTarget.All, playerID);
            }
            else
            {
                //Offline mode
                RPC_HandlePlayerDeath(playerID);
            }
        }
    }
    [PunRPC]
    public void RPC_HandlePlayerDeath(int deadPlayerID)
    {
        if (isGameOverTriggered) return;
        isGameOverTriggered = true;

        Debug.Log($"[GameFlow] Player {deadPlayerID} died. Global Game Over.");

        if (pv != null) pv.enabled = false;

        EventManager.TriggerPlayerDied(deadPlayerID);

        EventManager.TriggerGameOver(deadPlayerID);
        EventManager.TriggerSound("SFX_Death");

        WorldRunner world = Object.FindAnyObjectByType<WorldRunner>();
        if (world != null) world.enabled = false;

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We only syncronize position Y and isJumping
            stream.SendNext(rb.position);
            stream.SendNext(isJumping);
        }
        else
        {
            //We save the position received to interpolate in update
            networkPosition = (Vector2)stream.ReceiveNext();
            isJumping = (bool)stream.ReceiveNext();
            view.SetState(isSliding, isJumping);
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkJumpTime = Time.time;
        }
    }
}

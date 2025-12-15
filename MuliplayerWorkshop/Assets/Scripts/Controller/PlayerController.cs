using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
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

    private Vector2 networkPosition;
    private float networkJumpTime;
    private void Awake()
    {
        if (rb != null) rb = GetComponent<Rigidbody2D>();
        //playerID = 1;//That's for now, we'll assign a real id with photon
        if (pv == null) pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (playerID == -1)
        {
            PlayerController inputController = GetComponent<PlayerController>();
            if (inputController != null)
            {
                inputController.playerID = playerID;
            }
        }
    }
    private void OnEnable()
    {
        EventManager.OnPlayerJump += HandleJump;/*
        EventManager.OnPlayerSlide += HandleSlide;*/
        EventManager.OnPlayerDied += HandlePlayerDeath;
    }
    private void OnDisable()
    {
        EventManager.OnPlayerJump -= HandleJump;/*
        EventManager.OnPlayerSlide -= HandleSlide;*/
        EventManager.OnPlayerDied -= HandlePlayerDeath;
    }
    private void Update()
    {
        if (GetComponent<PhotonView>().IsMine == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, networkPosition, 5f * Time.deltaTime);
        }
    }
    private void HandleJump(int id)
    {
        if (!pv.IsMine) return;
        if (id != playerID || !isAlive || isJumping) return;
        ExecuteJumpLogic();
        //We call the rcp in all clients
        pv.RPC("RPCSyncJump", RpcTarget.Others, playerID);
        Debug.Log($"[PlayerController] RPC sended");
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
        if (id == playerID && !isJumping)
        {
            isJumping = true;
            view.SetState(isSliding, isJumping);
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
            //Deactivade the input system
            GetComponent<PlayerInputController>().enabled = false;
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
            EventManager.TriggerPlayerDied(playerID);
            EventManager.TriggerSound("SFX_Death");
        }
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
            //We saver the position received to interpolate in update
            networkPosition = (Vector2)stream.ReceiveNext();
            isJumping = (bool)stream.ReceiveNext();
            //We calculate the lag
            networkJumpTime = Time.time;
        }
    }
}

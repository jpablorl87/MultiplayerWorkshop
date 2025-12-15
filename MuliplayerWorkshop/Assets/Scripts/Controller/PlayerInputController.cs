using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerControls controls;
    //Detection of of touch controls
    private Vector2 startPos;
    [SerializeField] private float swipeTreshold = 50f;
    //ID of the player
    public int playerID { get; set; } = -1;
    private void Awake()
    {
        controls = new PlayerControls();
        //Keyboard inputs
        controls.Gameplay.Jump.performed += OnJumpPerformed;/*
        controls.Gameplay.Slide.performed += OnSlidePerformed;*/
        //Touchscreen inputs
        controls.Gameplay.Jump.started += OnPressStarted;
        controls.Gameplay.Jump.canceled += OnPressEnded;
    }
    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();
    //Keyboard callbacks
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        EventManager.TriggerPlayerJump(playerID);
        EventManager.TriggerSound("SFX_Jump");
    }/*
    private void OnSlidePerformed(InputAction.CallbackContext context)
    {
        EventManager.TriggerPlayerSlide(playerID);
        EventManager.TriggerSound("SFX_Slide");
    }*/
    //Touchscreen callbacks
    private void OnPressStarted(InputAction.CallbackContext context)
    {
        startPos = controls.Gameplay.Position.ReadValue<Vector2>();
    }
    private void OnPressEnded(InputAction.CallbackContext context)
    {
        Vector2 endPos = controls.Gameplay.Position.ReadValue<Vector2>();
        Vector2 swipeVector = endPos - startPos;
        if (swipeVector.magnitude < swipeTreshold)
        {
            return;
        }
        //If movement is vertical then we continue
        if (Mathf.Abs(swipeVector.y) > Mathf.Abs(swipeVector.x))
        {
            //if is positive, it's going up
            if (swipeVector.y > 0)
            {
                EventManager.TriggerPlayerJump(playerID);
                EventManager.TriggerSound("SFX_Jump");
            }/* This part will be commented for the checkpoint
            else//else is going down
            {
                EventManager.TriggerPlayerSlide(playerID);
                EventManager.TriggerSound("SFX_Slide");
            }*/
        }
    }
}

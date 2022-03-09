using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author : DRUCKES Lucas
/// Initial player controller class
/// Handles basic movements and camera movements
/// </summary>

public class PlayerController : MonoBehaviour
{
    public int playerID = 0; //useless?
    public bool controlled = false; //if false, player won't be able to move this object
    public GameObject playerBodyObject; //get the cylinder gameobject to change its material

    public Transform playerCamera = null;
    public float mouseSensitivity = 3.5f;
    public float walkSpeed = 6.0f;
    public float runSpeed = 15.0f; //when shift is pressed
    public float gravity = -13.0f;
    public float jumpForce = 10.0f;

    [Range(0.0f, 10.0f)] public float moveSmoothTime = 0.3f;
    [Range(0.0f, 0.5f)] public float mouseSmoothTime = 0.03f;

    public bool lockCursor = true;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;

    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    // Start is called before the first frame update
    //MAKE THE PLAYER CONTROLLABLE ACCORDING TO IF ITS A MULTIPLAYER GAME OR LOCAL GAME, AND PLAYER
    void Start()
    {
        if (STEAMAPIMANAGER.instance != null)
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                if (!STEAMAPIMANAGER.instance.network_manager.user.hasLobby)
                {
                    controlled = true;
                }
                else if (STEAMAPIMANAGER.instance.GetLobbyHostSteamID() == STEAMAPIMANAGER.instance.network_manager.user.steamid)
                {
                    playerID = 1;
                    controlled = true;
                }
                else
                {
                    playerID = 2;
                }
            }
            else // SINGLE PLAYER
            {
                controlled = true;
            }
        }
        else // SINGLE PLAYER
        {
            controlled = true;
        }

        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controlled)
        {
            UpdateMouseLook();
            UpdateMovement();
        }
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(currentMouseDelta.x * mouseSensitivity * Vector3.up);
    }

    void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded)
        {
            velocityY = 0.0f;
        }

        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        velocityY += gravity * Time.deltaTime;

        float realSpeed;
        if (Input.GetButton("Sprint"))
        {
            realSpeed = runSpeed;
        }
        else
        {
            realSpeed = walkSpeed;
        }

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * realSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        velocityY = Mathf.Sqrt(jumpForce * -2 * gravity);
    }
}

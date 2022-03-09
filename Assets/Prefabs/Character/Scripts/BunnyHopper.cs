using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Author : DRUCKES Lucas
/// Script taken/inspired from github repository https://github.com/TheAsuro/VelocityMovement (thanks to him!)
/// 
/// A first version of a PlayerController has been made in Prefabs/Character/Scripts/PlayerController.cs.
/// Thus, this script does not have the bunny hop feature and does not require a Camera Script as it handles it on its own.
/// 
/// This script will handle everything player related. Ground speed, air speed, air acceleration etc..
/// Also, will check if the game is multiplayer and gives the control to the player according to its role.
/// 
/// Suggest reading https://www.reddit.com/r/Unity3D/comments/2vwcuw/bunnyhopping_from_the_programmers_perspective_an/ for a better understand of Bunny Hop if you wish
/// as multiple mathematics vector notions are used, such as Dot product.
/// </summary>

public class BunnyHopper : MonoBehaviour
{
    public int playerID = 0;
    public bool controlled = false;

    public float accel = 200f;
    public float airAccel = 200f;
    public float maxSpeed = 6.4f;
    public float maxAirSpeed = 0.6f;
    public float friction = 8f;
    public float jumpForce = 5f;
    public LayerMask groundLayers;

    public GameObject camObj;

    private float lastJumpPress = -1f;
    private float jumpPressDuration = 0.1f;
    private bool onGround = false;

    [Header("UI_Speed")]
    public TMPro.TMP_Text textSpeedUI;


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
    }

    private void Update()
    {
        SetUISpeed();
        CheckShouldDie();

        if (Input.GetButton("Jump"))
        {
            lastJumpPress = Time.time;
        }
    }


    private void FixedUpdate()
    {
        if (controlled)
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 playerVelocity = GetComponent<Rigidbody>().velocity;

            playerVelocity = CalculateFriction(playerVelocity);
            playerVelocity += CalculateMovement(input, playerVelocity);

            GetComponent<Rigidbody>().velocity = playerVelocity;

        }
        
    }
    private void SetUISpeed()
    {
        float speed = new Vector3(GetComponent<Rigidbody>().velocity.x, 0f, GetComponent<Rigidbody>().velocity.z).magnitude;
        if (textSpeedUI != null) 
        { 
            if(speed <= 0.02f)
            {
                textSpeedUI.text = "Speed : " + (0.0f).ToString();
                return;
            }
            textSpeedUI.text = "Speed : " + speed.ToString("F2"); 
        }
    }

    private void CheckShouldDie()
    {
        if (gameObject.transform.position.y < -5.0f)
        {
            gameObject.transform.position = new Vector3(10f, 5f, -10f);
        }
    }
    private Vector3 CalculateFriction(Vector3 currentVelocity)
    {
        onGround = CheckGround();
        float speed = currentVelocity.magnitude;

        if(!onGround || Input.GetButton("Jump") || speed == 0f)
        {
            return currentVelocity;
        }

        float drop = speed * friction * Time.deltaTime;
        return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
    }

    private Vector3 CalculateMovement(Vector2 input, Vector3 velocity)
    {
        onGround = CheckGround();

        float curAccel = accel;
        if (!onGround)
        {
            curAccel = airAccel;
        }

        float curMaxSpeed = maxSpeed;
        if (!onGround)
        {
            curMaxSpeed = maxAirSpeed;
        }

        Vector3 camRotation = new Vector3(0f, camObj.transform.rotation.eulerAngles.y, 0f);
        Vector3 inputVelocity = Quaternion.Euler(camRotation) * new Vector3(input.x * curAccel, 0f, input.y * curAccel);

        Vector3 alignedInputVelocity = new Vector3(inputVelocity.x, 0f, inputVelocity.z) * Time.deltaTime;

        Vector3 currentVelocity = new Vector3(velocity.x, 0f, velocity.z);

        float max = Mathf.Max(0f, 1 - (currentVelocity.magnitude / curMaxSpeed));

        float velocityDot = Vector3.Dot(currentVelocity, alignedInputVelocity);

        Vector3 modifiedVelocity = alignedInputVelocity * max;

        Vector3 correctVelocity = Vector3.Lerp(alignedInputVelocity, modifiedVelocity, velocityDot);

        correctVelocity += GetJumpVelocity(velocity.y);

        return correctVelocity;
    }

    private Vector3 GetJumpVelocity(float yVelocity)
    {
        Vector3 jumpVelocity = Vector3.zero;

        if(Time.time < lastJumpPress + jumpPressDuration && yVelocity < jumpForce && CheckGround())
        {
            lastJumpPress = -1f;
            jumpVelocity = new Vector3(0f, jumpForce - yVelocity, 0f);
        }

        return jumpVelocity;
    }

    private bool CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        bool result = Physics.Raycast(ray, GetComponent<Collider>().bounds.extents.y + 0.1f, groundLayers);
        return result;
    }
}

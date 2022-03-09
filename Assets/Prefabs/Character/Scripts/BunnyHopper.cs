using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
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

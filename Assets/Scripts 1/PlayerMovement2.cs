using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Movement")]
    public float currentMs;
    public float moveSpeed;
    public float runSpeed;

    [Header("Acceleration")]
    public float moveSpeedAcc;
    public float runSpeedAcc;
    public float airDeacc;
    public float groundDeacc;
    public float overRunSpeedAirDeacc;
    public float overRunSpeedGroundDeacc;


    [HideInInspector] public float turnAroundDeacc;


    [Header("Drag")]
    public float groundDrag;
    public float airDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float extraJumpForceWhenSliding;
    public float extraJumpMomentumWhenSliding;
    public float jumpCooldown;
    public float airMultiplier;
    public bool ableToJump;

    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    public bool ableToCrouch;
    [HideInInspector] public bool crouching;

    [Header("Slopes")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    [HideInInspector] public bool grounded;

    public Transform orientation;

    [HideInInspector] public bool sliding;

    float horizontalInput;
    float verticalInput;

    float lastHorizontalInput;
    float lastVerticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        Acceleration();
        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
    }

    private void Acceleration()
    {
        //Make the movement more smooth
        if (!sliding)
        {
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                deAccelerate(0);
            }
            else
            {
                //Check if sprinting
                if (Input.GetKey(sprintKey))
                {
                    currentMs += runSpeedAcc * Time.deltaTime;
                    if (currentMs >= runSpeed)
                    {
                        OverRunSpeedDeaccelerate();
                    }
                }
                //Walking
                else
                {
                    if (currentMs >= moveSpeed)
                    {
                        deAccelerate(0);
                    }
                    else
                    {
                        currentMs += moveSpeedAcc * Time.deltaTime;
                    }
                }
            }

            if (crouching)
            {
                //Set ms to crouchspeed if moving.
                if (currentMs >= crouchSpeed)
                {
                    currentMs = crouchSpeed;
                }
            }
            //--------------------------//
        }
    }

    private void deAccelerate(float extra)
    {
        if (grounded)
        {
            currentMs -= (groundDeacc + extra) * Time.deltaTime;
            if (currentMs <= 0)
            {
                currentMs = 0;
            }
        }
        else
        {
            currentMs -= airDeacc * Time.deltaTime;
            if (currentMs <= 0)
            {
                currentMs = 0;
            }
        }
    }

    private void OverRunSpeedDeaccelerate()
    {
        if (grounded)
        {
            currentMs -= (overRunSpeedGroundDeacc + runSpeedAcc) * Time.deltaTime;
            if (currentMs <= 0)
            {
                currentMs = 0;
            }
        }
        else
        {
            currentMs -= (overRunSpeedAirDeacc + runSpeedAcc) * Time.deltaTime;
            if (currentMs <= 0)
            {
                currentMs = 0;
            }
        }
    }

    public void Crouch()
    {
        //Crouching
        if (grounded && !crouching)
        {
            crouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }

    public void StopCrouch()
    {
        crouching = false;
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public bool canIJump()
    {
        if (Input.GetKey(jumpKey) && readyToJump && grounded && !crouching && ableToJump)
        {
            return true;
        }
        return false;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        // when to jump
        if (canIJump())
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        lastHorizontalInput = horizontalInput;
        lastVerticalInput = verticalInput;
    }

    private void MovePlayer()
    {
        //Make sure the player keeps moving while sliding
        if (sliding && rb.velocity.y <= 0 && (horizontalInput == 0 && verticalInput == 0))
        {
            verticalInput = 1;
        }


        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * currentMs * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // on ground
       else if (grounded)
            rb.AddForce(moveDirection.normalized * currentMs * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * currentMs * 10f * airMultiplier, ForceMode.Force);

        // Turn off gravity when the player is on a slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //Limit speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > currentMs)
            {
                rb.velocity = rb.velocity.normalized * currentMs;
            }

            if(rb.velocity.y > 0 && sliding)
            {
                deAccelerate(5);
            }

        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > currentMs)
            {
                Vector3 limitedVel = flatVel.normalized * currentMs;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float Force;
        if (sliding && !OnSlope())
        {
            Force = jumpForce * extraJumpForceWhenSliding;
        }
        else
        {
            Force = jumpForce;
        }

        if (sliding)
        {
            if(currentMs >= runSpeed - 0.15 && currentMs <= runSpeed + extraJumpMomentumWhenSliding + 1)
            {
                currentMs += extraJumpMomentumWhenSliding;
            }
        }
        rb.AddForce(transform.up * Force, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
            {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}

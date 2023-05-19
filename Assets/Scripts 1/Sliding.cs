using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement2 pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideSpeedDecrease;
    public float slopeSpeedBuildUp;
    public float minSlideSpeed;
    public float slideCooldown;
    public bool ableToSlide;

    private float slideCooldownLeft;

    public float slideYSCale;
    private float startYScale;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    public KeyCode slideAndCrouchKey = KeyCode.C;

    private bool lastGroundedState;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement2>();

        startYScale = playerObj.localScale.y;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(slideCooldownLeft >= 0)
        {
            slideCooldownLeft -= 1 * Time.deltaTime;
        }

        if (Input.GetKey(slideAndCrouchKey) && (!pm.sliding && !pm.crouching) && pm.grounded)
        {
            if (verticalInput >= 0 && ableToSlide && (pm.OnSlope() || (pm.currentMs >= minSlideSpeed 
                && (horizontalInput != 0 || verticalInput != 0))))
            {
                StartSlide();
            }
            else if(pm.ableToCrouch)
            {
                pm.Crouch();
            }
        }

        if (!pm.grounded && pm.sliding)
        {
            StopSlide();
        }

        if(Input.GetKeyUp(slideAndCrouchKey))
        {
            if (pm.sliding)
            {
                StopSlide();
            }
            
            if (pm.crouching)
            {
                pm.StopCrouch();
            }
        }

        if(verticalInput < 0)
        {
            StopSlide();
        }

        lastGroundedState = pm.grounded;
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        pm.sliding = true;


        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYSCale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        if (lastGroundedState && slideCooldownLeft <= 0)
        {
            pm.currentMs += slideForce;
        }
        slideCooldownLeft = slideCooldown;
    }

    private void SlidingMovement()
    {
        Vector3 InputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Sliding down a slope
        if (pm.OnSlope())
        {
            pm.currentMs += slopeSpeedBuildUp * Time.deltaTime;
        }

        //Sliding normal
        else
        {
            pm.currentMs -= slideSpeedDecrease * Time.deltaTime;
        }


        if(pm.currentMs <= minSlideSpeed && !pm.OnSlope())
        {
            StopSlide();
            pm.Crouch();
        }
    }

    private void StopSlide()
    {
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    public float moveSpeed;
    public float defaultSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    public float sprintMultiplier;
    float sprintDuration;
    public float sprintLimit;
    public float sprintCoolDownTimer;
    bool readyToJump;
    public float wallCheckDistance = 0.5f;
    public bool sprinting; // public so camera can access
    bool nearWall;
    bool canSprint;
    

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;



    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
       
        // handle drag
        if (grounded) 
        {
          
            rb.drag = groundDrag;
            readyToJump = true; 
        }
        else 
        {   
            
            rb.drag = 0;
        }
        
        if (sprinting) 
        {
            sprintDuration+= Time.deltaTime; 
            
            
            if (sprintDuration >= sprintLimit) 
            {
                sprinting = false;
                canSprint = false;
                sprintDuration = 0;
                moveSpeed = defaultSpeed;
                StartCoroutine(StartSprintCooldown());
            }   
        }
        
        if (Input.GetKeyUp(sprintKey))
        {
            sprinting = false;
            sprintDuration = 0;
            moveSpeed = defaultSpeed;  // Reset speed when key is let go
        }

    }

    private void FixedUpdate() 
    {
        MovePlayer();    
    }

    private void Start()
    {
     
        defaultSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        canSprint = true;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jumping
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
          
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        // Sprinting (my own code)
        if (Input.GetKey(sprintKey) && canSprint)
        {
            
            if (!sprinting) // Prevent speed stacking
            {
                sprinting = true;
                moveSpeed = defaultSpeed * sprintMultiplier;  // Apply sprint speed once
                
            }
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // add force to player
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }   


    private void SpeedControl() 
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
      
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private IEnumerator StartSprintCooldown()
    {
     
        yield return new WaitForSeconds(sprintCoolDownTimer);
        canSprint = true;
        
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
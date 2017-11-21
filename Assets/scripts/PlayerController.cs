using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float sneakSpeed = 2.5f;
    public float runSpeed = 8.0f;
    public float crouchWalkSpeed = 3.5f;
    public float crouchRunSpeed = 6.5f;
    public float crouchSneakSpeed = 1f;
    public float jumpSpeed = 6.0f;

    public bool limitDiagonalSpeed = true;
    public bool toggleRun = false;
    public bool toggleSneak = false;
    public bool airControl = true; // strafing / b-hop
    public bool firstPerson = false;
    public bool crouching = false;

    public float gravity = 10.0f;
    public float fallingDamageLimit = 10.0f;

    private Vector3 moveDirection;

    private bool grounded;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
	private bool punching;
    private bool playerControl;
    private Animator anim;


    // Use this for initialization
    void Start()
    {
        moveDirection = Vector3.zero;
        grounded = false;
        playerControl = false;
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? 0.6701f : 1.0f;

        anim.SetFloat("BlendX", (inputX * 2));
        anim.SetFloat("BlendY", (inputY * 2));
        anim.SetBool("Walking", (anim.GetFloat("BlendX") != 0 || anim.GetFloat("BlendY") != 0));

		anim.SetBool("Punching", Input.GetButton("Fire1"));

        if (grounded)
        {

            if (falling)
            {
                falling = false;
                if (myTransform.position.y < (fallStartLevel - fallingDamageLimit))
                {
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
                }
            }

            if (!toggleRun)
            {
                bool running = Input.GetButton("Run");
                speed = running ? runSpeed : walkSpeed;
                anim.SetBool("Running", running);
            } 

            else
            {
                anim.SetBool("Running", true);
            }

            if (!toggleSneak)
            {
                bool sneaking = Input.GetButton("Sneak");
                speed = sneaking ? sneakSpeed : speed;
                anim.SetBool("Sneaking", sneaking);
            }

            if (crouching)
            {
                speed = Input.GetButton("Run") ? crouchRunSpeed : crouchWalkSpeed;
                speed = Input.GetButton("Sneak") ? crouchSneakSpeed : speed;


            }

            //print(speed);
            moveDirection = new Vector3(inputX * inputModifyFactor, 0, inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;

            if (!Input.GetButton("Jump"))
            {
                anim.SetBool("Jump", false);
            }
            else 
            {
                moveDirection.y = jumpSpeed;
                
                anim.SetBool("Jump", true);
            }
        }
        else
        {
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }

            if (airControl && playerControl)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        moveDirection.y -= gravity * Time.deltaTime;

        
    }

    void Update()
    {
        if (toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);

        if (Input.GetButtonUp("Crouch"))
        {
            crouching = !crouching;
            anim.SetBool("Crouch", crouching);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //print(hit.point);
    }

    void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }
}

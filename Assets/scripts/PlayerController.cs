using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 5.0f;
    public float sneakSpeed = 2.5f;
    public float runSpeed = 8.0f;
    public float jumpSpeed = 6.0f;

    public bool limitDiagonalSpeed = true;
    public bool toggleRun = false;
    public bool toggleSneak = false;
    public bool slideWhenOverSlope = true;
    public bool slideOnTaggedObject = true;
    public bool airControl = true; // strafing / b-hop

    public float gravity = 20.0f;

    public float fallingDamageLimit = 10.0f;
    public float slideSpeed = 12.0f;
    public float antiBumpFactor = 0.75f;
    public int antiBunnyHopFactor = 0;

    private Vector3 moveDirection;
    private bool grounded;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl;
    private int jumpTimer;
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
        rayDistance = (controller.height * 0.5f) + controller.radius;
        slideLimit = controller.slopeLimit - 0.1f;
        jumpTimer = antiBunnyHopFactor;
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? 0.7071f : 1.0f;
        anim.SetFloat("BlendX", inputX * inputModifyFactor);
        anim.SetFloat("BlendY", inputY * inputModifyFactor);

        if (grounded)
        {
            bool sliding = false;

            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                {
                    sliding = true;
                }
            }
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }

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
                speed = Input.GetButton("Run") ? runSpeed : walkSpeed;
            }

            if (!toggleSneak)
            {
                speed = Input.GetButton("Sneak") ? sneakSpeed : walkSpeed;
            }

            if ((sliding && slideWhenOverSlope) || (slideOnTaggedObject && hit.collider.tag == "Slide"))
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection = moveDirection * slideSpeed;
                playerControl = false;
            }
            else
            {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }

            if (!Input.GetButton("Jump"))
            {
                jumpTimer++;
            }
            else if (jumpTimer > antiBunnyHopFactor)
            {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
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

        moveDirection.y -= gravity * Time.deltaTime;

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        print("X: " + inputX + " Y: " + inputY + " Modifier: " + inputModifyFactor);
    }

    void Update()
    {
        if (toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }

    void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }
}

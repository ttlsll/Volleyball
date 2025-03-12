using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("HorizontalMovement")]
    public float walkSpeed;
    public float runSpeed;
    private float playerSpeed;
    float horizontalInput;

    [Header("Jumping")]
    public Rigidbody2D rb;
    public LayerMask Ground;
    private bool isGrounded = true;
    public float jumpPower;
    public float maxJumpTime;
    private float jumpTime;
    private bool isJumping = false;
    private float basicGravity;
    public float fallingGravityMultiplyer;

    [Header("Controls")]
    public string JumpButton;
    public string HorizontalAxis;
    public string RunButton;
    public string PunchButton;
    public string KickButton;
    public string DefenceButton;

    [Header("Actions")]
    public float punchPower;
    public float kickPower;
    public float defencePower;
    public float superMiltiplier;
    public float appliedPower;
    public float resetTime;

    private MovementState movementState;
    public ActionState actionState;

    public Animator animator;
    public Transform ballTransform;
    public SpriteRenderer spriteRenderer;
    private bool isSpriteFlipped = false;


    private void Awake()
    {
        basicGravity = rb.gravityScale;
    }


    void Update()
    {
        RunCheck();
        horizontalInput = Input.GetAxisRaw(HorizontalAxis);
        transform.position += transform.right * horizontalInput * playerSpeed * Time.deltaTime;

        GroundCheck();
        JumpStart();
        JumpProcess();
        FallCheck();

        ActionStateHandler();
        MovementStateHandler();
        AnimationHandler();
        
    }



    //checks if the player is on the ground to allow jump
    void GroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, new Vector2(1,2f), 0f, Vector2.down,0.3f,Ground))
        {
            isGrounded = true;

            rb.gravityScale = basicGravity;
        }
        else
        {
            isGrounded = false;
        }
    }



    //to manage player's speed
    void RunCheck()
    {
        if (Input.GetButton(RunButton)) playerSpeed = runSpeed;
        else playerSpeed = walkSpeed;
    }


    //throws player up
    void JumpStart()
    {
        if (!Input.GetButtonDown(JumpButton)) return;
        
        if (isGrounded)
        {
            rb.velocity = Vector2.up * jumpPower;
            jumpTime = maxJumpTime;
            isJumping = true;
        }
    }


    //to stop jumping or continue going up
    void JumpProcess()
    {
        if (!isJumping) return;

        if (jumpTime > 0)
        {
            jumpTime -= Time.deltaTime;
            rb.velocity = Vector2.up * jumpPower;
        }

        if (Input.GetButtonUp(JumpButton))
        {
            isJumping = false;
            FallCheck();
        }
    }


    //to make falling faster by increasing gravity. gravity sets back when grounded
    void FallCheck()
    {
        if (isGrounded) return;

        if (rb.gravityScale == basicGravity)
        {
            rb.gravityScale *= fallingGravityMultiplyer;
        }
    }


    //to know what how the player is moving
    private enum MovementState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Fall
    }


    //to know how the player wants to interact with ball
    public enum ActionState
    {
        None,
        // interaction with ball
        Punch,
        Kick,
        Defence
    }


    void MovementStateHandler()
    {
        
        if (!isGrounded && rb.velocity.y < 0)
        {
            movementState = MovementState.Fall;
        }
        else if (!isGrounded)
        {
            movementState = MovementState.Jump;
        }
        else if (horizontalInput != 0 && playerSpeed == runSpeed)
        {
            movementState = MovementState.Run;
        }
        else if (horizontalInput != 0)
        {
            movementState = MovementState.Walk;
        }
        else movementState = MovementState.Idle;
    }


    void ActionStateHandler()
    {
        if (!(actionState == ActionState.None || actionState == ActionState.Defence)) return;


        if (Input.GetButtonDown(PunchButton) && Input.GetButton(RunButton))
        {
            appliedPower = punchPower * superMiltiplier;
            actionState = ActionState.Punch;
            Invoke(nameof(ResetActionState), resetTime);
            
        }
        else if (Input.GetButtonDown(PunchButton))
        {
            appliedPower = punchPower;
            actionState = ActionState.Punch;
            Invoke(nameof(ResetActionState), resetTime);

        }
        else if (Input.GetButtonDown(KickButton) && Input.GetButton(RunButton))
        {
            appliedPower = kickPower * superMiltiplier;
            actionState = ActionState.Kick;
            Invoke(nameof(ResetActionState), resetTime);
            
        }
        else if (Input.GetButtonDown(KickButton))
        {
            appliedPower = kickPower;
            actionState = ActionState.Kick;
            Invoke(nameof(ResetActionState), resetTime);

        }
        else if (Input.GetButton(DefenceButton))
        {
            appliedPower = defencePower;
            actionState = ActionState.Defence;
        }
        else
        {
            actionState = ActionState.None;
            appliedPower = 0;
        }
    }


    void ResetActionState() 
    { 
        actionState = ActionState.None;
    }


    void AnimationHandler()
    {
        //flips player sprite so its facing the ball
        if (ballTransform.position.x < transform.position.x && !isSpriteFlipped)
        {
            spriteRenderer.flipX = true;
            isSpriteFlipped = true;
        }

        if (ballTransform.position.x > transform.position.x && isSpriteFlipped)
        {
            spriteRenderer.flipX = false;
            isSpriteFlipped = false;
        }
        
   
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        //animations
        if (actionState != ActionState.None)
        {
            if (actionState == ActionState.Punch) animator.SetBool("Punch", true);
            if (actionState == ActionState.Kick) animator.SetBool("Kick", true);
            if (actionState == ActionState.Defence) animator.SetBool("Defence", true);
        }
        else
        {
            animator.SetBool("Punch", false);
            animator.SetBool("Kick", false);
            animator.SetBool("Defence", false);

            if (movementState == MovementState.Fall)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Fall", true);
            }
            else if (movementState == MovementState.Jump)
            {
                animator.SetBool("Jump", true);
            }
            else if (movementState != MovementState.Fall)
            {
                animator.SetBool("Fall", false);
            }
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{


    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDedected;
    private bool facingRight = true;
    private int facingDir = 1;
    private Rigidbody2D rb;
    private Animator anim;
    private float xInput;
    private float yInput;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        UpdateAirborneStatus();

        HandleInputs();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDedected && rb.linearVelocityY < 0;

        float yModifier = yInput < 0 ? 1 : .05f;

        if (!canWallSlide)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * yModifier);

    }

    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
        {
            HandleLanding();
        }
        if (!isGrounded && !isAirborne)
        {
            BecomeAirborne();
        }
    }

    private void BecomeAirborne()
    {
        isAirborne = true;
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;
    }

    private void HandleInputs()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            JumpButton();

    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
    }


    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

    private void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpForce);
        canDoubleJump = false;
    }

    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDedected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDedected", isWallDedected);
    }
    void HandleFlip()
    {
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
            Flip();
    }
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * -1;
    }

    private void HandleMovement()
    {
        if (isWallDedected)
            return;

        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}

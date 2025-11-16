using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;
    private bool canBeControlled;
    private float defaultGravityScale;

    [Header("Wall Instructions")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private bool isWallJumping;
    private bool isWallDedected;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;

    [Header("Buffer & Cayote Jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated;
    [SerializeField] private float cayoteJumpWindow = .25f;
    private float cayoteJumpActivated;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    [Header("VFX")]
    [SerializeField] private GameObject deathVFX;

    [Header("References and Variables")]
    private bool facingRight = true;
    private int facingDir = 1;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;
    private float xInput;
    private float yInput;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponent<CapsuleCollider2D>();
    }
    void Start()
    {
        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);
    }

    void Update()
    {

        UpdateAirborneStatus();

        if (!canBeControlled)
            return;

        if (isKnocked)
            return;

        HandleInputs();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }
    public void RespawnFinished(bool finished)
    {
        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;

        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }
    }
    public void Knockback(float sourceDamageXPos)

    {
        float knockbackDir = 1;
        if (transform.position.x < sourceDamageXPos)
        {
            knockbackDir = -1;
        }
        if (isKnocked)
            return;

        StartCoroutine(KnockbackRoutine());
        rb.linearVelocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }
    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        anim.SetBool("isKnocked", true);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        anim.SetBool("isKnocked", false);
    }

    public void Die()
    {
        Instantiate(deathVFX, transform.position, quaternion.identity);
        Destroy(gameObject);
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

        if (rb.linearVelocityY < 0)
            ActivateCayoteJump();
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;
        AttemptBufferJump();
    }

    private void HandleInputs()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }


    }
    #region Buffer & Cayote Jump
    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    private void ActivateCayoteJump() => cayoteJumpActivated = Time.time;
    private void CancelCayoteJump() => cayoteJumpActivated = Time.time - 1;
    private void RequestBufferJump()
    {
        if (isAirborne)
            bufferJumpActivated = Time.time;
    }
    #endregion
    private void JumpButton()
    {
        bool cayoteJumpAvaible = Time.time < cayoteJumpActivated + cayoteJumpWindow;
        if (isGrounded || cayoteJumpAvaible)
        {
            Jump();
        }
        else if (isWallDedected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
        CancelCayoteJump();
    }


    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

    private void DoubleJump()
    {
        isWallJumping = false;
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpForce);
    }
    private void WallJump()
    {
        canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());

    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }
    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDedected && rb.linearVelocityY < 0;

        float yModifier = yInput < 0 ? 1 : .05f;

        if (!canWallSlide)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * yModifier);

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
        if (isWallJumping)
            return;

        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}

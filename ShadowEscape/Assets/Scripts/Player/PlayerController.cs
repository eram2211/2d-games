using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.Player
{
    /// <summary>
    /// Handles horizontal movement, jumping, double jumping and footstep/jump audio.
    /// Attach to: Player GameObject (must also have Rigidbody2D + Collider2D + Animator).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float acceleration = 60f;
        [SerializeField] private float airControlMultiplier = 0.6f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private int maxJumps = 2; // enables double jump
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Footsteps")]
        [SerializeField] private float footstepInterval = 0.35f;

        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private float moveInput;
        private int jumpsRemaining;
        private float coyoteTimer;
        private float jumpBufferTimer;
        private float footstepTimer;
        private bool isGrounded;

        // Public read-only state so other scripts (e.g. ShadowClone) can mirror actions
        public Vector2 Velocity => rb.velocity;
        public bool IsGrounded => isGrounded;
        public bool FacingRight { get; private set; } = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

            HandleInput();
            HandleTimers();
            HandleAnimationAndFlip();
            HandleFootsteps();
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

            CheckGrounded();
            ApplyHorizontalMovement();
        }

        private void HandleInput()
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferTimer = jumpBufferTime;
            }
            else
            {
                jumpBufferTimer -= Time.deltaTime;
            }

            // Consume jump if buffered and allowed
            if (jumpBufferTimer > 0f && jumpsRemaining > 0)
            {
                DoJump();
                jumpBufferTimer = 0f;
            }
        }

        private void HandleTimers()
        {
            if (isGrounded)
            {
                coyoteTimer = coyoteTime;
            }
            else
            {
                coyoteTimer -= Time.deltaTime;
            }
        }

        private void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = groundCheck != null &&
                         Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // Reset jumps when we (re)touch the ground, or while coyote time is active
            if (isGrounded && !wasGrounded)
            {
                jumpsRemaining = maxJumps;
            }
            else if (isGrounded)
            {
                jumpsRemaining = maxJumps;
            }
        }

        private void ApplyHorizontalMovement()
        {
            float targetSpeed = moveInput * moveSpeed;
            float control = isGrounded ? 1f : airControlMultiplier;
            float newX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, acceleration * control * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newX, rb.velocity.y);
        }

        private void DoJump()
        {
            // First jump can use coyote time even if just left the ground
            bool usingCoyote = jumpsRemaining == maxJumps && coyoteTimer <= 0f && !isGrounded;

            if (jumpsRemaining <= 0) return;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsRemaining--;
            coyoteTimer = 0f;

            animator.SetTrigger("Jump");
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.jumpClip);
        }

        private void HandleAnimationAndFlip()
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.velocity.y);

            if (moveInput > 0.01f && !FacingRight) Flip();
            else if (moveInput < -0.01f && FacingRight) Flip();
        }

        private void Flip()
        {
            FacingRight = !FacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }

        private void HandleFootsteps()
        {
            if (isGrounded && Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    AudioManager.Instance?.PlaySfx(AudioManager.Instance.footstepClip, 0.6f);
                    footstepTimer = footstepInterval;
                }
            }
            else
            {
                footstepTimer = 0f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

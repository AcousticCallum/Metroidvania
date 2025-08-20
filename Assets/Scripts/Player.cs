using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public static Player instance;

    [Space]

    public Animator animator;
    public Rig rig;

    [Space]

    public PlayerWeapon weapon;
    public List<PlayerWeapon> weapons = new();

    [Space]

    public Collider2D defaultCollider;
    public GameObject defaultSprite;

    [Space]

    public Collider2D dashCollider;
    public GameObject dashSprite;

    [Space]

    public Transform cameraTarget;
    [SerializeField] private Vector2 defaultCameraOffset;
    [SerializeField] private Vector2 dashCameraOffset;

    [Space]

    [SerializeField] private float jumpHeight;
    [SerializeField] private int additionalJumpCount;
    private int additionalJumpCounter;

    [Space]

    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveControl;
    [SerializeField] private float stopControl;

    [Space]

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashControl;
    [SerializeField] private float dashSpinSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private int dashCount;
    [SerializeField] private LineRenderer dashLine;

    [Space]

    [SerializeField] private LayerMask groundMask;

    private const float COYOTE_TIME = 0.1f;
    private float coyoteTimer;

    private const float JUMP_BUFFER_TIME = 0.1f;
    private float jumpBufferTimer;

    private const float DASH_TIME_SCALE = 0.0f;
    private const float DASH_TIME_SCALE_RATE = 25.0f;
    private float dashDurationTimer;
    private float dashCooldownTimer;
    private int dashCounter;
    private bool dashCharging, dashing;

    private float groundAngle;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        /// Timers
        coyoteTimer = Mathf.Max(coyoteTimer - Time.deltaTime, 0.0f);
        jumpBufferTimer = Mathf.Max(jumpBufferTimer - Time.deltaTime, 0.0f);
        dashDurationTimer = Mathf.Max(dashDurationTimer - Time.deltaTime, 0.0f);
        if(!dashing && !dashCharging) dashCooldownTimer = Mathf.Max(dashCooldownTimer - Time.deltaTime, 0.0f);
        if (dashCooldownTimer == 0.0f) dashCounter = dashCount;

        /// Inputs

        // Move
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = JUMP_BUFFER_TIME;
        }

        // Dash
        bool dashInput = Input.GetButton("Aim");

        if (dashCounter > 0 && (dashCharging || dashInput))
        {
            dashCharging = true;

            Time.timeScale = Mathf.Lerp(Time.timeScale, DASH_TIME_SCALE, DASH_TIME_SCALE_RATE * Time.deltaTime);

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dashDirection = (mousePos - (Vector2)transform.position).normalized;

            // On ground and dash direction is mostly horizontal
            if (Grounded())
            {
                if (Vector2.Angle(dashDirection, Vector2.up) >= 60.0f)
                {
                    dashDirection.y = 0.0f;
                    dashDirection.Normalize();
                }
            }

            if (!dashInput) // End dash
            {
                rb.velocity = dashDirection * dashSpeed;

                Time.timeScale = 1.0f;

                SoundManager.instance.PlaySound("Dash");

                dashLine.gameObject.SetActive(false);

                dashDurationTimer = dashDuration;
                dashCooldownTimer = dashCooldown;

                dashCounter--;

                dashCharging = false;
                dashing = true;
            }
            else
            {
                dashLine.gameObject.SetActive(true);
                dashLine.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, dashDirection));
            }
        }

        bool dashOngoing = dashCharging || dashing;

        rig.weight = dashOngoing ? 0.0f : 1.0f;

        // Change Weapon
        int weaponChangeInput = CheckNumberInputs();
        if (weaponChangeInput >= 0 && weaponChangeInput < weapons.Count)
        {
            weapon.Deactivate();
            weapon = weapons[weaponChangeInput];
        }

        // Aim
        bool aimInput = Input.GetButton("Fire");

        if(aimInput && !dashOngoing) weapon.Activate();
        else weapon.Deactivate();
        weapon.weaponEnd.gameObject.SetActive(!dashOngoing);

        /// Grounded check
        bool grounded = false;
        if (Grounded())
        {
            additionalJumpCounter = additionalJumpCount;

            grounded = true;
        }

        if (jumpBufferTimer > 0.0f && (grounded || additionalJumpCounter > 0))
        {
            float newVelocityY = Mathf.Sqrt(jumpHeight * -Physics2D.gravity.y * 2.0f);
            rb.velocity = new Vector2(rb.velocity.x, newVelocityY);

            if (!dashOngoing) animator.SetTrigger("Jump");
            SoundManager.instance.PlaySound("Jump");

            coyoteTimer = 0.0f;
            jumpBufferTimer = 0.0f;

            if (!grounded) additionalJumpCounter--;
        }

        /// Other
        float targetVelocityX = moveInput * moveSpeed;
        if(Mathf.Abs(groundAngle) > 40 && targetVelocityX * Mathf.Sign(groundAngle) < 0.0f) targetVelocityX = 0.0f; // Prevent moving against the slope

        float control = dashOngoing ?
            ((targetVelocityX != 0.0f && targetVelocityX * rb.velocity.x < 0.0f) || Mathf.Abs(targetVelocityX) > Mathf.Abs(rb.velocity.x) ?
                dashControl :
                0.0f) :
            (targetVelocityX * rb.velocity.x < 0.0f ?
                stopControl :
                moveControl);

        float newVelocityX = Mathf.Lerp(rb.velocity.x, targetVelocityX, control * Time.deltaTime);

        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);

        defaultCollider.enabled = !dashOngoing;
        dashCollider.enabled = dashOngoing;

        defaultSprite.SetActive(!dashOngoing);
        dashSprite.SetActive(dashOngoing);

        cameraTarget.localPosition = !dashOngoing ? defaultCameraOffset : dashCameraOffset;

        animator.SetBool("Dashing", dashOngoing);

        float aimDirection = Vector3.Dot((weapon.aimTarget.transform.position - transform.position).normalized, Vector3.right);

        if (dashOngoing)
        {
            animator.transform.RotateAround(GetCenter(), Vector3.forward, Time.deltaTime * dashSpinSpeed * -rb.velocityX);
        }
        else
        {
            animator.transform.localPosition = Vector3.zero;

            animator.transform.localRotation = Quaternion.Euler(0.0f, aimDirection < 0.0f ? -90.0f : 90.0f, 0.0f);
        }
        
        animator.SetFloat("MoveSpeed", Mathf.Abs(rb.velocityX));
        animator.SetFloat("SignedMoveSpeed", aimDirection < 0.0f ? -rb.velocityX : rb.velocityX);

        animator.SetBool("Falling", !Grounded());
    }

    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.CircleCast(rb.position + 0.375f * Vector2.up, 0.35f, Vector2.down, 0.1f, groundMask);
        
        if(hit.collider)
        {
            groundAngle = Vector2.SignedAngle(hit.normal, Vector2.up);

            if (rb.velocity.y < 0.01f && Mathf.Abs(groundAngle) <= 40)
            {
                if (!Grounded()) SoundManager.instance.PlaySound("Land");

                coyoteTimer = COYOTE_TIME;

                // Dash End
                if (dashDurationTimer == 0.0f) dashing = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Destructible destructible))
        {
            if (destructible.dashOnly && !dashing) return;

            Vector2 normal = collision.GetContact(0).normal;
            float speed = Vector2.Dot(collision.relativeVelocity, normal);
            if (destructible.TryDestruct(speed))
            {
                rb.velocity += destructible.bounciness * speed * normal; // Bounce
            }
        }
    }

    public override void OnDamage()
    {

    }

    public override void OnDie()
    {
        Invoke(nameof(ReloadScene), 1.0f);
    }

    public bool Grounded()
    {
        return coyoteTimer > 0.0f && rb.velocity.y < 0.01f;
    }

    public Vector2 GetCenter()
    {
        return (Vector2)transform.position + (!dashing && !dashCharging ? defaultCameraOffset : dashCameraOffset) * Vector2.up;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private int CheckNumberInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) return 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) return 4;
        return -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + (Vector3)dashCameraOffset, 0.05f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + (Vector3)defaultCameraOffset, 0.05f);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(rb.position + 0.375f * Vector2.up, 0.35f);
        Gizmos.DrawWireSphere(rb.position + 0.375f * Vector2.up + 0.1f * Vector2.down, 0.35f);
    }
}

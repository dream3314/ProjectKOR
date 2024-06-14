using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;
    public bool Stun = false;

    private bool IsFaceRight = true;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashDirection = 1f;
    private float stunTimer = 0f;
    private int currentStunHealth;
    private SwordController swordController;

    [SerializeField] private Animator animator;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float JumpingPower = 16f;
    [SerializeField] private Transform GroundTouch;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private int stunHealth = 5;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float stunDuration = 5f;

    private bool isChargingDash = false;
    private float dashChargeTime = 0f;
    private float maxDashChargeTime = 2f;
    private float[] dashSpeedLevels = { 10f, 15f };
    private float[] dashDurationLevels = { 0.2f, 0.4f };
    private float chargeMoveSpeed = 2f;
    private PlayerType playerType;

    public enum PlayerType
    {
        None = -1,
        Player1, Player2
    }

    public void Init(PlayerType playerType)
    {
        this.playerType = playerType;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        swordController = GetComponent<SwordController>();
        currentStunHealth = stunHealth;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Stun)
        {
            HandleStun();
        }
        else
        {
            HandleMovement();
            HandleJump();
            HandleDash();
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (isDashing)
        {
            PerformDash();
        }
        else if (!Stun && !isChargingDash)
        {
            MovePlayer();
            WrapAroundScreen();
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetBool("IsRun", horizontal != 0);
        if (!isChargingDash)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.X) && IsGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpingPower);
        }
        if (Input.GetKeyUp(KeyCode.X) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartDashCharge();
        }
        if (Input.GetKey(KeyCode.Z))
        {
            ChargeDash();
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * chargeMoveSpeed, rb.velocity.y);
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            ExecuteDash();
        }
    }

    private void HandleStun()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            Stun = false;
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void PerformDash()
    {
        rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    private void WrapAroundScreen()
    {
        if (transform.position.x < screenBounds.x * -1 - objectWidth)
        {
            transform.position += new Vector3(screenBounds.x * 2, 0f, 0f);
        }
        else if (transform.position.x > screenBounds.x + objectWidth)
        {
            transform.position += new Vector3(-screenBounds.x * 2, 0f, 0f);
        }
    }

    private bool IsGround()
    {
        return Physics2D.OverlapCircle(GroundTouch.position, 0.2f, GroundLayer);
    }

    private void Flip()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (IsFaceRight && horizontal < 0f || !IsFaceRight && horizontal > 0f)
        {
            IsFaceRight = !IsFaceRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void StartDashCharge()
    {
        isChargingDash = true;
        dashChargeTime = 0f;
    }

    private void ChargeDash()
    {
        dashChargeTime += Time.deltaTime;
        dashChargeTime = Mathf.Min(dashChargeTime, maxDashChargeTime);
    }

    private void ExecuteDash()
    {
        isChargingDash = false;
        isDashing = true;

        int level = dashChargeTime < 1f ? 0 : 1;
        dashSpeed = dashSpeedLevels[level];
        dashDuration = dashDurationLevels[level];

        dashTimer = dashDuration;
        dashDirection = IsFaceRight ? 1f : -1f;
        speed = dashSpeed;

        photonView.RPC("SyncDash", RpcTarget.Others, dashSpeed, dashDuration, dashDirection);
    }

    [PunRPC]
    private void SyncDash(float dashSpeed, float dashDuration, float dashDirection)
    {
        this.dashSpeed = dashSpeed;
        this.dashDuration = dashDuration;
        this.dashDirection = dashDirection;
        isDashing = true;
        dashTimer = dashDuration;
    }

    public void StunPlayer(float duration)
    {
        Stun = true;
        stunTimer = duration;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Stun");
    }

    public void StunPlayer()
    {
        StunPlayer(stunDuration);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Player"))
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null && !otherPlayer.isDashing)
            {
                ApplyStunDamage(otherPlayer, 1);
            }
        }
    }

    public void ApplyStunDamage(PlayerController otherPlayer, int damage)
    {
        otherPlayer.currentStunHealth -= damage;
        if (otherPlayer.currentStunHealth <= 0)
        {
            otherPlayer.StunPlayer();
            otherPlayer.currentStunHealth = stunHealth;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(animator.GetBool("IsRun"));
            stream.SendNext(animator.GetBool("IsJump"));
        }
        else
        {
            rb.position = (Vector2)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();
            animator.SetBool("IsRun", (bool)stream.ReceiveNext());
            animator.SetBool("IsJump", (bool)stream.ReceiveNext());
        }
    }
}
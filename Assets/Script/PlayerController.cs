using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;
    public bool Stun = false;

    private bool IsFaceRight = true;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private bool isDashing = false;
    private float dashDuration = 0.2f; // �뽬 ���� �ð� (��)
    private float dashTimer = 0f;
    private float dashDirection = 1f; // �뽬 ����
    private float stunDuration = 5f; // Stun ���� �ð� (��)
    private float stunTimer = 0f;
    private int currentStunHealth;
    private SwordController swordController;

    [SerializeField] private Animator animator;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float JumpingPower = 16f;
    [SerializeField] private Transform GroundTouch;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private int stunHealth = 5; // ���� ü��

    // ��� ���� ���� ���� �߰�
    private bool isChargingDash = false;
    private float dashChargeTime = 0f;
    private float maxDashChargeTime = 2f;
    private float[] dashSpeedLevels = { 10f, 15f }; // �ܰ躰 ��� �ӵ�
    private float[] dashDurationLevels = { 0.2f, 0.4f }; // �ܰ躰 ��� ���� �ð�
    private float chargeMoveSpeed = 2f; // ���� �� �̵� �ӵ�


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
        if (!Stun)
        {
            float Horizontal = 0f;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Horizontal = -1f;
                animator.SetBool("IsRun", true);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Horizontal = 1f;
                animator.SetBool("IsRun", true);
            }
            else
            {
                animator.SetBool("IsRun", false);
            }

            // X Ű�� ������ �� ����
            if (Input.GetKeyDown(KeyCode.X) && IsGround())
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpingPower);
            }
            if (Input.GetKeyUp(KeyCode.X) && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            // ��� ���� ����
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartDashCharge();
            }

            // ��� ���� ��
            if (Input.GetKey(KeyCode.Z))
            {
                ChargeDash();
                // ���� ���� �� �÷��̾� �̵�
                rb.velocity = new Vector2(Horizontal * chargeMoveSpeed, rb.velocity.y);
            }

            // ��� ����
            if (Input.GetKeyUp(KeyCode.Z))
            {
                ExecuteDash();
            }

            Flip(Horizontal);
        }

        // Stun ���� �ð� ��� üũ
        if (Stun)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                Stun = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // �뽬 ���� ���
            rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);

            // �뽬 �ð��� ������ �뽬 ����
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                speed = 5f; // �뽬�� ����Ǹ� �ٽ� �⺻ �ӵ��� ���ư�
            }
        }
        else if (!Stun && !isChargingDash)
        {
            float Horizontal = 0f;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Horizontal = -1f;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Horizontal = 1f;
            }

            rb.velocity = new Vector2(Horizontal * speed, rb.velocity.y);

            // ȭ���� ����� �ݴ������� �̵�
            if (transform.position.x < screenBounds.x * -1 - objectWidth)
            {
                transform.position += new Vector3(screenBounds.x * 2, 0f, 0f);
            }
            else if (transform.position.x > screenBounds.x + objectWidth)
            {
                transform.position += new Vector3(-screenBounds.x * 2, 0f, 0f);
            }
        }
    }

    private bool IsGround()
    {
        return Physics2D.OverlapCircle(GroundTouch.position, 0.2f, GroundLayer);
    }

    private void Flip(float Horizontal)
    {
        if (IsFaceRight && Horizontal < 0f || !IsFaceRight && Horizontal > 0f)
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
        if (isChargingDash)
        {
            dashChargeTime += Time.deltaTime;
            if (dashChargeTime > maxDashChargeTime)
            {
                dashChargeTime = maxDashChargeTime;
            }
        }
    }

    private void ExecuteDash()
    {
        isChargingDash = false;
        isDashing = true;

        // ������ �ð��� ���� ��� �ӵ��� ���� �ð� ����
        if (dashChargeTime < 1f)
        {
            dashSpeed = dashSpeedLevels[0];
            dashDuration = dashDurationLevels[0];
        }
        else
        {
            dashSpeed = dashSpeedLevels[1];
            dashDuration = dashDurationLevels[1];
        }

        dashTimer = dashDuration;
        dashDirection = IsFaceRight ? 1f : -1f; // ���� �ٶ󺸴� �������� �뽬
        speed = dashSpeed; // �뽬 �ӵ� ����
    }

    public void StunPlayer(float duration)
    {
        Stun = true;
        stunTimer = duration;
        rb.velocity = Vector2.zero; // ���� ������ �� ������ ����

        // ���� �ִϸ��̼� Ʈ����
        animator.SetTrigger("Stun");
    }

    // �⺻ Stun ���� �ð��� ����ϴ� �޼��� �߰�
    public void StunPlayer()
    {
        StunPlayer(stunDuration);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Player"))
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null && !otherPlayer.isDashing) // �ٸ� �÷��̾ �뽬 ���� �ƴ� ��쿡�� ���� �������� ��
            {
                ApplyStunDamage(otherPlayer, 1); // �뽬�� �� 1 �������� ��
            }

            // �뽬�� �÷��̾�� ���� �������� ���� ����
        }
    }

    public void ApplyStunDamage(PlayerController otherPlayer, int damage)
    {
        otherPlayer.currentStunHealth -= damage;
        Debug.Log("Stun health left: " + otherPlayer.currentStunHealth);
        if (otherPlayer.currentStunHealth <= 0)
        {
            otherPlayer.StunPlayer();
            otherPlayer.currentStunHealth = stunHealth; // Reset stun health if needed
        }
    }
}
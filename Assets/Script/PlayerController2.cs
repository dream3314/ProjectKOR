using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;
    public Animator animator;
    public bool Stun = false;

    private float speed = 5f;
    private float JumpingPower = 16f;
    private bool IsFaceRight = true;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private bool isDashing = false;
    private float dashSpeed = 10f;
    private float dashDuration = 0.2f; // �뽬 ���� �ð� (��)
    private float dashTimer = 0f;
    private float dashDirection = 1f; // �뽬 ����
    private float stunDuration = 5f; // Stun ���� �ð� (��)
    private float stunTimer = 0f;

    [SerializeField] private Transform GroundTouch;
    [SerializeField] private LayerMask GroundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
    }

    void Update()
    {
        if (!Stun)
        {
            float Horizontal = 0f;

            if (Input.GetKey(KeyCode.Keypad4))
            {
                Horizontal = -1f;
            }
            else if (Input.GetKey(KeyCode.Keypad6))
            {
                Horizontal = 1f;
            }

            animator.SetFloat("Run", Mathf.Abs(Horizontal));
            animator.SetBool("Jump", !IsGround());

            // ���� �Է� ����
            if (Input.GetKeyDown(KeyCode.P) && IsGround())
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpingPower);
            }
            if (Input.GetKeyUp(KeyCode.P) && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            // �뽬 �Է� ����
            if (Input.GetKeyDown(KeyCode.O) && Horizontal != 0f)
            {
                StartDash(Horizontal);
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
        else if (!Stun)
        {
            float Horizontal = 0f;

            if (Input.GetKey(KeyCode.Keypad4))
            {
                Horizontal = -1f;
            }
            else if (Input.GetKey(KeyCode.Keypad6))
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

    private void StartDash(float Horizontal)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = Horizontal > 0f ? 1f : -1f; // �뽬 ���� ����
        speed = dashSpeed; // �뽬 �ӵ� ����
    }

    public void StunPlayer(float duration)
    {
        Stun = true;
        stunTimer = duration;
        rb.velocity = Vector2.zero; // ���� ������ �� ������ ����
    }

    // �⺻ Stun ���� �ð��� ����ϴ� �޼��� �߰�
    public void StunPlayer()
    {
        StunPlayer(stunDuration);
    }
}
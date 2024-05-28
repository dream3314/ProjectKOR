using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;
    public Animator animator;
    public bool Stun = false;

    private bool IsFaceRight = true;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private bool isDashing = false;
    private float dashDuration = 0.2f; // 대쉬 지속 시간 (초)
    private float dashTimer = 0f;
    private float dashDirection = 1f; // 대쉬 방향
    private float stunDuration = 5f; // Stun 지속 시간 (초)
    private float stunTimer = 0f;
    private SwordController swordController;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float JumpingPower = 16f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private Transform GroundTouch;
    [SerializeField] private LayerMask GroundLayer;

    // 대시 충전 관련 변수 추가
    private bool isChargingDash = false;
    private float dashChargeTime = 0f;
    private float maxDashChargeTime = 2f;
    private float[] dashSpeedLevels = { 10f, 20f }; // 단계별 대시 속도
    private float[] dashDurationLevels = { 0.2f, 0.4f }; // 단계별 대시 지속 시간

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        swordController = GetComponent<SwordController>();
    }

    void Update()
    {
        if (!Stun)
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

            animator.SetFloat("Run", Mathf.Abs(Horizontal));
            animator.SetBool("Jump", !IsGround());

            // X 키를 눌렀을 때 점프
            if (Input.GetKeyDown(KeyCode.X) && IsGround())
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpingPower);
            }
            if (Input.GetKeyUp(KeyCode.X) && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            // 대시 충전 시작
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartDashCharge();
            }

            // 대시 충전 중
            if (Input.GetKey(KeyCode.Z))
            {
                ChargeDash();
            }

            // 대시 시작
            if (Input.GetKeyUp(KeyCode.Z))
            {
                ExecuteDash();
            }

            Flip(Horizontal);
        }

        // Stun 상태 시간 경과 체크
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
            // 대쉬 중인 경우
            rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);

            // 대쉬 시간이 지나면 대쉬 종료
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                speed = 5f; // 대쉬가 종료되면 다시 기본 속도로 돌아감
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

            // 화면을 벗어나면 반대편으로 이동
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

        // 충전된 시간에 따라 대시 속도와 지속 시간 결정
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
        dashDirection = IsFaceRight ? 1f : -1f; // 현재 바라보는 방향으로 대쉬
        speed = dashSpeed; // 대쉬 속도 적용
    }

    public void StunPlayer(float duration)
    {
        Stun = true;
        stunTimer = duration;
        rb.velocity = Vector2.zero; // 스턴 상태일 때 움직임 멈춤
    }

    // 기본 Stun 지속 시간을 사용하는 메서드 추가
    public void StunPlayer()
    {
        StunPlayer(stunDuration);
    }
}
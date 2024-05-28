using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public float zoomInSize = 3f;
    public float zoomOutSize = 5f;
    public float zoomSpeed = 0.1f;
    public Vector3 zoomInOffset = new Vector3(0, 0, -10); // 줌인 시 카메라 위치 오프셋

    private bool isTouchingSword = false;
    private float pullSpeed = 0.005f; // 예시로 기본 값을 설정합니다.
    private Rigidbody2D playerRigidbody;
    private Camera mainCamera;
    private bool isZoomedIn = false;
    private Vector3 originalCameraPosition = new Vector3(0, 0, -10); // 카메라의 원래 위치
    private bool isZooming = false; // 줌 상태를 추적하는 변수
    private GameObject currentPlayer;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 Sword에 닿았을 때만 검사합니다.
        {
            isTouchingSword = true;
            currentPlayer = other.gameObject; // 접촉한 Player 오브젝트를 저장합니다.
            playerRigidbody = currentPlayer.GetComponent<Rigidbody2D>(); // 해당 Player의 Rigidbody2D를 가져옵니다.
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject == currentPlayer) // 플레이어가 Sword에서 벗어났을 때만 검사합니다.
        {
            isTouchingSword = false;
            currentPlayer = null;
            playerRigidbody = null;
            if (isZoomedIn)
            {
                StartCoroutine(ZoomCamera(zoomOutSize, originalCameraPosition, zoomSpeed)); // 줌아웃 시 카메라 원래 위치로 이동
                isZoomedIn = false;
            }
        }
    }

    void Update()
    {
        if (isTouchingSword && currentPlayer != null && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad5)) && !isZooming) // 플레이어가 Sword와 닿아 있고, 아래 방향키 또는 키패드 5키를 눌렀을 때
        {
            Debug.Log("Now you can pull out sword!!");
            transform.Translate(Vector2.up * pullSpeed);

            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            if (!isZoomedIn)
            {
                Vector3 targetPosition = currentPlayer.transform.position + zoomInOffset;
                StartCoroutine(ZoomCamera(zoomInSize, targetPosition, zoomSpeed));
                isZoomedIn = true;
            }
        }

        if (isZoomedIn && currentPlayer != null && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8)) && !isZooming) // 플레이어가 Sword와 닿아 있는 상태에서 위 방향키 또는 키패드 8키를 눌렀을 때
        {
            Debug.Log("Zoom out and free the player!");
            StartCoroutine(ZoomCamera(zoomOutSize, originalCameraPosition, zoomSpeed)); // 원래 위치로 카메라 이동
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 고정만 유지하고 나머지 속성 해제
            isZoomedIn = false;
        }
    }

    IEnumerator ZoomCamera(float targetSize, Vector3 targetPosition, float speed)
    {
        isZooming = true;
        while (Mathf.Abs(mainCamera.orthographicSize - targetSize) > 0.01f || Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, speed);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, speed);
            yield return null;
        }
        mainCamera.orthographicSize = targetSize;
        mainCamera.transform.position = targetPosition;
        isZooming = false;
    }
}
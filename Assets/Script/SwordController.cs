using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public float zoomInSize = 3f;
    public float zoomOutSize = 5f;
    public float zoomSpeed = 0.1f;
    public Vector3 zoomInOffset = new Vector3(0, 0, -10); // ���� �� ī�޶� ��ġ ������

    private bool isTouchingSword = false;
    private float pullSpeed = 0.005f; // ���÷� �⺻ ���� �����մϴ�.
    private Rigidbody2D playerRigidbody;
    private Camera mainCamera;
    private bool isZoomedIn = false;
    private Vector3 originalCameraPosition = new Vector3(0, 0, -10); // ī�޶��� ���� ��ġ
    private bool isZooming = false; // �� ���¸� �����ϴ� ����
    private GameObject currentPlayer;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // �÷��̾ Sword�� ����� ���� �˻��մϴ�.
        {
            isTouchingSword = true;
            currentPlayer = other.gameObject; // ������ Player ������Ʈ�� �����մϴ�.
            playerRigidbody = currentPlayer.GetComponent<Rigidbody2D>(); // �ش� Player�� Rigidbody2D�� �����ɴϴ�.
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject == currentPlayer) // �÷��̾ Sword���� ����� ���� �˻��մϴ�.
        {
            isTouchingSword = false;
            currentPlayer = null;
            playerRigidbody = null;
            if (isZoomedIn)
            {
                StartCoroutine(ZoomCamera(zoomOutSize, originalCameraPosition, zoomSpeed)); // �ܾƿ� �� ī�޶� ���� ��ġ�� �̵�
                isZoomedIn = false;
            }
        }
    }

    void Update()
    {
        if (isTouchingSword && currentPlayer != null && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad5)) && !isZooming) // �÷��̾ Sword�� ��� �ְ�, �Ʒ� ����Ű �Ǵ� Ű�е� 5Ű�� ������ ��
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

        if (isZoomedIn && currentPlayer != null && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8)) && !isZooming) // �÷��̾ Sword�� ��� �ִ� ���¿��� �� ����Ű �Ǵ� Ű�е� 8Ű�� ������ ��
        {
            Debug.Log("Zoom out and free the player!");
            StartCoroutine(ZoomCamera(zoomOutSize, originalCameraPosition, zoomSpeed)); // ���� ��ġ�� ī�޶� �̵�
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation; // ȸ�� ������ �����ϰ� ������ �Ӽ� ����
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
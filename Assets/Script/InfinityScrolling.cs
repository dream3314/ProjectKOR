using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScrolling : MonoBehaviour
{
    public GameObject player; // Player ������Ʈ�� ������ ����
    public GameObject objectToSpawn; // ������ ������Ʈ

    private Camera mainCamera;
    private float leftBound;
    private float rightBound;

    void Start()
    {
        mainCamera = Camera.main;

        // ī�޶��� ���ʰ� ������ ���� ��ǥ�� ���
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        rightBound = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
    }

    void Update()
    {
        // �÷��̾��� ��ġ�� ������
        float playerX = player.transform.position.x;

        // �÷��̾ ȭ�� ���� ������ �̵��� ��
        if (playerX < leftBound)
        {
            // ���ο� ������Ʈ�� ȭ�� �����ʿ� ����
            Vector3 spawnPosition = new Vector3(rightBound, player.transform.position.y, player.transform.position.z);
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
         //�÷��̾ ȭ�� ������ ������ �̵��� �� (�ݴ� ���⵵ ���� ������� ó�� ����)
         else if (playerX > rightBound)
         {
             Vector3 spawnPosition = new Vector3(leftBound, player.transform.position.y, player.transform.position.z);
             Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
         }
    }
}
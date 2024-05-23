using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScrolling : MonoBehaviour
{
    public GameObject player; // Player 오브젝트를 참조할 변수
    public GameObject objectToSpawn; // 생성할 오브젝트

    private Camera mainCamera;
    private float leftBound;
    private float rightBound;

    void Start()
    {
        mainCamera = Camera.main;

        // 카메라의 왼쪽과 오른쪽 끝의 좌표를 계산
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        rightBound = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
    }

    void Update()
    {
        // 플레이어의 위치를 가져옴
        float playerX = player.transform.position.x;

        // 플레이어가 화면 왼쪽 끝으로 이동할 때
        if (playerX < leftBound)
        {
            // 새로운 오브젝트를 화면 오른쪽에 생성
            Vector3 spawnPosition = new Vector3(rightBound, player.transform.position.y, player.transform.position.z);
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
         //플레이어가 화면 오른쪽 끝으로 이동할 때 (반대 방향도 같은 방식으로 처리 가능)
         else if (playerX > rightBound)
         {
             Vector3 spawnPosition = new Vector3(leftBound, player.transform.position.y, player.transform.position.z);
             Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
         }
    }
}
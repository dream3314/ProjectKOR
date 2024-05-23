using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerWeek"))
        {
            Debug.Log("Stomp!!");

            // 충돌한 객체의 PlayerController2 스크립트를 찾아 StunPlayer 메서드 호출
            PlayerController2 otherPlayer = collision.GetComponentInParent<PlayerController2>();
            if (otherPlayer != null)
            {
                otherPlayer.StunPlayer(1f); // 1초 동안 스턴 상태로 설정
            }
        }
    }
}
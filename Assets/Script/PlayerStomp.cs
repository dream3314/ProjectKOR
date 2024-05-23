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

            // �浹�� ��ü�� PlayerController2 ��ũ��Ʈ�� ã�� StunPlayer �޼��� ȣ��
            PlayerController2 otherPlayer = collision.GetComponentInParent<PlayerController2>();
            if (otherPlayer != null)
            {
                otherPlayer.StunPlayer(1f); // 1�� ���� ���� ���·� ����
            }
        }
    }
}
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMain : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button joinButton;

    void Start()
    {
        joinButton.onClick.AddListener(() =>
        {
            Connet();
        });

        //������ ���� ����
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); //������ ���� ���� �õ�
        joinButton.interactable = false;  //��ư�� ���ͷ����� ��Ȱ��ȭ


        UpdateStatusText("������ ������ ������...");
    }

    public override void OnConnectedToMaster()
    {
        //������ ������ �����ϸ� ȣ��Ǵ� �ݹ�
        joinButton.interactable = true; //��ư�� ������ �ְ� ������ְ�


        UpdateStatusText("�¶��� : ������ ������ �����.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //�����ͼ����� ���� ����, ������ ������ ���� �Ǿ��ִ� ���¿��� ������ ���涧 ȣ�� �Ǵ� �ݹ�
        UpdateStatusText("�������� : ������ ������ ���� ���� ����\n���� ��õ� ��...");

    }

    private void UpdateStatusText(string message)
    {
        statusText.text = message;
    }

    public void Connet()
    {
        //JOIN��ư�� ������ ȣ��Ǵ� �Լ�

        joinButton.interactable = false; //�� ������ ����
        //������ ���� �������̶�� ������ ����(���� ���ٸ� ���� ����� ����)
        // ������ ���� �������ΰ�?


        if (PhotonNetwork.IsConnected)
        {
            //��
            UpdateStatusText("�뿡 ���� ���Դϴ�...");
            //������ ���� (���� ���ٸ� �� ������ ����)
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {

            // �ƴϿ�
            ReconnectToMasterServer();
            //�����ͼ����� ����
        }

    }

    private void ReconnectToMasterServer()
    {
        UpdateStatusText("�������� : ������ ������ ������ ��������\n���� ��õ� ��...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //(����� ���) ���� �� ������ ������ ��� ȣ��Ǵ� �ݹ�
        UpdateStatusText("����� �����ϴ�.\n���ο� ���� ���� ���Դϴ�.");


        //�� ���� ��û
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }); //null = ���̸�
    }

    public override void OnCreatedRoom()
    {
        //�� ������ ���������� ȣ��Ǵ� �ݹ�
        UpdateStatusText("�� ������ �����߽��ϴ�.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //�� ������ ���������� ȣ��Ǵ� �ݹ�
        UpdateStatusText($"�� ������ �����߽��ϴ�\n{message}");
    }

    public override void OnJoinedRoom()
    {
        //�� ������ �����ϴ� ��� ȣ��Ǵ� �ݹ�
        UpdateStatusText("�濡 �����߽��ϴ�.");
    }
}

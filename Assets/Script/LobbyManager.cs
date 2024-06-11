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

        //마스터 서버 접속
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); //마스터 서버 접속 시도
        joinButton.interactable = false;  //버튼의 인터렉션을 비활성화


        UpdateStatusText("마스터 서버에 접속중...");
    }

    public override void OnConnectedToMaster()
    {
        //마스터 서버에 접속하면 호출되는 콜백
        joinButton.interactable = true; //버튼을 누를수 있게 만들어주고


        UpdateStatusText("온라인 : 마스터 서버와 연결됨.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //마스터서버에 접속 실패, 마스터 서버에 접속 되어있는 상태에서 접속이 끊길때 호출 되는 콜백
        UpdateStatusText("오프라인 : 마스터 서버와 연결 되지 않음\n접속 재시도 중...");

    }

    private void UpdateStatusText(string message)
    {
        statusText.text = message;
    }

    public void Connet()
    {
        //JOIN버튼을 누르면 호출되는 함수

        joinButton.interactable = false; //못 누르게 만듬
        //마스터 서버 접속중이라면 랜덤룸 조인(룸이 없다면 방을 만들고 조인)
        // 마스터 서버 접속중인가?


        if (PhotonNetwork.IsConnected)
        {
            //예
            UpdateStatusText("룸에 입장 중입니다...");
            //랜덤룸 조인 (룸이 없다면 룸 생성후 조인)
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {

            // 아니오
            ReconnectToMasterServer();
            //마스터서버에 접속
        }

    }

    private void ReconnectToMasterServer()
    {
        UpdateStatusText("오프라인 : 마스터 서버와 연결이 되지않음\n접속 재시도 중...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //(빈방이 없어서) 랜덤 룸 참가에 실패한 경우 호출되는 콜백
        UpdateStatusText("빈방이 없습니다.\n새로운 방을 생성 중입니다.");


        //방 생성 요청
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }); //null = 룸이름
    }

    public override void OnCreatedRoom()
    {
        //방 생성에 성공했을때 호출되는 콜백
        UpdateStatusText("방 생성에 성공했습니다.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //방 생성에 실패했을때 호출되는 콜백
        UpdateStatusText($"방 생성에 실패했습니다\n{message}");
    }

    public override void OnJoinedRoom()
    {
        //룸 참가에 성공하는 경우 호출되는 콜백
        UpdateStatusText("방에 입장했습니다.");
    }
}

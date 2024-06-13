using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] public GameObject Player1Prefab;
    [SerializeField] public GameObject Player2Prefab;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CreatePlayer(Player1Prefab.name, PlayerController.PlayerType.Player1);
        }
        else
        {
            CreatePlayer(Player2Prefab.name, PlayerController.PlayerType.Player2);
        }
    }

    private void CreatePlayer(string prefabName, PlayerController.PlayerType playerType)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, new Vector3(0, -3.8f, 0), Quaternion.identity);
        PlayerController playerController = go.GetComponent<PlayerController>();
        playerController.Init(playerType);
    }

}

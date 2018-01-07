using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : Photon.PunBehaviour
{
    public GameObject connectingUI;
    public GameObject connectingText;
    public PhotonLogLevel logLevel = PhotonLogLevel.Informational;
    public byte maxPlayers = 4;

    bool isConnecting = false;

    void Start()
    {
        PhotonNetwork.logLevel = logLevel;
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        connectingUI.SetActive(true);
        connectingText.SetActive(false);
	}
	
	public void Connect()
	{
        connectingUI.SetActive(false);
        connectingText.SetActive(true);
        isConnecting = true;
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("1");
        }
	}

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        connectingUI.SetActive(true);
        connectingText.SetActive(false);
        Debug.LogError("Disconnected");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("game");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers }, null);
    }
}

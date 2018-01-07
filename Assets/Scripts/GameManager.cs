using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;

    void Awake()
	{
        instance = this;
        if (playerPrefab)
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);
            }
        }
        else
        {
            Debug.LogError("no player prefab", this);
        }
	}

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("title");
    }
}

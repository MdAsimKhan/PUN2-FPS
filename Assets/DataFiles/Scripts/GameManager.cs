using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab, exitToLobbyButton;

    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), Quaternion.identity);
        }
    }
    
    public void OnExitToLobbyClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Entering Lobby");
        SceneManager.LoadScene("Lobby");
    }
}

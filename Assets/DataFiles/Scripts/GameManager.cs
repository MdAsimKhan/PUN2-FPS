using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab, exitToLobbyButton;

    readonly PhotonManager photonManager;
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), Quaternion.identity);
        }
    }
    
    public void OnExitToLobbyClick()
    {
        photonManager.OnLeaveRoomClick();
    }
}

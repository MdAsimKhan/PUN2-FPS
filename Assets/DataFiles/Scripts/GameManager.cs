using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public TMP_Text playerName;

    #region UnityMethods
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), Quaternion.identity);
            playerName.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }
    #endregion

    #region UtilityMethods

    #endregion

    #region PunCallbacks
    
    #endregion
}

using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyPlayer : MonoBehaviourPunCallbacks
{
    public Button readyButton, playButton;
    public TMP_Text readyCountText;
    private int readyCount = 0;
    private bool isMasterClient => PhotonNetwork.IsMasterClient;

    void Start()
    {
        // Initialize UI elements
        readyButton.onClick.AddListener(OnReadyButtonClicked);
        playButton.onClick.AddListener(OnPlayButtonClicked);

        // Disable play button for non-master clients
        playButton.interactable = isMasterClient;
    }

    void OnReadyButtonClicked()
    {
        // Send an RPC to all clients to update the ready count
        photonView.RPC("UpdateReadyCount", RpcTarget.AllBuffered, 1);
    }

    void OnPlayButtonClicked()
    {
        if (isMasterClient && readyCount == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void UpdateReadyCount(int increment)
    {
        readyCount += increment;
        readyCountText.text = $"Ready Players: {readyCount}";

        // Enable play button if all clients are ready
        if (isMasterClient && readyCount == PhotonNetwork.PlayerList.Length)
        {
            playButton.interactable = true;
        }
    }

    [PunRPC]
    void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
        Debug.Log("Game Started!");
    }
}

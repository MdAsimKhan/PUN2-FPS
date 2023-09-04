using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    #region Public_Variables
    public TMP_InputField playerName, newRoomName, maxPlayers, roomToJoin;
    public GameObject loginPanel, connectingPanel, lobbyPanel, createRoomPanel, playGamePanel, joinRoomPanel;
    public GameObject playerListContent, playDetails;
    public GameObject playButton;
    #endregion

    #region Private_Variables
    private Dictionary<int, GameObject> playerListEntries;

    #endregion

    #region UnityMethods
    void Start()
    {
        ActivatePanel("LoginPanel");
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Update()
    {
        Debug.Log("Network State: " + PhotonNetwork.NetworkClientState);
    }
    #endregion

    #region UIMethods
    public void OnLoginClick()
    {
        string name = playerName.text;

        if(!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            ActivatePanel(connectingPanel.name);
        }
        else
        {
            Debug.Log("Player name is invalid");
        }
    }

    public void OnLogoutClick()
    {
        PhotonNetwork.Disconnect();
        ActivatePanel(loginPanel.name);
    }

    public void OnLeaveRoomClick()
    {
        PhotonNetwork.LeaveRoom();
        ActivatePanel(lobbyPanel.name);
    }
    
    public void OnCreateRoomClick()
    {
        string room = newRoomName.text;

        if (!string.IsNullOrEmpty(room))
        {
            RoomOptions roomOptions = new()
            {
                MaxPlayers = byte.Parse(maxPlayers.text),
                IsVisible = true,  // Make sure the room is visible
                IsOpen = true      // Make sure the room is open
            };
            PhotonNetwork.CreateRoom(room, roomOptions);
        }
        else
        {
            Debug.LogError("Setting a random room name...");
            RoomOptions roomOptions = new RoomOptions
            {
                IsVisible = true,  // Make sure the room is visible
                IsOpen = true      // Make sure the room is open
            };
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }

    public void OnJoinRoomClick()
    {
        string room = roomToJoin.text;

        if(!string.IsNullOrEmpty(room))
        {
            PhotonNetwork.JoinRoom(room);
        }
        else
        {
            Debug.Log("Room name is invalid");
        }
    }

    public void OnPlayClick()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    #endregion

    #region Utility Methods
    public void ActivatePanel(string panelName)
    {
        loginPanel.SetActive(panelName.Equals(loginPanel.name));
        connectingPanel.SetActive(panelName.Equals(connectingPanel.name));
        lobbyPanel.SetActive(panelName.Equals(lobbyPanel.name));
        playGamePanel.SetActive(panelName.Equals(playGamePanel.name));
        createRoomPanel.SetActive(panelName.Equals(createRoomPanel.name));
        joinRoomPanel.SetActive(panelName.Equals(joinRoomPanel.name));
    }

    private void AddPlayerToList(Player player)
    {
        if (!playerListEntries.ContainsKey(player.ActorNumber))
        {
            GameObject playerObj = Instantiate(playDetails, playerListContent.transform);
            playerObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = player.NickName;

            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerObj.transform.GetChild(1).gameObject.SetActive(true);
            }

            playerListEntries.Add(player.ActorNumber, playerObj);
        }
    }
    #endregion

    #region Photon_Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to internet");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Photon: " + cause.ToString());
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " connected to Photon!");
        ActivatePanel(lobbyPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room " + PhotonNetwork.CurrentRoom.Name + " created successfully!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined room " + PhotonNetwork.CurrentRoom.Name + " successfully!");
        ActivatePanel(playGamePanel.name);

        if(PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }

        playerListEntries ??= new Dictionary<int, GameObject>();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            AddPlayerToList(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerToList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        if(PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
    }

    public override void OnLeftRoom()
    {
        foreach(GameObject player in playerListEntries.Values)
        {
            Destroy(player.gameObject);
        }
        playerListEntries.Clear();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
        switch (returnCode)
        {
            case ErrorCode.GameDoesNotExist:
                Debug.LogError("The room does not exist. Please make sure you have the correct room name.");
                break;
            case ErrorCode.GameFull:
                Debug.LogError("The room is full.");
                break;
            case ErrorCode.GameClosed:
                Debug.LogError("The room is closed.");
                break;
            default:
                Debug.LogError("An unknown error occurred. Error code: " + returnCode);
                break;
        }
        // Re-activate the Lobby Panel to allow the user to try again
        ActivatePanel(lobbyPanel.name);
    }

    #endregion
}

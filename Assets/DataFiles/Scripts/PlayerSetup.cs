using UnityEngine;
using Photon.Pun;
using SUPERCharacter;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] localPlayerItems, remotePlayerItems;
    public GameObject playerCamera;
    public Button fireButton;
    private Shooting shooting;
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            shooting.Fire();
        }
    }
    
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            shooting = GetComponent<Shooting>();
            fireButton.onClick.AddListener(shooting.Fire);
            
            // local player
            if(photonView.IsMine)
            {
                foreach(GameObject g in localPlayerItems)
                {
                    g.SetActive(true);
                }

                foreach(GameObject g in remotePlayerItems)
                {
                    g.SetActive(false);
                }

                GetComponent<SUPERCharacterAIO>().cameraPerspective = PerspectiveModes._1stPerson;
                playerCamera.SetActive(true);
            }
            
            // remote player
            else
            {
                foreach(GameObject g in remotePlayerItems)
                {
                    g.SetActive(true);
                }

                foreach(GameObject g in localPlayerItems)
                {
                    g.SetActive(false);
                }
                
                GetComponent<SUPERCharacterAIO>().enabled = false;
                playerCamera.SetActive(false);
            }
        }
    }
}

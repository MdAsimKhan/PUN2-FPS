using UnityEngine;
using Photon.Pun;
using SUPERCharacter;
public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] localPlayerItems, remotePlayerItems;
    public GameObject playerCamera;
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
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

    void Update()
    {
        
    }
}

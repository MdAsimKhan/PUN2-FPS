using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using FirstPersonMobileTools.DynamicFirstPerson;
// using SUPERCharacter;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] localPlayerItems, remotePlayerItems;
    public GameObject playerCamera;
    public Button fireButton;
    private Shooting shooting;
    private MovementController movementController;
    private CameraLook cameraLookScript;
        
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            shooting = GetComponent<Shooting>();
            movementController = GetComponent<MovementController>();
            cameraLookScript = GetComponent<CameraLook>();
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

                // GetComponent<SUPERCharacterAIO>().cameraPerspective = PerspectiveModes._1stPerson;
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
                
                movementController.enabled = false;
                cameraLookScript.enabled = false;
                // GetComponent<SUPERCharacterAIO>().enabled = false;
                playerCamera.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            shooting.Fire();
        }
    }
}

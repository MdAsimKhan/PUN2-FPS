using UnityEngine;
using Photon.Pun;
using SUPERCharacter;
using FirstPersonMobileTools.DynamicFirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] localPlayerItems, remotePlayerItems;
    public GameObject playerCamera;
    public MovementController movementController;
    public CameraLook cameraLook;
    public Animator animator;
    // public AnimationController animationController;
    public SuperAnimator superAnimator;
    
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            animator = GetComponent<Animator>();
            superAnimator = GetComponent<SuperAnimator>();
            
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
                animator.SetBool("isRemote", false);
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
                animator.SetBool("isRemote", true);
                // movementController.enabled = false;
                // cameraLook.enabled = false;
                // animationController.enabled = false;
                superAnimator.enabled = false;
                playerCamera.SetActive(false);
            }
        }
    }
}

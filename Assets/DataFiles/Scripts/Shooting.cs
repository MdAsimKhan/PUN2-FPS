using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.Collections;
using Photon.Realtime;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera FPS_Camera;
    public GameObject bloodEffect;
    public TMP_Text playerCount, killCount, killMsg;
    
    [Header("Health")]
    public float maxHealth = 100f;
    public float curHealth;
    public Image healthBar;
    private int kills;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Fire();
        }
    }

    private void Start()
    {
        playerCount = GameObject.Find("PlayerCountText").GetComponent<TMPro.TMP_Text>();
        killCount = GameObject.Find("KillCountText").GetComponent<TMPro.TMP_Text>();
        killMsg = GameObject.Find("KillMsgText").GetComponent<TMPro.TMP_Text>();
        
        playerCount.text = "Player Left: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        if(photonView.IsMine)
        {
            kills = 0;
        }

        curHealth = maxHealth;
        healthBar.fillAmount = curHealth / maxHealth;
    }

    public void Fire()
    {
        if (Physics.Raycast(FPS_Camera.transform.position, FPS_Camera.transform.forward, out RaycastHit hit))
        {
            //blood effect showing on every player even if not hit
            // Player targetPlayer = hit.transform.GetComponent<PhotonView>().Owner;
            photonView.RPC("CreateEffect", RpcTarget.Others, hit.point);

            if(hit.transform.tag == "Player" && !hit.transform.GetComponent<PhotonView>().IsMine)
            {
                hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
            }
        }
    }

    private IEnumerator HideKillMessage()
    {
        yield return new WaitForSeconds(2f);
        killMsg.text = "";
    }

    [PunRPC]
    public void UpdatePlayerUI(string msg, int cnt)
    {
        Debug.Log("Players Left: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        playerCount.text = "Players Left: " + cnt;
        killCount.text = "Kills: " + kills.ToString();
        killMsg.text = msg;
        StartCoroutine(HideKillMessage());
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo messageInfo)
    {
        string msg;
        int cnt;
        curHealth -= damage;
        healthBar.fillAmount = curHealth / maxHealth;

        if(curHealth <= 0)
        {
            /// <summary>
            /// This photonView is of the player who's hit
            /// </summary>

            if (photonView.IsMine)
            {
                // PhotonNetwork.LeaveRoom();
                PhotonNetwork.DestroyPlayerObjects(photonView.Owner);
            }
            else
            {
                kills++;
            }

            msg = messageInfo.Sender.NickName + " killed " + messageInfo.photonView.Owner.NickName;
            Debug.Log(msg);
            cnt = PhotonNetwork.CurrentRoom.PlayerCount - 1;
            photonView.RPC("UpdatePlayerUI", RpcTarget.AllBuffered, msg, cnt);
        }
    }

    [PunRPC]
    public void CreateEffect(Vector3 position)
    {
        GameObject blood = Instantiate(bloodEffect, position, Quaternion.identity);
        Destroy(blood, 1f);
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        PhotonNetwork.DestroyPlayerObjects(player);
    }

}

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
    public TMP_Text playerCount, killCount, killMsg, debugMessage;
    
    [Header("Health")]
    public float maxHealth = 100f;
    public float curHealth;
    public Image healthBar;

#region UnityMethods
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
        debugMessage = GameObject.Find("DebugLogs").GetComponent<TMPro.TMP_Text>();
        
        playerCount.text = "Player Left: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();

        curHealth = maxHealth;
        healthBar.fillAmount = curHealth / maxHealth;
    }
#endregion

#region UtilityMethods
    public void Fire()
    {
        if (Physics.Raycast(FPS_Camera.transform.position, FPS_Camera.transform.forward, out RaycastHit hit))
        {
            //blood effect showing on every player even if not hit
            // Player targetPlayer = hit.transform.GetComponent<PhotonView>().Owner;
            photonView.RPC("CreateEffect", RpcTarget.Others, hit.point);

            if(hit.transform.tag == "Player" && !hit.transform.GetComponent<PhotonView>().IsMine)
            {
                // the player who is hit sends RPC to every other player to update its state as seen by them in their game
                hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f, photonView.Owner.ActorNumber);
            }
        }
    }

    // not working
    private IEnumerator HideKillMessage()
    {
        yield return new WaitForSeconds(2f);
        killMsg.text = "";
    }

    private void Die()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.DestroyPlayerObjects(photonView.Owner);
            PhotonNetwork.LeaveRoom();
        }
    }

    /// <summary>
    /// Kill count using CustomProps
    /// </summary>
    public void AddKill()
    {
        debugMessage.text += "Add Kill called\n";

        if (photonView.IsMine) return;

        debugMessage.text += "Add Kill is mine\n";

        int currentKills = 0;
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("kills"))
        {
            currentKills = (int)PhotonNetwork.LocalPlayer.CustomProperties["kills"];
        }

        currentKills++;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "kills", currentKills } });
        debugMessage.text += "Player props set\n";
        debugMessage.text += "Kill count updated to " + killCount.text + " kills\n";
    }
#endregion

#region PunRPCs
    [PunRPC]
    public void UpdatePlayerUI(string msg, int cnt)
    {
        playerCount.text = "Players Left: " + cnt;
        killMsg.text = msg;
        StartCoroutine(HideKillMessage());
    }

    [PunRPC]
    public void TakeDamage(float damage, int attackerID, PhotonMessageInfo messageInfo)
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

            msg = messageInfo.Sender.NickName + " killed " + messageInfo.photonView.Owner.NickName;
            debugMessage.text += msg + "\n";
            cnt = PhotonNetwork.CurrentRoom.PlayerCount - 1;
            photonView.RPC("UpdatePlayerUI", RpcTarget.AllBuffered, msg, cnt);

            if(PhotonNetwork.LocalPlayer.ActorNumber == attackerID)
            {
                AddKill();
            }

            Die();
        }
    }

    [PunRPC]
    public void CreateEffect(Vector3 position)
    {
        GameObject blood = Instantiate(bloodEffect, position, Quaternion.identity);
        Destroy(blood, 1f);
    }
#endregion

#region PunCallbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("kills"))
        {
            // debugMessage.text += "New kill count: " + changedProps["kills"] + "\n";
            killCount.text = "Kills: " + changedProps["kills"].ToString();
        }
    }
#endregion
}

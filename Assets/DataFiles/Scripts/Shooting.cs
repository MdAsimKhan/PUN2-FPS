using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera FPS_Camera;
    public GameObject bloodEffect;
    
    [Header("Health")]
    public float maxHealth = 100f;
    public float curHealth;
    public Image healthBar;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Fire();
        }
    }

    private void Start()
    {
        curHealth = maxHealth;
        healthBar.fillAmount = curHealth / maxHealth;
    }
    public void Fire()
    {
        if (Physics.Raycast(FPS_Camera.transform.position, FPS_Camera.transform.forward, out RaycastHit hit))
        {
            Debug.Log(hit.transform.name);
            photonView.RPC("CreateEffect", RpcTarget.All, hit.point);

            if(hit.transform.tag == "Player" && !hit.transform.GetComponent<PhotonView>().IsMine)
            {
                hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo messageInfo)
    {
        curHealth -= damage;
        healthBar.fillAmount = curHealth / maxHealth;
        if(curHealth <= 0)
        {
            Debug.Log(messageInfo.Sender.NickName + " killed " + messageInfo.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateEffect(Vector3 position)
    {
        GameObject blood = Instantiate(bloodEffect, position, Quaternion.identity);
        Destroy(blood, 1f);
    }
}

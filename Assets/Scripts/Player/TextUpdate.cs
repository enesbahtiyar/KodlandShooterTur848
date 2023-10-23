using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TextUpdate : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] TMP_Text playerNickname;
    int health = 100;


    private void Start()
    {
        if(photonView.IsMine)
        {
            playerNickname.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
            photonView.RPC("RotateName", RpcTarget.Others);
        }
    }

    public void setHealth(int newHealth)
    {
        //can değeri artık yeni can
        health = newHealth;
        //ui yazısını güncelleyecez
        //photonda oyuncununu isminin altında olacak can
        playerNickname.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString(); 
    }

    [PunRPC]
    public void RotateName()
    {
        playerNickname.GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            playerNickname.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
        }
    }
}

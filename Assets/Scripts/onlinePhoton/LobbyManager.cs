using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Photon.Realtime;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text chatText;
    [SerializeField] TMP_Text inputText;
    [SerializeField] GameObject startButton;

    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(false);
        }

        if(PlayerPrefs.HasKey("Winner") && PhotonNetwork.IsMasterClient)
        {
            //oyuncunun ismini "Winner" key stringine kaydetmiştik şimdi çekiyoruz
            string winner = PlayerPrefs.GetString("Winner");
            //son maçı kazanan oyuncu şu oyuncu diye yazdıracaz
            photonView.RPC("ShowMessage", RpcTarget.All, "The winner of the last game is " + winner);
            //kaydettiğimiz datayı temizle
            PlayerPrefs.DeleteAll();
        }
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    void Log(string message)
    {
        chatText.text += "\n";
        chatText.text += message;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //oyuncunun odadan ayrıldığını chate yazdırdık
        Log(otherPlayer.NickName + " left the room");
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //hangi oyuncu odaya geldiyse nickname'ini yazdır
        Log(newPlayer.NickName + " joined the room");
    }


    [PunRPC]
    public void ShowMessage(string message)
    {
        chatText.text += "\n";
        chatText.text += message;
    }

    public void Send()
    {
        //input field boşluksa ya da null'sa geri dön || hiçbir şey yapma
        if (string.IsNullOrWhiteSpace(inputText.text)) 
            return;

        //eğer oyuncu enter'a tıklarsa
        if(Input.GetKeyDown(KeyCode.Return))
        {
            //              showmessage   odadaki herkes   Nicknameimizi al yazdığımız mesajımızı al 
            //              methodunu                      chate yazdır
            //              çağır
            photonView.RPC("ShowMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + inputText.text);

            //yeni mesaj girilebilmesi için input kısmımızı boşalttık
            inputText.text = "";
        }

    }



}

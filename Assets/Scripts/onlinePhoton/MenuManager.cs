using UnityEngine;
using TMPro;
using Photon.Pun;


public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text logText;

    void Log(string message)
    {
        logText.text += "\n";
        logText.text += message;
    }

    private void Start()
    {
        //oyuncunun ismini otomatik oluştur
        PhotonNetwork.NickName = "Player" + Random.Range(1, 9999);
        //oyuncunun ismini loga yazdır
        Log("Player Name: " + PhotonNetwork.NickName);
        //oyunun ayarlarını yap
        PhotonNetwork.AutomaticallySyncScene = true;        //pencereler arasında otomatik geçiş
        //oyunun versiyonu
        PhotonNetwork.GameVersion = "1";
        //photon sunucusuna bağlan
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 15 });
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnConnectedToMaster()
    {
        Log("Connected to the server");
    }
    public override void OnJoinedRoom()
    {
        Log("Joined the lobby");
        PhotonNetwork.LoadLevel("Lobby");
    }



}

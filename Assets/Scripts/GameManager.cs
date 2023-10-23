using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Transform> spawns = new List<Transform>();
    [SerializeField] List<Transform> bugSpawns = new List<Transform>();
    [SerializeField] List<Transform> turretSpawn = new List<Transform>();
    //Yazı elemanına bir referans veriyoruz
    [SerializeField] public TMP_Text playersText;
    //oyuncuları tutacak bir array
    GameObject[] players;
    //aktif oyuncuları tutacak olan Liste
    List<string> activePlayers = new List<string>();
    int checkPlayers = 0;
    private int previousPlayerCount;
    [SerializeField] GameObject exitGameButton;
    int randomSpawn;

    private void Start()
    {
        randomSpawn = Random.Range(0, spawns.Count);
        PhotonNetwork.Instantiate("Player", spawns[randomSpawn].position, spawns[randomSpawn].rotation);
        Invoke("SpawnEnemy", 5f);
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }

    private void Update()
    {
        if(PhotonNetwork.PlayerList.Length < previousPlayerCount)
        {
            ChangePlayersList();
        }
        previousPlayerCount = PhotonNetwork.PlayerList.Length;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitGameButton.active)
            {
                exitGameButton.SetActive(false);
            }
            exitGameButton.SetActive(true);
        }
    }

    public void SpawnEnemy()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for(int i = 0; i < bugSpawns.Count; i++)
            {
                PhotonNetwork.Instantiate("Bug", bugSpawns[i].position, bugSpawns[i].rotation);
            }
            for (int i = 0; i < turretSpawn.Count; i++)
            {
                PhotonNetwork.Instantiate("Turret", turretSpawn[i].position, turretSpawn[i].rotation);
            }
        }
    }
    public void ChangePlayersList()
    {
        photonView.RPC("PlayerList", RpcTarget.All);
    }


    [PunRPC]
    public void PlayerList()
    {
        //Player tagine sahip olan herkesi çek 
        players = GameObject.FindGameObjectsWithTag("Player");
        //aktif oyuncular listesini temizle
        activePlayers.Clear();
        foreach(GameObject player in players)
        {
            //eğer oyuncu hayattaysa
            if(player.GetComponent<PlayerController>().isDead == false)
            {
                activePlayers.Add(player.GetComponent<PhotonView>().Owner.NickName);
            }
        }

        playersText.text = "Players in game" + activePlayers.Count.ToString();


        //eğer sadece 1 oyuncu kaldıysa
        if(activePlayers.Count <= 1 && checkPlayers > 0)
        {
            PlayerPrefs.SetString("Winner", activePlayers[0]);
            //oyundaki bütün düşmanları öldürmek için bir array oluştur
            var enemies = GameObject.FindGameObjectsWithTag("enemy");
            foreach(GameObject enemy in enemies)
            {
                //oyuncu sayısı 1 kaldıysa bütün enemy tagine sahip objeleri çek ve hepsine 100 damage at 
                enemy.GetComponent<Enemy>().ChangeHealth(100);
            }
            Invoke("EndGame", 5f);
        }

        checkPlayers++;
    }

    void EndGame()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //ana menüye dön
        SceneManager.LoadScene(0);
        //oyuncu listesini tekrar güncelle
        ChangePlayersList();
    }
}

using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks
{
    //can
    [SerializeField] protected int health;
    //can resmini tutalım 
    [SerializeField] Image healthBar;
    //atak mesafesi
    [SerializeField] protected float attackDistance;
    //hasar
    [SerializeField] protected int damage;
    //bekleme süresi
    [SerializeField] protected float coolDown;
    //oyuncuları tutacak bir liste array
    protected GameObject[] players;
    //burada oyuncu karakteri tutacaz
    protected GameObject player;
    //düşmanların animasyonu
    protected Animator anim;
    //düşmanların fiziği
    protected Rigidbody rb;
    //oyuncuya mesafe
    protected float distance;
    //geri sayım
    protected float timer;
    //ölülük durumu
    bool isDead = false;


    //override edeceğimiz methodlar
    public virtual void Move()
    {

    }
    public virtual void Attack()
    {

    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        CheckPlayers();
    }

    void CheckPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Invoke("CheckPlayers", 3f);
    }

    private void Update() 
    {
        //enemynin oyunculara olan uzaklığını tutacak bir değişken
        float closestDistance = Mathf.Infinity;
        // x = sonsuz

        //players arrayinin içerisindeki bütün player gameobjectlerini kontrol et
        foreach(GameObject closestPlayer in  players) 
        {
            //düşman ve oyuncu arasındaki uzaklığı hesapla
            float checkDistance = Vector3.Distance(closestPlayer.transform.position, transform.position);
            //eğer oyuncuya olan mesafe bir önceki mesafeden daha kısaysa closesDistance değişlenini değiştir
            if(checkDistance < closestDistance)
            {
                //yakın olarak bulunan oyuncu hayatta mı diye kontrol et
                if(closestPlayer.GetComponent<PlayerController>().isDead == false)
                {
                    //saldırılacak oyuncuyu yakın olan oyuncu 
                    player = closestPlayer;
                    //en yakın mesafeyi ayarla
                    closestDistance = checkDistance;
                }
            }
        }
        //güvenlik önlemi olarak oyuncu gerçekten var mı diye kontrol ediyoruz
        if(player != null)
        {
            distance = Vector3.Distance(transform.position, player.transform.position);
            if(!isDead && !player.GetComponent<PlayerController>().isDead)
            {
                Attack();
            }
        }
    }
    

    private void FixedUpdate() 
    {
        if(!isDead && player != null)
        {
            Move();
        }
    }

    [PunRPC]
    public void ChangeHealth(int damage)
    {
        health -= damage;
        float fillPercent = health / 100f;
        healthBar.fillAmount = fillPercent;
        if(health <= 0)
        {
            isDead = true;
            //düşmanını colliderını kapat
            GetComponent<Collider>().enabled = false;
            anim.enabled = true;
            //animasyon
            anim.SetBool("Die", true);
        }
    }

    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }
}

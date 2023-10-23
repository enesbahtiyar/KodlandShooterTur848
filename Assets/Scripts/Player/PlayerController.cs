using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    public enum EnumWeapons
    {
        None,
        Pistol,
        MiniGun,
        Rifle,
        Count
    }

    EnumWeapons enumWeapons = EnumWeapons.None;

    [SerializeField] float runSpeed = 7f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float stamina = 5f;
    [SerializeField] Image pistolUI, rifleUI, miniGunUI, cursor;
    [SerializeField] GameObject pistol, miniGun, rifle;
    //ses kaynağımızı tutmak için
    [SerializeField] AudioSource characterSounds;
    //bu ses klibimizi tutmak için
    [SerializeField] AudioClip jumpSound;
    float currentSpeed;
    Rigidbody rb;
    Animator anim;
    Vector3 direction;
    int health;
    TextUpdate textUpdate;

    bool isGrounded;
    public bool isDead;
    bool isPistol, isRifle, isMiniGun;
    GameManager gameManager;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            //kamerayı bul ve devre dışı bırak
            transform.Find("Main Camera").gameObject.SetActive(false);
            //canvas yani ui devre dışı bırakcaz
            transform.Find("Canvas").gameObject.SetActive(false);
            //player controller scriptini iptal et
            this.enabled = false;
        }

        health = 100;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        textUpdate = GetComponent<TextUpdate>();
        //hiyerarşiyi gez ve GameManager bulunun gameobjecti çek 
        gameManager = FindObjectOfType<GameManager>();
        currentSpeed = movementSpeed;
        gameManager.ChangePlayersList();
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        direction = new Vector3(moveHorizontal, 0f, moveVertical);
        direction = transform.TransformDirection(direction);

        if (direction.x != 0 || direction.z != 0)
        {
            anim.SetBool("Run", true);
            if (!characterSounds.isPlaying && isGrounded)
            {
                characterSounds.Play();
            }
        }

        if (direction.x == 0 && direction.z == 0)
        {
            anim.SetBool("Run", false);
            characterSounds.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isGrounded = false;
            anim.SetBool("Jump", true);
            characterSounds.Stop();
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        }
        if (!isGrounded)
        {
            anim.SetBool("Jump", true);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina > 0)
            {
                currentSpeed = runSpeed;
                stamina -= Time.deltaTime;
            }
            else
            {
                currentSpeed = movementSpeed;
            }
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            stamina += Time.deltaTime;
            currentSpeed = movementSpeed;
        }

        if (stamina > 5)
        {
            stamina = 5f;
        }
        else if (stamina < 0)
        {
            stamina = 0f;
        }
        Debug.Log(stamina);

        if (Input.GetKeyDown(KeyCode.Alpha1) && isPistol)
        {
            //ChooseWeapon(EnumWeapons.Pistol);
            photonView.RPC("ChooseWeapon", RpcTarget.All, EnumWeapons.Pistol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isRifle)
        {
            //ChooseWeapon(EnumWeapons.Rifle);
            photonView.RPC("ChooseWeapon", RpcTarget.All, EnumWeapons.Rifle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && isMiniGun)
        {
            //ChooseWeapon(EnumWeapons.MiniGun);
            photonView.RPC("ChooseWeapon", RpcTarget.All, EnumWeapons.MiniGun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //ChooseWeapon(EnumWeapons.None);
            photonView.RPC("ChooseWeapon", RpcTarget.All, EnumWeapons.None);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);
    }

    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }


    [PunRPC]
    public void ChangeHealth(int damage)
    {
        health -= damage;
        textUpdate.setHealth(health);

        if (health <= 0)
        {
            gameManager.ChangePlayersList();
            //ölüm animasyonunu çalıştır
            anim.SetBool("Die", true);
            //silahı kaldır
            ChooseWeapon(EnumWeapons.None);
            //karakterin hareketini engelle
            this.enabled = false;
            isDead = true;        
            transform.Find("Main Camera").GetComponent<ThirdPersonCamera>().isSpectator = true;
        }
    }

    public void ChooseWeapon(string weapons)
    {
        switch (weapons)
        {
            case "Pistol":
                anim.SetBool("Pistol", true);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", false);
                pistol.SetActive(true);
                rifle.SetActive(false);
                miniGun.SetActive(false);
                break;
            case "Rifle":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", true);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", false);
                pistol.SetActive(false);
                rifle.SetActive(true);
                miniGun.SetActive(false);
                break;
            case "MiniGun":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", true);
                anim.SetBool("NoWeapon", false);
                pistol.SetActive(false);
                rifle.SetActive(false);
                miniGun.SetActive(true);
                break;
            case "NoWeapon":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", true);
                pistol.SetActive(false);
                rifle.SetActive(false);
                miniGun.SetActive(false);
                break;

        }
    }

    [PunRPC]
    public void ChooseWeapon(EnumWeapons enumWeapons)
    {
        anim.SetBool("Pistol", enumWeapons == EnumWeapons.Pistol);
        anim.SetBool("Assault", enumWeapons == EnumWeapons.Rifle);
        anim.SetBool("MiniGun", enumWeapons == EnumWeapons.MiniGun);
        anim.SetBool("NoWeapon", enumWeapons == EnumWeapons.None);
        pistol.SetActive(enumWeapons == EnumWeapons.Pistol);
        rifle.SetActive(enumWeapons == EnumWeapons.Rifle);
        miniGun.SetActive(enumWeapons == EnumWeapons.MiniGun);

        if (enumWeapons != EnumWeapons.None)
        {
            cursor.enabled = true;
        }
        else
        {
            cursor.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
        anim.SetBool("Jump", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Pistol":
                if (!isPistol)
                {
                    isPistol = true;
                    pistolUI.color = Color.white;
                    ChooseWeapon(EnumWeapons.Pistol);
                }
                break;
            case "Rifle":
                if (!isRifle)
                {
                    isRifle = true;
                    rifleUI.color = Color.white;
                    ChooseWeapon(EnumWeapons.Rifle);
                }
                break;
            case "MiniGun":
                if (!isMiniGun)
                {
                    isMiniGun = true;
                    miniGunUI.color = Color.white;
                    ChooseWeapon(EnumWeapons.MiniGun);
                }
                break;
            default:
                break;
        }

        Destroy(other.gameObject);
    }
}

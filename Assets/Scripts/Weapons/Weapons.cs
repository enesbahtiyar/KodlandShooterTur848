using UnityEngine;
using TMPro;
using Photon.Pun;

public class Weapons : MonoBehaviourPunCallbacks
{
    [SerializeField] protected GameObject particle;
    [SerializeField] protected GameObject cam;
    [SerializeField] protected TMP_Text ammoText;
    //ateş sesi tutucu
    [SerializeField] AudioSource shootSound;
    //mermi sesi, kuru sıkı sesi, şarjör değiştirme
    [SerializeField] AudioClip bulletSound, noBulletSound, reloadSound;
    protected bool auto = false;
    protected float cooldown = 0f;
    protected int ammoCurrent;
    protected int ammoMax;
    protected int ammoCapacity;
    private float timer = 0;

    private void Start()
    {
        timer = cooldown;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            timer += Time.deltaTime;

            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
            AmmoTextUpdate();

            if (Input.GetKey(KeyCode.R))
            {
                if (ammoCurrent != ammoMax || ammoCapacity != 0)
                {
                    Invoke("Reload", 1);
                    shootSound.PlayOneShot(reloadSound);
                }
            }
        }
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0) || auto)
        {
            if (timer > cooldown)
            {
                if (ammoCurrent > 0)
                {
                    OnShoot();
                    timer = 0;
                    ammoCurrent--;
                    shootSound.PlayOneShot(bulletSound);
                    shootSound.pitch = Random.Range(1, 1.5f);
                }
                else
                {
                    shootSound.PlayOneShot(noBulletSound);
                }
            }
        }
    }

    private void AmmoTextUpdate()
    {
        ammoText.text = ammoCurrent.ToString() + " / " + ammoCapacity.ToString();
    }

    private void Reload()
    {
        int ammoNeed = ammoMax - ammoCurrent;

        if (ammoCapacity >= ammoNeed)
        {
            ammoCapacity -= ammoNeed;
            ammoCurrent += ammoNeed;
        }
        else
        {
            ammoCurrent += ammoCapacity;
            ammoCapacity = 0;
        }
    }

    protected virtual void OnShoot()
    {

    }
}

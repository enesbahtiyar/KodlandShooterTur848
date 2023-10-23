using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapons
{
    private void Start() 
    {
        cooldown = 0;
        auto = false;
        ammoCurrent = 12;
        ammoMax = 12;
        ammoCapacity = 36;
    }


    protected override void OnShoot()
    {
        Vector3 rayStartPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(rayStartPosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            GameObject gameBullet = Instantiate(particle, hit.point, hit.transform.rotation);

            if (hit.collider.CompareTag("enemy"))
            {
                //hit.collider.gameObject.GetComponent<Enemy>().ChangeHealth(10);
                hit.collider.gameObject.GetComponent<Enemy>().GetDamage(10);
            }
            else if(hit.collider.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<PlayerController>().GetDamage(10);
            }


            Destroy(gameBullet, 1f);
        }
    }
}

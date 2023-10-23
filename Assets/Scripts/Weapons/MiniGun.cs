using UnityEngine;

public class MiniGun : Weapons
{
    void Start()
    {
        cooldown = 0.08f;
        auto = true;
        ammoCurrent = 120;
        ammoMax = 120;
        ammoCapacity = 240;
    }

    protected override void OnShoot()
    {
        Vector3 rayStartPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        Vector3 drift = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
        
        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(rayStartPosition + drift);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject gameBullet = Instantiate(particle, hit.point, hit.transform.rotation);

            if(hit.collider.CompareTag("enemy"))
            {
                //hit.collider.gameObject.GetComponent<Enemy>().ChangeHealth(10);
                hit.collider.gameObject.GetComponent<Enemy>().GetDamage(10);
            }
            else if (hit.collider.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<PlayerController>().GetDamage(10);
            }

            Destroy(gameBullet, 1);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugEnemy : Enemy
{
    [SerializeField] float speed;
    [SerializeField] float detectionDistance;

    public override void Move()
    {
        //saldırabileceği pozisyonu bulana kadar hareket et
        if(distance < detectionDistance && distance > attackDistance)
        {
            //oyuncuya bak
            transform.LookAt(player.transform);
            //animasyonunun çalıştır
            anim.SetBool("Run", true);
            //böcek hareket etsin
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }
        else
        {
            //koşma
            anim.SetBool("Run", false);
        }
    }

    public override void Attack()
    {
        timer += Time.deltaTime;

        if(distance < attackDistance && timer > coolDown)
        {
            //vurduğumuz için timer 0 oldu 
            timer = 0;
            //oyuncununu canı azalsın
            player.GetComponent<PlayerController>().ChangeHealth(damage);
            //animasyon yine
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }
    }
}

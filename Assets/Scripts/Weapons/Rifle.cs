using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Pistol
{
    private void Start() 
    {
        cooldown = 0.2f;
        auto = true;
        ammoCurrent = 30;
        ammoMax = 30;
        ammoCapacity = 90;
    }
}

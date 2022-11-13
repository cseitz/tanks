using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeapon_MainGun : TankWeapon
{
    public string weaponName = "mainGun";
    
    public float damage = 60f;
    public float firerate = 10 / 60.0f;
    public float bulletSpeed = 10f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        base._weaponName = weaponName;
    }

    public override void Update()
    {
        base.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public override void Shoot() {
        if (cooldown == 0) {
            cooldown += 1 / firerate;
            print("Shot main gun");
            // Instantiate(GetComponent<TankParticles>().explosionDirt, Vector3.zero + new Vector3(0, 10, 0), Quaternion.identity);
            GetComponent<TankParticles>().Explosion("dirt", new Vector3(3, 0, 14));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeapon_MainGun : TankWeapon
{
    public string weaponName = "mainGun";
    
    public float damage = 60f;
    public float firerate = 10 / 60.0f;
    public float bulletSpeed = 10f;

    public GameObject barrel;

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
            // GetComponent<TankParticles>().Explosion("dirt", new Vector3(3, 0, 14));
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            // state.targetPosition = ray.origin + ray.direction * ((transform.position - ray.origin).magnitude * 2);
            RaycastHit hit;
            if (Raycast(out hit, barrel.transform.position, barrel.transform.forward)) {
            // if (Raycast(out hit, ray.origin, ray.direction)) {
                EntityHealth entityHealth = null;
                if (hit.rigidbody != null) {
                    print("hit " + hit.rigidbody.gameObject.ToString());
                    hit.rigidbody.gameObject.TryGetComponent<EntityHealth>(out entityHealth);
                }
                string type = entityHealth != null ? "Small" : "Ground";
                print("explosion type: " + type);
                ExplosionManager.Spawn(new ExplosionConfig () {
                    type = type,
                    position = hit.point,
                    damage = 60.0f,
                    radius = 2.0f,
                });
            }

            // TODO: calculate target position by deriving it from (camera * root:inverse()) * turretRotation
            // use same math as earlier for calculating turret end position
            // except we calculate where the target would be based on where we want to be and how rotated we are

        }
    }
}

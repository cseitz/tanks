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
        if (barrel != null) {
            Transform steamEmitter = barrel.transform.Find("steam");
            if (steamEmitter != null) {
                ParticleSystem steam = steamEmitter.GetComponent<ParticleSystem>();
                steam.Clear();
                steam.Stop();
            }
        }
    }

    public override void Update()
    {
        base.Update();
    }


    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        base.state.deltaSinceMainGunShoot += Time.fixedDeltaTime;
        float delta = base.state.deltaSinceMainGunShoot;

        if (barrel != null) {
            Transform steamEmitter = barrel.transform.Find("steam");
            if (steamEmitter != null) {
                ParticleSystem steam = steamEmitter.GetComponent<ParticleSystem>();
                var emission = steam.emission;
                emission.enabled = delta <= 5f;
                emission.rateOverTime = 20f * Mathf.Clamp01((2f - delta) / 1f);
                emission.rateOverDistance = emission.rateOverTime;
                if (!steam.isPlaying && emission.enabled) {
                    steam.Play();
                }
            }

            // Transform flashEmitter = barrel.transform.Find("steam");
            // if (flashEmitter != null) {
            //     ParticleSystem flash = flashEmitter.GetComponent<ParticleSystem>();
            //     flash.Emit(1);
            // }
        }
    }

    public override void Shoot() {
        if (cooldown == 0) {
            cooldown += 1 / firerate;
            base.state.deltaSinceMainGunShoot = 0f;
            print("Shot main gun");

            Vector3 up = transform.up;
            Vector3 startPosition = barrel.transform.position;
            
            Ray _targetRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPosition = state.targetPosition = _targetRay.origin + _targetRay.direction * 100f;

            RaycastHit _targetHit;
            if (Raycast(out _targetHit, _targetRay.origin, _targetRay.direction)) {
                targetPosition = _targetHit.point;
            }

            Vector3 barrelDirection = barrel.transform.forward;
            Vector3 targetDirection = (targetPosition - startPosition).normalized;

            // print(barrelDirection + " -> " + targetDirection);


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

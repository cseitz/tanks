using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCombatController : MonoBehaviour
{
    private EntityHealth entityHealth;
    private TankControllerState state;
    private TankConfig config;
    private Rigidbody rb;

    public bool dead = false;

    public float health {
        get { return entityHealth.health; }
        set { 
            entityHealth.health = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        entityHealth = GetComponent<EntityHealth>();
        state = GetComponent<TankControllerState>();
        config = GetComponent<TankConfig>();
        rb = GetComponent<Rigidbody>();

        health = config.maxHealth;
        entityHealth.maxHealth = config.maxHealth;
    }

    bool Ready() {
        if (entityHealth == null) {
            Start();
            return false;
        }
        return true;
    }

    private float lastHealth = 0.0f;
    void FixedUpdate()
    {
        Ready();

        if (health < lastHealth) {
            state.deltaSinceDamage = 0.0f;
        }

        state.deltaSinceDamage += Time.fixedDeltaTime;
        if (state.deltaSinceDamage > config.healthRecoveryDelay && health < config.maxHealth && !dead) {
            health += config.healthRecovery * Time.fixedDeltaTime;
            if (health > config.maxHealth) {
                health = config.maxHealth;
            }
        }

        state.health = health;
        lastHealth = health;

        if (health <= 0) {
            if (!dead) {
                dead = true;
                transform.Find("effects").Find("destroyed").gameObject.SetActive(true);
                ExplosionManager.Spawn(new ExplosionConfig () {
                    type = "Big",
                    position = transform.position,
                    damage = 30.0f,
                    radius = 4.0f,
                });
            }
        }
    }

    public void Shoot(string type) {
        TankWeapon weapon = null;
        if (type == "mainGun") {
            TankWeapon_MainGun mainGun;
            TryGetComponent<TankWeapon_MainGun>(out mainGun);
            weapon = mainGun;
        }

        if (weapon != null) {
            weapon.Shoot();
        }
    }
}

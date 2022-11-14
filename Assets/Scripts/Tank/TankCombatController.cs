using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCombatController : MonoBehaviour
{
    private EntityHealth entityHealth;
    private TankControllerState state;
    private TankConfig config;
    private Rigidbody rb;

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
        if (health < config.maxHealth && state.deltaSinceDamage > config.healthRecoveryDelay) {
            health += config.healthRecovery * Time.fixedDeltaTime;
            if (health > config.maxHealth) {
                health = config.maxHealth;
            }
        }

        state.health = health;
        lastHealth = health;
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

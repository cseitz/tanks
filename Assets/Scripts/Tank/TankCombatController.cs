using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCombatController : MonoBehaviour
{
    private TankControllerState state;
    private TankConfig config;
    private Rigidbody rb;

    public float health {
        get { return state.health; }
        set { state.health = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<TankControllerState>();
        config = GetComponent<TankConfig>();
        rb = GetComponent<Rigidbody>();

        health = config.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        state.deltaSinceDamage += Time.fixedDeltaTime;
        if (state.deltaSinceDamage > config.healthRecoveryDelay) {
            health += config.healthRecovery * Time.fixedDeltaTime;
        }
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health < 0) {
            // TODO: kill
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

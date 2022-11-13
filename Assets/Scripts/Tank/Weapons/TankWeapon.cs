using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankWeapon : MonoBehaviour
{
    public float cooldown = 0f;

    [System.NonSerialized] public TankControllerState state;
    [System.NonSerialized] public TankConfig config;

    [System.NonSerialized] public string _weaponName;

    // Start is called before the first frame update
    public virtual void Start()
    {
        state = GetComponent<TankControllerState>();
        config = GetComponent<TankConfig>();
    }

    public virtual void Update() {
        if (cooldown > 0) {
            cooldown -= Time.deltaTime;
            if (cooldown < 0) {
                cooldown = 0f;
                print("ready to shoot " + _weaponName + " again");
            }
        }
    }

    public abstract void Shoot();

    public virtual void Raycast(Vector3 from, Vector3 to) {
        // TODO: implement raycasting
    }
}

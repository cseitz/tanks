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

    public bool _Ready() {
        if (state == null) {
            Start();
            return false;
        }
        return true;
    }

    public virtual void Update() {
        _Ready();
        if (cooldown > 0) {
            cooldown -= Time.deltaTime;
            if (cooldown < 0) {
                cooldown = 0f;
                print("ready to shoot " + _weaponName + " again");
            }
        }
    }

    public virtual void FixedUpdate() {
        _Ready();
    }

    public abstract void Shoot();

    public virtual bool Raycast(out RaycastHit hit, Vector3 from, Vector3 direction) {
        if (Physics.Raycast(from, direction.normalized, out hit, Mathf.Infinity)) {
            return true;
        }
        return false;
    }
}

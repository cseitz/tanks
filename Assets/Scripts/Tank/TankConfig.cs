using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankConfig : MonoBehaviour
{
    public float acceleration = 10f;
    public float breakingForce = 20f;
    public float maxTurnAngle = 30f;
    public float maxSpeed = 2f;

    public float maxTurretTurnSpeed = 20f;
    public float maxBarrelTurnSpeed = 10f;

    public float maxHealth = 100f;
    public float healthRecovery = 20 / 60.0f;
    public float healthRecoveryDelay = 20.0f;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Tank Controller - State
 * Contains the state for a tank
 * This state is serialized and sent to other players so the tank can be replicated in multiplayer
*/


public class TankControllerState : MonoBehaviour
{
     [System.NonSerialized] public Vector3 positionOffset = Vector3.zero;

    public float currentAcceleration = 0f;
    public float currentBreakForce = 0f;
    public float currentTurnAngle = 0f;

    public float currentSpeed = 0f;
    public float currentMovePotential = 0f;
    public float deltaSinceTurn = 0f;
    
    public Vector3 targetPosition = new Vector3(10, 5, 10);

    // Dynamic fields
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public Quaternion rotation;
    public Quaternion turretRotation;
    public Quaternion barrelRotation;

    public bool invertedThrottle = false;

    public float health;
    public float deltaSinceDamage = 0f;

    [System.NonSerialized] private Rigidbody rb;
    [System.NonSerialized] private Transform turret;
    [System.NonSerialized] private Transform barrel;
    [System.NonSerialized] private HingeJoint hingeTurret;
    [System.NonSerialized] private HingeJoint hingeBarrel;
    [System.NonSerialized] private Transform rotatorTurret;
    [System.NonSerialized] private Transform rotatorBarrel;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        turret = transform.Find("body").Find("turret_hinge");
        barrel = turret.Find("turret_base").Find("turret").Find("barrel_hinge");
        hingeTurret = turret.GetComponent<HingeJoint>();
        hingeBarrel = barrel.GetComponent<HingeJoint>();
        rotatorTurret = hingeTurret.transform.GetChild(0).transform;
        rotatorBarrel = hingeBarrel.transform.GetChild(0).transform;
    }

    bool Ready() {
        if (!rb) {
            Start();
            return false;
        }
        return true;
    }

    [System.NonSerialized] private bool applyFixedUpdate = false;
    public void Apply(string serializedState)
    {
        if (!Ready()) return;

        JsonUtility.FromJsonOverwrite(serializedState, this);
        this.targetPosition += positionOffset;
        applyFixedUpdate = true;
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Ready()) return;

        if (applyFixedUpdate)
        {
            // Apply tank state
            applyFixedUpdate = false;
            rb.MovePosition(position + positionOffset);
            rb.MoveRotation(rotation);
            rb.angularVelocity = angularVelocity;
            rb.velocity = velocity;
            rotatorTurret.localRotation = turretRotation;
            rotatorBarrel.localRotation = barrelRotation;
        }

        position = transform.position;
        rotation = transform.rotation;
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        turretRotation = rotatorTurret.localRotation;
        barrelRotation = rotatorBarrel.localRotation;

        // DEBUG VISUALIZER
        // transform.Find("target").transform.position = targetPosition;
    }
}

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

    [System.NonSerialized] private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    [System.NonSerialized] private bool applyFixedUpdate = false;
    public void Apply(string serializedState)
    {
        JsonUtility.FromJsonOverwrite(serializedState, this);
        applyFixedUpdate = true;
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        position = transform.position;
        rotation = transform.rotation;
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;

        if (applyFixedUpdate)
        {
            applyFixedUpdate = false;
            rb.MovePosition(position + positionOffset);
            rb.MoveRotation(rotation);
            rb.angularVelocity = angularVelocity;
            rb.velocity = velocity;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Tank Movement Controller
 * - Handles all movement of a tank.
 * - Relies on the TankControllerState
*/



public class TankMovementController : MonoBehaviour
{
    private TankControllerState state;
    private TankConfig config;

    private WheelCollider[] wheelColliders;
    private Transform[] wheelTransforms;
    private int wheelCount;
    private int wheelCountPerSide;

    private float inversed_currentMovePotential = 0f;
    private bool movingForward = false;

    private Transform turret;
    private Transform barrel;
    private HingeJoint hingeTurret;
    private HingeJoint hingeBarrel;

    private Rigidbody rb;

    

    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<TankControllerState>();
        config = GetComponent<TankConfig>();
        rb = GetComponent<Rigidbody>();

        turret = transform.Find("body").Find("turret_hinge");
        barrel = turret.Find("turret_base").Find("turret").Find("barrel_hinge");
        hingeTurret = turret.GetComponent<HingeJoint>();
        hingeBarrel = barrel.GetComponent<HingeJoint>();

        Transform _wheels = transform.Find("wheels").Find("meshes");
        Transform _colliders = transform.Find("wheels").Find("colliders");
        wheelCount = _colliders.childCount;
        wheelCountPerSide = wheelCount / 2;

        wheelColliders = new WheelCollider[wheelCount];
        wheelTransforms = new Transform[wheelCount];

        for (int i = 0; i < wheelCount; ++i) {
            Transform collider = _colliders.GetChild(i);
            string name = collider.name;
            Transform wheel = _wheels.Find(name);
            bool left = name.EndsWith("_l");
            int num;
            int.TryParse(name.Split('-')[1].Split('_')[0], out num);
            --num;
            int index = left ? num : num + wheelCountPerSide;
            wheelColliders[index] = collider.GetComponent<WheelCollider>();
            wheelTransforms[index] = wheel;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        state.currentSpeed = rb.velocity.magnitude;
        state.currentMovePotential = state.currentSpeed / config.maxSpeed;

        float dotP = Vector3.Dot(transform.forward.normalized, rb.velocity.normalized);
        movingForward = dotP > 0.0f;

        inversed_currentMovePotential = 1 - state.currentMovePotential;

        UpdateAxis();
        UpdateTurret();

        // UpdateTurret();
        // FixedUpdateTurret();
        // UpdateBarrel();

        for (int i = 0; i < wheelCount; ++i) {
            UpdateWheel(i);
        }
    }

    void UpdateWheel(int _wheel)
    {

        bool left = _wheel < wheelCountPerSide;
        int num = left ? _wheel : _wheel - wheelCountPerSide;

        Transform wheel = wheelTransforms[_wheel];
        WheelCollider collider = wheelColliders[_wheel];

        float movingAngle = 0f;
        if (num < wheelCountPerSide / 2) {
            movingAngle = state.currentTurnAngle * 0.5f;
        } else {
            movingAngle = -state.currentTurnAngle * 0.5f;
        }

        float restingAngle = 0f;
        float restingTorque = 0f;
        float restingStiffness = 1f;
        if (state.currentTurnAngle < 0) {
            restingStiffness = 0f;
            if (left) {
                restingTorque = -config.acceleration;
            } else {
                restingTorque = config.acceleration;
            }
        } else if (state.currentTurnAngle > 0) {
            restingStiffness = 0f;
            if (left) {
                restingTorque = config.acceleration;
            } else {
                restingTorque = -config.acceleration;
            }
        }

        var friction = collider.sidewaysFriction;
        friction.stiffness = state.currentSpeed + (restingStiffness * inversed_currentMovePotential);
        collider.sidewaysFriction = friction;

        collider.brakeTorque = state.currentBreakForce;
        collider.motorTorque = state.currentAcceleration + (0.5f * restingTorque * inversed_currentMovePotential);
        collider.steerAngle = (movingAngle * Mathf.Clamp(state.currentMovePotential * 10, 0, 1)) + (restingAngle * inversed_currentMovePotential);

        if (!movingForward) {
            collider.steerAngle = -collider.steerAngle;
        }

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        wheel.position = position;
        wheel.rotation = rotation;
        // TODO: remove turning from wheel rotation
        // wheel.rotation = Quaternion.Euler(rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotation.eulerAngles.z);
        
    }

     void UpdateTurret()
    {
        float turretTargetAngle;
        float turretRelativeAngle;
        float barrelTargetAngle;
        float barrelRelativeAngle;


        Vector3 targetPosition = state.targetPosition;
        Vector3 up = transform.up;

        // Turret Calculations (the yaw & direction to rotate the turret)
        Vector3 barrelPosition;
        {
            Vector3 startPosition = transform.position;

            // Calculate directions and axises of rotation to reach the target
            Vector3 targetDirection = (targetPosition - startPosition).normalized;
            Vector3 targetLeft = Vector3.Cross(targetDirection, up).normalized;
            Vector3 targetForward = -Vector3.Cross(targetLeft, up).normalized;
            Vector3 targetUp = up;

            // Calculte the relative directions and axises
            Vector3 localLeft = Vector3.Cross(targetDirection, Vector3.up).normalized;
            Vector3 localForward = -Vector3.Cross(targetLeft, Vector3.up).normalized;
            Vector3 localUp = Vector3.up;

            // Calculate the current directions and axises
            Vector3 currentLeft = Vector3.Cross(turret.GetChild(0).forward, up).normalized;
            Vector3 currentForward = -Vector3.Cross(currentLeft, up).normalized;
            Vector3 currentUp = up;

            // Determine the target angle and the difference from the current rotation
            float targetAngle = Vector3.SignedAngle(Vector3.forward, targetForward, up);
            float relativeAngle = Vector3.SignedAngle(currentForward, targetForward, up);

            turretTargetAngle = targetAngle;
            turretRelativeAngle = relativeAngle;

            // Calculate the barrel's desired position as if the turret has finished rotating
            Vector3 localBarrelPosition = Quaternion.FromToRotation(currentForward, Vector3.forward) * (barrel.position - turret.position);
            Quaternion barrelRotation = Quaternion.FromToRotation(Vector3.forward, targetForward);

            barrelPosition = turret.transform.position + (barrelRotation * localBarrelPosition);
        }

        // Barrel Calculations (the yaw & angle the barrel needs to be at)
        {
            Vector3 startPosition = barrelPosition;

            // Calculate directions and axises of rotation to reach the target
            Vector3 targetDirection = (targetPosition - startPosition).normalized;
            Vector3 targetLeft = Vector3.Cross(targetDirection, up).normalized;
            Vector3 targetForward = -Vector3.Cross(targetLeft, up).normalized;
            Vector3 targetUp = up;

            // Calculte the relative directions and axises
            Vector3 currentLeft = targetLeft;
            Vector3 currentUp = -Vector3.Cross(barrel.GetChild(0).forward, -barrel.GetChild(0).right).normalized;
            Vector3 currentForward = -Vector3.Cross(currentLeft, currentUp);

            // Determine the target angle and the difference from the current rotation
            float targetAngle = Vector3.SignedAngle(targetForward, targetDirection, targetLeft);
            float currentAngle = Vector3.SignedAngle(barrel.forward, barrel.GetChild(0).forward, -barrel.right);
            float relativeAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

            barrelTargetAngle = targetAngle;
            barrelRelativeAngle = relativeAngle;
        }

        // Update Turret
        {
            float targetAngle = turretRelativeAngle;
            float magnitude = Mathf.Abs(targetAngle);

            float easing = magnitude > config.maxTurretTurnSpeed ? 1 : 1 + (1 - Mathf.Min(0.5f, magnitude / config.maxTurretTurnSpeed));

            float currentVelocity = hingeTurret.motor.targetVelocity;
            float desiredVelocity = Mathf.MoveTowardsAngle(currentVelocity, -targetAngle, config.maxTurretTurnSpeed * (easing * 1f)); //Time.fixedDeltaTime
            float targetVelocity = Mathf.Clamp(desiredVelocity, -config.maxTurretTurnSpeed, config.maxTurretTurnSpeed);

            JointMotor motor = hingeTurret.motor;
            motor.force = 1;
            motor.targetVelocity = targetVelocity;
            motor.freeSpin = false;

            hingeTurret.motor = motor;
            hingeTurret.useMotor = true;
        }

        // Update Barrel
        {
            float targetAngle = barrelRelativeAngle;
            float magnitude = Mathf.Abs(targetAngle);

            float easing = magnitude > config.maxBarrelTurnSpeed ? 1 : 1 + (1 - Mathf.Min(0.5f, magnitude / config.maxBarrelTurnSpeed));

            float currentVelocity = hingeBarrel.motor.targetVelocity;
            float desiredVelocity = Mathf.MoveTowardsAngle(currentVelocity, -targetAngle, config.maxBarrelTurnSpeed * (easing * 1f)); //Time.fixedDeltaTime
            float targetVelocity = Mathf.Clamp(desiredVelocity, -config.maxBarrelTurnSpeed, config.maxBarrelTurnSpeed);

            JointMotor motor = hingeBarrel.motor;
            motor.force = 1;
            motor.targetVelocity = -targetVelocity;
            motor.freeSpin = false;

            hingeBarrel.motor = motor;
            hingeBarrel.useMotor = true;
        }
    }

    void setAxis(string axis, Vector3 direction, float angle = 0f)
    {
        Transform obj = transform.Find("axis").Find(axis);
        obj.LookAt(obj.position + direction * 10f);
    }

    void UpdateAxis() {
        Transform _axes = transform.Find("axis");
        _axes.position = turret.GetChild(0).position;

        Transform left = _axes.Find("left");
        Transform up = _axes.Find("up");
        Transform forward = _axes.Find("forward");
        Transform aim = _axes.Find("aim");
        
        left.LookAt(left.position + (10f * turret.GetChild(0).right));

        Vector3 targetPosition = state.targetPosition;
        aim.LookAt(aim.position + (10f * (targetPosition - aim.position).normalized));

        Vector3 currentForward = turret.GetChild(0).forward;
        Vector3 currentUp = barrel.GetChild(0).up;

    }

}

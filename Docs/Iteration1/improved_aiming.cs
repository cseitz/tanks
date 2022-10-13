using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAim : MonoBehaviour
{
    public GameObject target;
    public GameObject turret;
    public GameObject barrel;

    HingeJoint turretHinge;
    HingeJoint barrelHinge;

    float maxTurnSpeed = 20;
    float turnSpeedAngle = 20;

    float maxAimSpeed = 10;
    float aimSpeedAngle = 10;

    // Start is called before the first frame update
    void Start()
    {
        turretHinge = turret.GetComponent<HingeJoint>();
        barrelHinge = barrel.GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurret();
        UpdateBarrel();
        // GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 2));
        // GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 2, 0));

    }

    void UpdateTurret()
    {
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = turret.transform.position.y;
        Vector3 forward = turret.transform.Find("$rotator").transform.forward;
        float desiredAngle = Vector3.SignedAngle(forward, (targetPosition - turret.transform.position).normalized, Vector3.up);
        float currentAngle = 0;
        float difference = desiredAngle - currentAngle;
        float magnitude = Mathf.Abs(difference);
        float direction = difference > 0 ? -1 : 1;
        float speed = Mathf.Min(1, (magnitude / turnSpeedAngle)) * maxTurnSpeed;

        JointMotor motor = turretHinge.motor;
        motor.force = 1;
        motor.targetVelocity = speed * direction;
        motor.freeSpin = true;

        turretHinge.motor = motor;
        turretHinge.useMotor = true;
    }

    void UpdateBarrel()
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 forward = barrel.transform.Find("$rotator").transform.forward;
        float desiredAngle = Vector3.SignedAngle(forward, (targetPosition - barrel.transform.position).normalized, turret.transform.Find("$rotator").transform.right);
        float currentAngle = 0;
        float difference = desiredAngle - currentAngle;
        float magnitude = Mathf.Abs(difference);
        float direction = difference > 0 ? -1 : 1;
        float speed = Mathf.Min(1, (magnitude / aimSpeedAngle)) * maxAimSpeed;

        JointMotor motor = barrelHinge.motor;
        motor.force = 1;
        motor.targetVelocity = speed * direction;
        motor.freeSpin = true;

        barrelHinge.motor = motor;
        barrelHinge.useMotor = true;

        JointLimits limits = barrelHinge.limits;
        limits.max = 55;
        limits.min = -15;
        barrelHinge.limits = limits;
        barrelHinge.useLimits = true;
    }
}

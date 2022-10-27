using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldTankController : MonoBehaviour
{
    public GameObject turret;
    public GameObject barrel;
    public Vector3 lookAt = Vector3.zero;
    private Vector3 _lookAt = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float turnSpeed = Mathf.Clamp(2 / Mathf.Abs((lookAt - _lookAt).magnitude), 0.05f, 0.45f) * 0.01f;
        _lookAt = Vector3.Lerp(_lookAt, lookAt, turnSpeed);
        turret.transform.LookAt(new Vector3(_lookAt.x, turret.transform.position.y, _lookAt.z), Vector3.up);
        barrel.transform.LookAt(_lookAt, Vector3.left);
        Vector3 rot = barrel.transform.localRotation.eulerAngles;
        barrel.transform.localRotation = Quaternion.AngleAxis(-rot.x, Vector3.left);
    }
}

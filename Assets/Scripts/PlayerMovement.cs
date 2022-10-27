using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private OldTankController tank;

    [SerializeField]
    private GameObject Rotation;
    [SerializeField]
    private GameObject Attitude;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tank = GetComponent<OldTankController>();
    }

    // Update is called once per frame
    Vector3 previousMousePosition = Vector3.zero;
    Vector3 mouseDelta;
    Vector3 rotation;
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mouseDelta = mousePosition - previousMousePosition;
        previousMousePosition = mousePosition;
        ApplyRotation();
    }

    void ApplyRotation()
    {
        Vector3 combined = rotation + new Vector3(0, mouseDelta.x * 0.5f, mouseDelta.y * 0.05f);
        rotation = new Vector3(0, combined.y, Mathf.Max(-20f, Mathf.Min(45, combined.z)));
        // tank.rotation = Quaternion.LookRotation
        // tank.lookAt = (Attitude.transform.localPosition + new Vector3(0, 0, 10));
        // tank.lookAt = Attitude.transform.position;
        Quaternion turn = Quaternion.AngleAxis(rotation.y, Vector3.up);
        Quaternion up = Quaternion.AngleAxis(rotation.z * 0.5f, Vector3.left);
        // Rotation.transform.localRotation = turn * up;
        // Attitude.transform.localRotation = up * up;
        Update2();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        // Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical);
        rb.AddRelativeForce(movement * speed);
        Vector3 torque = new Vector3(0.0f, moveHorizontal, 0.0f);
        rb.AddRelativeTorque(torque * speed);
        

        float x = 5 * Input.GetAxis("Mouse X");
        float y = 5 * -Input.GetAxis("Mouse Y");
        // transform.Rotate(x, y, 0);

        

    }

    public float sensitivity = 2f;
    private Vector2 currentRotation;
    void Update2()
    {
        if (Input.GetMouseButtonDown(0) || true)
        {
            currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -80f, 40f);
            // transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            Rotation.transform.localRotation = Quaternion.Euler(currentRotation.y * 0.5f, currentRotation.x, 0);
            Attitude.transform.localRotation = Quaternion.Euler(currentRotation.y * 0.5f, 0, 0);
            tank.lookAt = Attitude.transform.GetChild(0).transform.position;
            // Cursor.lockState = CursorLockMode.Locked;
        } else {
            // Cursor.lockState = CursorLockMode.None;
        }
    }
}

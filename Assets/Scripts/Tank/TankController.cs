using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Tank Controller
 * Handles player input and client-only tank logic.
*/


public class TankController : MonoBehaviour
{
    public Transform virtualCamera;

    [System.NonSerialized] public TankControllerState state;
    [System.NonSerialized] public TankConfig config;

    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<TankControllerState>();
        config = GetComponent<TankConfig>();
    }

    void FixedUpdate()
    {

        float iVertical = Input.GetAxis("Vertical");
        float iHorizontal = Input.GetAxis("Horizontal");

        if (state.currentSpeed > config.maxSpeed) iVertical = 0f;
        state.currentAcceleration = config.acceleration * iVertical;

        if (Mathf.Abs(iHorizontal) <= 0.05f) {
            state.deltaSinceTurn += Time.fixedDeltaTime;
        } else {
            state.deltaSinceTurn = 0f;
        }

        float desiredTurnAngle = config.maxTurnAngle * iHorizontal;
        state.currentTurnAngle = Mathf.MoveTowardsAngle(
            state.currentTurnAngle,
            desiredTurnAngle,
            (0.1f * config.maxTurnAngle) + (0.9f * (config.maxTurnAngle - state.currentTurnAngle))
        );

        if (Input.GetKey(KeyCode.Space)) {
            state.currentBreakForce = config.breakingForce;
        } else {
            state.currentBreakForce = 0f;
        }

    }

    public float sensitivity = 2f / 10f;
    public float cameraHeight = 1f;
    private Vector2 cameraRotation;

    void Update()
    {
        // UpdateCamera();
        // print(Input.GetMouseButton(0) + ":" + Input.GetMouseButton(1) + ":" + Input.GetMouseButton(2));
        if (Input.GetMouseButton(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            cameraRotation.x += Input.GetAxis("Mouse X") * sensitivity;
            cameraRotation.y -= Input.GetAxis("Mouse Y") * sensitivity * 0.5f;
            cameraRotation.x = Mathf.Repeat(cameraRotation.x, 360);
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -40f, 40f);
        } else {
            Cursor.lockState = CursorLockMode.None;
        }

        // transform.Find("camera").GetChild(0).transform.position = new Vector3(0, cameraHeight * (1 - Mathf.Clamp01(currentRotation.y / 40f)), 0);
        transform.Find("camera").GetChild(0).transform.localPosition = new Vector3(0, 1f * Mathf.Clamp01(cameraRotation.y / -40f), 0);

        transform.Find("camera").rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);

        // state.targetPosition = virtualCamera.transform.position + (virtualCamera.transform.forward * 100);
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        state.targetPosition = ray.origin + ray.direction * ((transform.position - ray.origin).magnitude * 2);

        // transform.Find("target").transform.position = state.targetPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/** Tank Controller
 * Handles player input and client-only tank logic.
*/


public class TankController : MonoBehaviour
{

    [System.NonSerialized] public TankControllerState state;
    [System.NonSerialized] public TankConfig config;

    private CinemachineBrain brainVirtualCamera;
    private CinemachineVirtualCamera virtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<TankControllerState>();
        config = GetComponent<TankConfig>();

        brainVirtualCamera = Camera.main.GetComponent<CinemachineBrain>();
        try {
            if (brainVirtualCamera != null && brainVirtualCamera.ActiveVirtualCamera != null) {
                if (brainVirtualCamera.ActiveVirtualCamera.VirtualCameraGameObject != null) {
                    virtualCamera = brainVirtualCamera.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
                }
            }
        } catch(UnityException err) {
        }
    }

    bool Ready() {
        if (!state || !brainVirtualCamera || brainVirtualCamera.ActiveVirtualCamera == null) {
            Start();
            return false;
        }
        return true;
    }

    void FixedUpdate()
    {
        if (!Ready()) return;
        if (state.health <= 0) return;

        float iVertical = Input.GetAxis("Vertical");
        float iHorizontal = Input.GetAxis("Horizontal");
        
        float dotP = Vector3.Dot(transform.Find("body").forward, Camera.main.gameObject.transform.forward);
        if (Mathf.Abs(state.currentSpeed / config.maxSpeed) <= 0.1f) {
            state.invertedThrottle = dotP < -0.6f;
            // print(state.invertedThrottle + " -> " + dotP);
        }
        if (state.invertedThrottle) {
            iVertical = -iVertical;
        }

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

        if (Input.GetMouseButton(0)) {
            GetComponent<TankCombatController>().Shoot("mainGun");
        }

    }

    public float sensitivity = 2f / 10f;
    public float cameraHeight = 1f;
    private Vector2 cameraRotation;
    private Vector3 lastPosition = Vector3.zero;

    void Update()
    {
        if (!Ready()) return;

        Vector3 deltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;

        if (Input.GetMouseButton(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            cameraRotation.x += Input.GetAxis("Mouse X") * sensitivity;
            cameraRotation.y -= Input.GetAxis("Mouse Y") * sensitivity * 0.5f;
            cameraRotation.x = Mathf.Repeat(cameraRotation.x, 360);
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -40f, 40f);
        } else {
            Cursor.lockState = CursorLockMode.None;
        }

        transform.Find("camera").GetChild(0).transform.position = transform.Find("camera").position + (
            new Vector3(0, 1.5f * Mathf.Clamp01(cameraRotation.y / -40f), 0)
        );

        transform.Find("camera").rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);
        Camera.main.transform.position += deltaPosition;

        Cinemachine3rdPersonFollow follow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        follow.CameraDistance = 4f + (1f * Mathf.Clamp(state.currentSpeed / config.maxSpeed, -1, 1));

        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPosition = ray.origin + ray.direction * ((transform.position - ray.origin).magnitude * 3f);
        if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
            targetPosition = hit.point;
        }
        state.targetPosition = targetPosition;

        // DEBUG VISUALIZER
        // transform.Find("target").transform.position = state.targetPosition;
    }
}

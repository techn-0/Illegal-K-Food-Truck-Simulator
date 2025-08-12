using UnityEngine;
using UnityEngine.InputSystem;   // 새 Input System

/// <summary>트럭 주행 스크립트</summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider wcFL, wcFR, wcRL, wcRR;

    [Header("Wheel Meshes (Visual)")]
    public Transform meshFL, meshFR, meshRL, meshRR;

    [Header("Spec")]
    [Tooltip("최대 모터 토크 (N·m)")]
    public float maxMotorTorque = 2500f;
    [Tooltip("최대 조향 각 (deg)")]
    public float maxSteerAngle = 28f;
    [Tooltip("속도(km/h)별 조향 감소 한계")]
    public float steerFadeSpeed = 15f;   // 이 속도 이상부터 감쇄 0→1
    [Tooltip("브레이크 토크 (N·m)")]
    public float brakeTorque = 6000f;

    // ───────────────── InputActionReference 연결용 ─────────────────
    [Header("Input Actions")]
    public InputActionReference moveAction;   // 2D Vector (y: 전/후, x: 좌/우)
    public InputActionReference brakeAction;  // Button (Space 등)

    float throttle;   // -1 ~ 1  (전/후진)
    float steer;      // -1 ~ 1
    bool braking;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // ❶ 콜백 연결만 한다 (Enable() 호출 제거)
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += _ => throttle = steer = 0;

        brakeAction.action.performed += _ => braking = true;
        brakeAction.action.canceled += _ => braking = false;
    }

    void OnEnable()           // ❷ 컴포넌트 켜질 때 한 번만 Enable
    {
        moveAction.action.Enable();
        brakeAction.action.Enable();
    }
    void OnDisable()          // ❸ 끌 때 Disable
    {
        moveAction.action.Disable();
        brakeAction.action.Disable();
    }

    void OnDestroy()          // ❹ 중복 콜백 방지
    {
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= _ => throttle = steer = 0;
        brakeAction.action.performed -= _ => braking = true;
        brakeAction.action.canceled -= _ => braking = false;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        throttle = v.y;
        steer = v.x;
    }

    void FixedUpdate()
    {
        float speedKmh = rb.linearVelocity.magnitude * 3.6f;

        // ── 1. 추진 ──────────────────────────────
        float motor = throttle * maxMotorTorque;
        wcRL.motorTorque = wcRR.motorTorque = motor;

        // ── 2. 브레이크 ──────────────────────────
        float brake = braking ? brakeTorque : 0f;
        wcFL.brakeTorque = wcFR.brakeTorque = wcRL.brakeTorque = wcRR.brakeTorque = brake;

        // ── 3. 조향 (속도 감쇄) ───────────────────
        // 후진 시에도 조향이 가능하도록 수정
        float steerFactor = speedKmh < 5f ? 1f : Mathf.Clamp01(speedKmh / steerFadeSpeed);
        float steerAngle = steer * maxSteerAngle * steerFactor;
        
        // 후진 시 조향 방향 반전
        if (throttle < 0)
            steerAngle *= -1f;
            
        wcFL.steerAngle = wcFR.steerAngle = steerAngle;

        // ── 4. 시각적 바퀴 동기화 ────────────────
        PoseWheel(wcFL, meshFL);
        PoseWheel(wcFR, meshFR);
        PoseWheel(wcRL, meshRL);
        PoseWheel(wcRR, meshRR);
    }

    void PoseWheel(WheelCollider col, Transform vis)
    {
        col.GetWorldPose(out var pos, out var rot);
        vis.SetPositionAndRotation(pos, rot);
    }
}

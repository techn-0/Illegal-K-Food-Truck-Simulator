using UnityEngine;

/// <summary>단순한 자동차 시스템 - 운전 + 승하차 통합</summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("자동차 설정")]
    public float speed = 30f;           // 속도
    public float turnSpeed = 100f;      // 회전 속도
    public float acceleration = 5f;     // 가속도

    [Header("승하차 설정")]
    public Transform driverSeat;        // 운전석 위치
    public Transform exitPoint;         // 하차 위치

    private Rigidbody rb;
    private float currentSpeed;
    private float targetSpeed;
    
    // 승하차 관련
    private bool playerNear = false;    // 플레이어가 근처에 있는지
    private bool driving = false;       // 운전 중인지
    private GameObject player;          // 플레이어 오브젝트

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // 안정성을 위한 낮은 무게중심
    }

    void Update()
    {
        // E 키로 승하차
        if (playerNear && Input.GetKeyDown(KeyCode.F))
        {
            if (driving)
                GetOutOfCar();
            else
                GetInCar();
        }

        // 운전 중일 때만 자동차 조작 가능
        if (driving)
        {
            HandleCarInput();
        }
    }

    void HandleCarInput()
    {
        // 간단한 입력
        float vertical = Input.GetAxis("Vertical");     // W/S
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        bool brake = Input.GetKey(KeyCode.Space);       // 스페이스

        // 목표 속도 설정
        if (brake)
            targetSpeed = 0f;
        else
            targetSpeed = vertical * speed;

        // 부드러운 가속/감속
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime * 10f);

        // 회전 (이동 중일 때만)
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turn = horizontal * turnSpeed * Time.deltaTime;
            if (currentSpeed < 0) turn *= -1; // 후진시 반전
            transform.Rotate(0, turn, 0);
        }
    }

    void FixedUpdate()
    {
        // 운전 중일 때만 이동
        if (driving)
        {
            Vector3 forwardMovement = transform.forward * currentSpeed;
            rb.linearVelocity = new Vector3(forwardMovement.x, rb.linearVelocity.y, forwardMovement.z);
        }
        else
        {
            // 운전 중이 아닐 때는 차량을 완전히 정지
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            player = null;
        }
    }

    void GetInCar()
    {
        if (player == null || driverSeat == null) return;

        driving = true;
        
        // 플레이어를 운전석으로 이동
        player.transform.position = driverSeat.position;
        player.transform.rotation = driverSeat.rotation;
        player.transform.SetParent(driverSeat);

        // 플레이어 이동 막기
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
    }

    void GetOutOfCar()
    {
        if (player == null || exitPoint == null) return;

        driving = false;
        
        // 자동차 완전 정지
        currentSpeed = 0f;
        targetSpeed = 0f;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        rb.angularVelocity = Vector3.zero;

        // 플레이어를 하차 위치로 이동
        player.transform.SetParent(null);
        player.transform.position = exitPoint.position;
        player.transform.rotation = exitPoint.rotation;

        // 플레이어 이동 허용
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;
    }
}

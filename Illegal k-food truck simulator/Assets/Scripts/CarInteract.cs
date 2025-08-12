using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 트리거 안에서 E 키로 승·하차 전환
/// </summary>
public class CarInteract : MonoBehaviour
{
    [Tooltip("SeatPoint Transform")]
    public Transform seatPoint;
    [Tooltip("ExitPoint Transform")]
    public Transform exitPoint;
    [Tooltip("운전 스크립트")]
    public CarController carCtrl;

    const Key KeyInteract = Key.E;  // 키 직접 체크

    bool inRange;
    bool driving;
    GameObject playerObj;
    CharacterController playerCC;
    SkinnedMeshRenderer[] playerSkins;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inRange    = true;
        playerObj  = other.gameObject;
        playerCC   = other.GetComponent<CharacterController>();
        playerSkins = other.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inRange = false;
    }

    void Update()
    {
        if (!inRange) return;

        if (Keyboard.current[KeyInteract].wasPressedThisFrame)
        {
            if (!driving) Enter();
            else          Exit();
        }
    }

    void Enter()
    {
        driving = true;

        // 1) 플레이어 숨기고 이동 막기
        TogglePlayer(false);
        playerCC.enabled = false;

        // 2) 좌석 고정
        playerObj.transform.SetPositionAndRotation(seatPoint.position, seatPoint.rotation);
        playerObj.transform.SetParent(seatPoint);

        // 3) 차량 제어 on
        carCtrl.enabled = true;
    }

    void Exit()
    {
        driving = false;

        // 1) 원래 계층 분리 + 하차 위치 이동
        playerObj.transform.SetParent(null);
        playerObj.transform.position = exitPoint.position;

        // 2) 플레이어 보이기 + 이동 on
        TogglePlayer(true);
        playerCC.enabled = true;

        // 3) 차량 제어 off
        carCtrl.enabled = false;
    }

    void TogglePlayer(bool visible)
    {
        foreach (var skin in playerSkins) skin.enabled = visible;
    }
}

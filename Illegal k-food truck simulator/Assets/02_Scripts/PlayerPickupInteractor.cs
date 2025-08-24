using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 아이템 픽업 상호작용 시스템
/// 트리거 영역 내 아이템 감지 및 상호작용 키로 픽업 처리
/// 필수: CharacterController/Rigidbody + Trigger Collider
/// </summary>
public class PlayerPickupInteractor : MonoBehaviour
{
    [SerializeField] private Inventory inventory; // 아이템을 저장할 인벤토리
    
    private InputAction interactAction; // 상호작용 입력 액션 (E키, 게임패드 버튼)
    private readonly List<PickupTarget> candidates = new(); // 픽업 가능 범위 내 아이템 목록

    /// <summary>
    /// 활성화 시 상호작용 입력 설정 및 이벤트 바인딩
    /// </summary>
    private void OnEnable()
    {
        // 상호작용 입력 설정: E키와 게임패드 남쪽 버튼
        interactAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
        interactAction.AddBinding("<Gamepad>/buttonSouth");
        interactAction.performed += OnInteractPerformed;
        interactAction.Enable();
    }

    /// <summary>
    /// 비활성화 시 입력 액션 정리 및 메모리 해제
    /// </summary>
    private void OnDisable()
    {
        // 입력 액션 정리
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
            interactAction.Disable();
            interactAction.Dispose();
        }
    }

    /// <summary>
    /// 트리거 진입: 픽업 가능한 아이템을 후보 목록에 추가
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 픽업 대상이 범위에 들어왔을 때 후보 목록에 추가
        if (other.TryGetComponent<PickupTarget>(out var pickupTarget))
        {
            if (!candidates.Contains(pickupTarget))
                candidates.Add(pickupTarget);
        }
    }

    /// <summary>
    /// 트리거 탈출: 범위를 벗어난 아이템을 후보 목록에서 제거
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        // 픽업 대상이 범위를 벗어났을 때 후보 목록에서 제거
        if (other.TryGetComponent<PickupTarget>(out var pickupTarget))
            candidates.Remove(pickupTarget);
    }

    /// <summary>
    /// 상호작용 키 입력 처리: 가장 가까운 아이템 픽업 시도
    /// </summary>
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        // 상호작용 키가 눌렸을 때: 가장 가까운 아이템 픽업
        if (inventory == null) return;

        var nearest = GetNearestCandidate();
        if (nearest != null && nearest.TryPickup(inventory))
            candidates.Remove(nearest); // 픽업 성공 시 목록에서 제거
    }

    /// <summary>
    /// 후보 목록에서 플레이어와 가장 가까운 픽업 대상 찾기
    /// </summary>
    private PickupTarget GetNearestCandidate()
    {
        // 파괴된 오브젝트 정리 (null 참조 방지)
        candidates.RemoveAll(c => c == null);
        
        if (candidates.Count == 0) return null;
        
        // 가장 가까운 픽업 대상 찾기 (거리 기반 비교)
        var nearest = candidates[0];
        var nearestDist = Vector3.Distance(transform.position, nearest.transform.position);

        for (int i = 1; i < candidates.Count; i++)
        {
            var dist = Vector3.Distance(transform.position, candidates[i].transform.position);
            if (dist < nearestDist)
            {
                nearest = candidates[i];
                nearestDist = dist;
            }
        }

        return nearest;
    }
}

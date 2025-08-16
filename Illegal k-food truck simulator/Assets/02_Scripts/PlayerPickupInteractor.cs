using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 픽업 상호작용 처리
/// 필수: CharacterController/Rigidbody + Trigger Collider
/// </summary>
public class PlayerPickupInteractor : MonoBehaviour
{
    [SerializeField] private Inventory inventory; // 아이템을 저장할 인벤토리
    
    private InputAction interactAction; // 상호작용 입력 액션
    private readonly List<PickupTarget> candidates = new(); // 근처 픽업 가능한 대상들

    private void OnEnable()
    {
        // 상호작용 입력 설정: E키와 게임패드 남쪽 버튼
        interactAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
        interactAction.AddBinding("<Gamepad>/buttonSouth");
        interactAction.performed += OnInteractPerformed;
        interactAction.Enable();
    }

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

    private void OnTriggerEnter(Collider other)
    {
        // 픽업 대상이 범위에 들어왔을 때 후보 목록에 추가
        if (other.TryGetComponent<PickupTarget>(out var pickupTarget))
        {
            if (!candidates.Contains(pickupTarget))
                candidates.Add(pickupTarget);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 픽업 대상이 범위를 벗어났을 때 후보 목록에서 제거
        if (other.TryGetComponent<PickupTarget>(out var pickupTarget))
            candidates.Remove(pickupTarget);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        // 상호작용 키가 눌렸을 때: 가장 가까운 아이템 픽업
        if (inventory == null) return;

        var nearest = GetNearestCandidate();
        if (nearest != null && nearest.TryPickup(inventory))
            candidates.Remove(nearest); // 픽업 성공 시 목록에서 제거
    }

    private PickupTarget GetNearestCandidate()
    {
        // 파괴된 오브젝트 정리
        candidates.RemoveAll(c => c == null);
        
        if (candidates.Count == 0) return null;
        
        // 가장 가까운 픽업 대상 찾기
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

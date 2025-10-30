using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dialogue
{
    /// <summary>
    /// 플레이어 대화 상호작용 시스템
    /// 트리거 영역 내 대화 가능한 오브젝트 감지 및 상호작용 키로 대화 시작
    /// 필수: CharacterController/Rigidbody + Trigger Collider
    /// </summary>
    public class PlayerDialogueInteractor : MonoBehaviour
    {
        [SerializeField] private DialogueManager dialogueManager; // 대화 매니저
        
        private InputAction interactAction; // 상호작용 입력 액션 (E키, 게임패드 버튼)
        private readonly List<DialogueTarget> candidates = new(); // 대화 가능 범위 내 오브젝트 목록

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
            if (interactAction != null)
            {
                interactAction.performed -= OnInteractPerformed;
                interactAction.Disable();
                interactAction.Dispose();
            }
        }

        /// <summary>
        /// 트리거 진입: 대화 가능한 오브젝트를 후보 목록에 추가
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<DialogueTarget>(out var dialogueTarget))
            {
                if (!candidates.Contains(dialogueTarget))
                    candidates.Add(dialogueTarget);
            }
        }

        /// <summary>
        /// 트리거 탈출: 범위를 벗어난 오브젝트를 후보 목록에서 제거
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<DialogueTarget>(out var dialogueTarget))
                candidates.Remove(dialogueTarget);
        }

        /// <summary>
        /// 상호작용 키 입력 처리: 가장 가까운 대화 오브젝트와 대화 시작
        /// </summary>
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (dialogueManager == null) return;

            // 이미 대화 중이면 무시
            if (dialogueManager.IsDialogueActive()) return;

            var nearest = GetNearestCandidate();
            if (nearest != null && nearest.TryStartDialogue(dialogueManager))
                candidates.Remove(nearest); // 대화 시작 성공 시 목록에서 제거 (선택���)
        }

        /// <summary>
        /// 후보 목록에서 플레이어와 가장 가까운 대화 대상 찾기
        /// </summary>
        private DialogueTarget GetNearestCandidate()
        {
            // 파괴된 오브젝트 정리 (null 참조 방지)
            candidates.RemoveAll(c => c == null);
            
            if (candidates.Count == 0) return null;
            
            // 가장 가까운 대화 대상 찾기 (거리 기반 비교)
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
}


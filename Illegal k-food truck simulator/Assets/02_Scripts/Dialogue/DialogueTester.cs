using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// 다이얼로그 시스템 테스트용 스크립트 / Dialogue System Test Script
    /// 게임 시작 시 자동으로 다이얼로그를 시작하는 예시
    /// </summary>
    public class DialogueTester : MonoBehaviour
    {
        [Header("테스트 설정 / Test Settings")]
        [SerializeField] private DialogueManager dialogueManager;  // 다이얼로그 매니저 참조
        [SerializeField] private int startDialogueId = 100;        // 시작할 다이얼로그 ID
        [SerializeField] private bool autoStartOnAwake = true;     // 시작 시 자동 실행 여부

        /// <summary>
        /// 초기화 시 자동 시작 / Auto start on initialization
        /// </summary>
        private void Start()
        {
            if (autoStartOnAwake && dialogueManager != null)
            {
                // 약간의 지연 후 다이얼로그 시작 (UI 초기화 대기)
                Invoke(nameof(StartTestDialogue), 0.1f);
            }
        }

        /// <summary>
        /// 테스트 다이얼로그 시작 / Start test dialogue
        /// </summary>
        public void StartTestDialogue()
        {
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(startDialogueId);
            }
            else
            {
                Debug.LogError("DialogueManager is not assigned to DialogueTester");
            }
        }

        /// <summary>
        /// 키보드 입력으로 다이얼로그 테스트 / Test dialogue with keyboard input
        /// </summary>
        private void Update()
        {
            // T 키로 다이얼로그 재시작
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartTestDialogue();
            }

            // ESC 키로 다이얼로그 강제 종료
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (dialogueManager != null)
                {
                    dialogueManager.ForceEndDialogue();
                }
            }
        }
    }
}

using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// 대화 상호작용 가능한 오브젝트
    /// 특정 CSV 대화 데이터와 연결됨
    /// </summary>
    public class DialogueTarget : MonoBehaviour
    {
        [Header("대화 설정")]
        [SerializeField] private TextAsset dialogueCSV;  // 대화 CSV 파일
        [SerializeField] private int startDialogueId;     // 시작 대화 ID

        /// <summary>
        /// 대화 CSV 데이터 (읽기 전용)
        /// </summary>
        public TextAsset DialogueCSV => dialogueCSV;

        /// <summary>
        /// 시작 대화 ID (읽기 전용)
        /// </summary>
        public int StartDialogueId => startDialogueId;

        /// <summary>
        /// 대화 시작 시도
        /// </summary>
        /// <param name="dialogueManager">대화 매니저</param>
        /// <returns>대화 시작 성공 여부</returns>
        public bool TryStartDialogue(DialogueManager dialogueManager)
        {
            if (dialogueManager == null || dialogueCSV == null)
            {
                return false;
            }

            // 대화 매니저에 CSV 로드 및 시작
            dialogueManager.LoadAndStartDialogue(dialogueCSV, startDialogueId);
            return true;
        }
    }
}


using UnityEngine;
using TMPro;

namespace Minigame
{
    /// <summary>미니게임 테스트용 트리거 (개발/테스트 전용)</summary>
    public class MinigameTestTrigger : MonoBehaviour
    {
        [Header("테스트 설정")]
        public MinigameId testMinigameId = MinigameId.CatchPigeon;
        public KeyCode triggerKey = KeyCode.T;

        [Header("UI (선택)")]
        public TextMeshProUGUI resultText;

        private void Update()
        {
            if (Input.GetKeyDown(triggerKey))
            {
                StartTestMinigame();
            }
        }

        public void StartTestMinigame()
        {
            MiniGameManager.Instance.StartMinigame(testMinigameId, OnMinigameCompleted);
        }

        private void OnMinigameCompleted(MiniGameResult result)
        {
            Debug.Log($"[미니게임 완료] 점수: {result.score:F1}, 등급: {result.rank}, 시간: {result.duration:F2}초, 중단: {result.aborted}");

            if (resultText != null)
            {
                resultText.text = $"결과 - 점수: {result.score:F1} | 등급: {result.rank} | 시간: {result.duration:F2}초";
            }
        }
    }
}


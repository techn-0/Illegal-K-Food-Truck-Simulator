using UnityEngine;

namespace Minigame
{
    /// <summary>반죽 섞기 미니게임 파라미터</summary>
    [CreateAssetMenu(fileName = "MixDoughParameters", menuName = "Minigame/Parameters/Mix Dough")]
    public class MixDoughParameters : MinigameParametersBase
    {
        [Header("반죽 섞기 설정")]
        [Tooltip("목표 회전 수")]
        public int targetRotations = 10;

        [Tooltip("제한 시간")]
        public float timeLimit = 5f;

        [Tooltip("한 바퀴로 인정되는 최소 각도")]
        public float minRotationAngle = 300f;

        [Tooltip("역회전 감산 비율")]
        public float reverseRotationPenalty = 0.5f;

        [Tooltip("지연 시간당 감점")]
        public float latePenaltyPerSecond = 5f;

        [Tooltip("완벽 클리어 시간 (이보다 빠르면 만점)")]
        public float perfectClearTime = 4f;

        [Tooltip("마우스 감도")]
        public float mouseSensitivity = 1f;
    }
}


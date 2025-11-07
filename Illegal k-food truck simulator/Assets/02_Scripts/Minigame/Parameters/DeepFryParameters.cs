using UnityEngine;

namespace Minigame
{
    /// <summary>튀기기 미니게임 파라미터</summary>
    [CreateAssetMenu(fileName = "DeepFryParameters", menuName = "Minigame/Parameters/Deep Fry")]
    public class DeepFryParameters : MinigameParametersBase
    {
        [Header("튀기기 설정")]
        [Tooltip("목표 시간 (초)")]
        public float targetTime = 10f;

        [Tooltip("가우시안 표준편차 (작을수록 엄격)")]
        public float sigma = 0.5f;

        [Tooltip("완벽 판정 범위 (±초)")]
        public float perfectRange = 0.1f;

        [Tooltip("최대 대기 시간 (자동 실패)")]
        public float maxWaitTime = 15f;

        [Tooltip("너무 빠른 릴리즈 추가 감점")]
        public float earlyReleasePenalty = 1.5f;
    }
}


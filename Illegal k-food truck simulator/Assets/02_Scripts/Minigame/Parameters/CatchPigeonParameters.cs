using UnityEngine;

namespace Minigame
{
    /// <summary>비둘기 잡기 미니게임 파라미터</summary>
    [CreateAssetMenu(fileName = "CatchPigeonParameters", menuName = "Minigame/Parameters/Catch Pigeon")]
    public class CatchPigeonParameters : MinigameParametersBase
    {
        [Header("비둘기 잡기 설정")]
        [Tooltip("바가 왕복하는 속도")]
        public float barSpeed = 1.5f;

        [Tooltip("바의 이동 범위 (-range ~ +range)")]
        public float barRange = 5f;

        [Tooltip("바의 이동 패턴 (시간 0~1 → 위치 -1~1)")]
        public AnimationCurve barMovementCurve = AnimationCurve.Linear(0, -1, 1, 1);

        [Tooltip("완벽 판정 범위 (중앙 기준)")]
        public float perfectZone = 0.2f;

        [Tooltip("플레이 제한 시간 (0=무제한)")]
        public float playTimeLimit = 10f;

        [Header("감마 커브 설정")]
        [Tooltip("점수 계산 감마값 (높을수록 완벽에 가까워야 높은 점수)")]
        public float scoreGamma = 3.0f;
    }
}

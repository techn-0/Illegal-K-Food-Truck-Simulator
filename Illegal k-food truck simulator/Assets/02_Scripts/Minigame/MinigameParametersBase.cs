using UnityEngine;

namespace Minigame
{
    /// <summary>미니게임 파라미터 베이스 클래스</summary>
    public abstract class MinigameParametersBase : ScriptableObject
    {
        [Header("공통 설정")]
        [Tooltip("미니게임 식별자")]
        public MinigameId minigameId;

        [Tooltip("준비 시간 (카운트다운)")]
        public float prepareTime = 3f;

        [Tooltip("결과 표시 시간")]
        public float resultDisplayTime = 2f;

        [Header("점수 계산")]
        [Tooltip("점수 계산 커브 (0=완벽 실수, 1=완벽 성공)")]
        public AnimationCurve scoringCurve = AnimationCurve.EaseInOut(0, 0, 1, 100);
    }
}


using UnityEngine;

namespace Minigame
{
    /// <summary>밀가루 담기 미니게임 파라미터</summary>
    [CreateAssetMenu(fileName = "PourFlourParameters", menuName = "Minigame/Parameters/Pour Flour")]
    public class PourFlourParameters : MinigameParametersBase
    {
        [Header("밀가루 담기 설정")]
        [Tooltip("목표 양")]
        public float targetAmount = 100f;

        [Tooltip("허용 범위 (목표량 ±)")]
        public float toleranceRange = 10f;

        [Tooltip("초당 유량 (Hold 시)")]
        public float flowRate = 20f;

        [Tooltip("유량 증가 커브 (Hold 시간 0~1 → 유량 배율)")]
        public AnimationCurve flowCurve = AnimationCurve.Linear(0, 0.5f, 1, 1.5f);

        [Tooltip("오버슈트 감점 배율")]
        public float overshootPenalty = 2f;

        [Tooltip("최대 플레이 시간")]
        public float maxPlayTime = 15f;
    }
}


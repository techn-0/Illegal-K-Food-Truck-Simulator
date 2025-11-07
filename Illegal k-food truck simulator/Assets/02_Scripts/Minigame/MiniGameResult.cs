using UnityEngine;

namespace Minigame
{
    /// <summary>미니게임 결과 데이터</summary>
    public struct MiniGameResult
    {
        public float score;      // 0~100 점수
        public char rank;        // S/A/B/C/F 등급
        public float duration;   // 소요 시간 (선택)
        public bool aborted;     // 중단 여부

        public MiniGameResult(float score, float duration = 0f, bool aborted = false)
        {
            this.score = Mathf.Clamp(score, 0f, 100f);
            this.duration = duration;
            this.aborted = aborted;
            this.rank = CalculateRank(this.score);
        }

        private static char CalculateRank(float score)
        {
            if (score >= 90f) return 'S';
            if (score >= 75f) return 'A';
            if (score >= 60f) return 'B';
            if (score >= 40f) return 'C';
            return 'F';
        }
    }
}


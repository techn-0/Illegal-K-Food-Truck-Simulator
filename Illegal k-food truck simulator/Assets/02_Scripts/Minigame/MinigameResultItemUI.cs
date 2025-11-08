using UnityEngine;
using TMPro;

namespace Minigame
{
    /// <summary>
    /// 개별 미니게임 결과 아이템 UI
    /// </summary>
    public class MinigameResultItemUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI indexText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI rankText;

        /// <summary>
        /// 결과 아이템 설정
        /// </summary>
        public void Setup(int index, MiniGameResult result)
        {
            // 인덱스 표시
            if (indexText != null)
            {
                indexText.text = $"미니게임 {index}";
            }

            // 점수 표시
            if (scoreText != null)
            {
                scoreText.text = $"{result.score:F1}점";
            }

            // 랭크 표시
            if (rankText != null)
            {
                rankText.text = result.rank.ToString();
                rankText.color = GetRankColor(result.rank);
            }
        }

        /// <summary>
        /// 랭크별 색상 반환
        /// </summary>
        private Color GetRankColor(char rank)
        {
            switch (rank)
            {
                case 'S': return new Color(1f, 0.84f, 0f); // 금색
                case 'A': return new Color(0.53f, 0.81f, 0.92f); // 하늘색
                case 'B': return new Color(0.56f, 0.93f, 0.56f); // 연두색
                case 'C': return new Color(1f, 0.65f, 0f); // 주황색
                case 'F': return new Color(0.86f, 0.08f, 0.24f); // 빨강색
                default: return Color.white;
            }
        }
    }
}


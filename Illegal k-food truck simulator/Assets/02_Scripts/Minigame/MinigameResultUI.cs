using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

namespace Minigame
{
    /// <summary>
    /// 미니게임 결과를 표시하는 UI
    /// </summary>
    public class MinigameResultUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Transform resultItemContainer;
        [SerializeField] private GameObject resultItemPrefab;
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI finalRankText;
        [SerializeField] private TextMeshProUGUI totalPriceText;
        [SerializeField] private Button confirmButton;

        private System.Action onConfirm;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmClicked);
            }

            canvasGroup = resultPanel?.GetComponent<CanvasGroup>();
            if (canvasGroup == null && resultPanel != null)
            {
                canvasGroup = resultPanel.AddComponent<CanvasGroup>();
            }

            Hide();
        }

        /// <summary>
        /// 미니게임 결과 표시
        /// </summary>
        public void Show(List<MiniGameResult> results, char finalRank, int totalPrice, System.Action onConfirmCallback = null)
        {
            onConfirm = onConfirmCallback;

            // 기존 아이템 제거
            ClearResultItems();

            // 각 미니게임 결과 표시
            for (int i = 0; i < results.Count; i++)
            {
                CreateResultItem(i + 1, results[i]);
            }

            // 평균 점수 계산
            float totalScore = 0f;
            foreach (var result in results)
            {
                totalScore += result.score;
            }
            float averageScore = totalScore / results.Count;

            // 총점 표시
            if (totalScoreText != null)
            {
                totalScoreText.text = $"평균 점수: {averageScore:F1}점";
            }

            // 최종 랭크 표시
            if (finalRankText != null)
            {
                finalRankText.text = $"최종 등급: {finalRank}";
                finalRankText.color = GetRankColor(finalRank);
            }

            // 총 수익 표시
            if (totalPriceText != null)
            {
                totalPriceText.text = $"판매 가격: {totalPrice}원";
            }

            // 패널 활성화 및 애니메이션
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
                resultPanel.transform.localScale = Vector3.one * 0.8f;
                canvasGroup.alpha = 0f;
                resultPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                canvasGroup.DOFade(1f, 0.3f);
            }
        }

        /// <summary>
        /// 결과 아이템 생성
        /// </summary>
        private void CreateResultItem(int index, MiniGameResult result)
        {
            if (resultItemPrefab == null || resultItemContainer == null) return;

            GameObject itemObj = Instantiate(resultItemPrefab, resultItemContainer);
            var itemUI = itemObj.GetComponent<MinigameResultItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Setup(index, result);
            }
        }

        /// <summary>
        /// 기존 결과 아이템 제거
        /// </summary>
        private void ClearResultItems()
        {
            if (resultItemContainer == null) return;

            foreach (Transform child in resultItemContainer)
            {
                Destroy(child.gameObject);
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

        /// <summary>
        /// 확인 버튼 클릭
        /// </summary>
        private void OnConfirmClicked()
        {
            Hide();
            onConfirm?.Invoke();
        }

        /// <summary>
        /// UI 숨기기
        /// </summary>
        public void Hide()
        {
            if (resultPanel != null)
            {
                resultPanel.transform.DOScale(0.8f, 0.2f);
                canvasGroup.DOFade(0f, 0.2f).OnComplete(() => resultPanel.SetActive(false));
            }
        }
    }
}

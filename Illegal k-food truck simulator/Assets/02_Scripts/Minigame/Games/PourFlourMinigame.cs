using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Minigame
{
    /// <summary>밀가루 담기 미니게임 - Hold to Fill</summary>
    public class PourFlourMinigame : MinigameBase
    {
        [Header("References")]
        public PourFlourParameters parameters;
        public Transform flourBagTransform;     // 밀가루 포대
        public Image fillBar;                   // 채우기 바 (빨간색)
        public Image targetZone;                // 목표 구역 표시 (초록색)
        public ParticleSystem flourParticles;   // 밀가루 파티클
        public TextMeshProUGUI countdownText;
        public TextMeshProUGUI instructionText;
        public TextMeshProUGUI amountText;
        public GameObject resultPanel;
        public TextMeshProUGUI resultScoreText;
        public TextMeshProUGUI resultRankText;
        // 결과 카운트다운/스킵 안내 텍스트 (결과 패널에 새로 추가하여 인스펙터에서 할당)
        public TextMeshProUGUI resultCountdownText;
        public TextMeshProUGUI resultSkipText;

        [Header("UI 설정")]
        [Tooltip("바의 최대값")]
        public float maxBarValue = 100f;
        [Tooltip("목표값 (초록색 구간)")]
        public float targetValue = 60f;
        [Tooltip("목표 구간 너비")]
        public float targetZoneWidth = 5f;

        private float currentAmount;
        private bool isPouring;
        private float pouringTime;
        private Quaternion originalBagRotation;
        private MiniGameResult gameResult;

        protected override void OnEnter()
        {
            resultPanel.SetActive(false);
            currentAmount = 0f;
            isPouring = false;
            pouringTime = 0f;
            
            if (flourBagTransform != null)
                originalBagRotation = flourBagTransform.localRotation;
            
            if (flourParticles != null)
                flourParticles.Stop();
            
            // 바 초기화
            if (fillBar != null)
            {
                fillBar.fillAmount = 0f;
            }
            
            ChangeState(MinigameState.Prepare);
        }

        protected override void OnPrepare()
        {
            countdownText.gameObject.SetActive(true);
            instructionText.gameObject.SetActive(false);
            
            // 목표 구역 위치 설정
            if (targetZone != null && fillBar != null)
            {
                // 목표값의 위치 계산 (0-100 중 60)
                float targetPosition = targetValue / maxBarValue;
                
                // 목표 구간의 너비 계산
                float zoneWidth = targetZoneWidth / maxBarValue;
                
                // RectTransform을 이용해 목표 구간 위치 설정
                RectTransform targetRect = targetZone.GetComponent<RectTransform>();
                RectTransform fillBarRect = fillBar.GetComponent<RectTransform>();
                
                if (targetRect != null && fillBarRect != null)
                {
                    // 목표 구간을 60 위치에 배치
                    targetRect.anchorMin = new Vector2(targetPosition - zoneWidth / 2f, 0);
                    targetRect.anchorMax = new Vector2(targetPosition + zoneWidth / 2f, 1);
                    targetRect.offsetMin = Vector2.zero;
                    targetRect.offsetMax = Vector2.zero;
                }
            }
        }

        protected override void UpdatePrepare()
        {
            float remaining = parameters.prepareTime - stateTimer;
            if (remaining > 0)
            {
                countdownText.text = Mathf.CeilToInt(remaining).ToString();
            }
            else
            {
                countdownText.gameObject.SetActive(false);
                ChangeState(MinigameState.Play);
            }
        }

        protected override void OnPlayStart()
        {
            instructionText.gameObject.SetActive(true);
            instructionText.text = "스페이스 바를 눌러 밀가루를 부으세요!";
        }

        protected override void UpdatePlay()
        {
            // 스페이스 홀드 감지
            bool holding = Input.GetKey(KeyCode.Space);

            if (holding)
            {
                if (!isPouring)
                {
                    isPouring = true;
                    pouringTime = 0f;
                    if (flourParticles != null)
                        flourParticles.Play();
                }

                pouringTime += Time.unscaledDeltaTime;
                
                // 유량 계산 (커브 적용)
                float flowMultiplier = parameters.flowCurve.Evaluate(Mathf.Clamp01(pouringTime / 2f));
                float flow = parameters.flowRate * flowMultiplier * Time.unscaledDeltaTime;
                currentAmount += flow;
                
                // 최대값 제한
                currentAmount = Mathf.Min(currentAmount, maxBarValue);

                // 포대 기울이기
                if (flourBagTransform != null)
                {
                    flourBagTransform.localRotation = Quaternion.Lerp(
                        originalBagRotation,
                        originalBagRotation * Quaternion.Euler(-90, 0, 0),
                        0.3f
                    );
                }
            }
            else
            {
                if (isPouring)
                {
                    isPouring = false;
                    if (flourParticles != null)
                        flourParticles.Stop();
                    
                    // 포대 원위치
                    if (flourBagTransform != null)
                    {
                        flourBagTransform.localRotation = Quaternion.Lerp(
                            flourBagTransform.localRotation,
                            originalBagRotation,
                            0.3f
                        );
                    }
                }
            }

            // UI 업데이트 (0-100 범위)
            if (fillBar != null)
            {
                fillBar.fillAmount = Mathf.Clamp01(currentAmount / maxBarValue);
            }

            if (amountText != null)
            {
                amountText.text = $"{currentAmount:F1} / {targetValue:F0}";
            }

            // 완료 조건: 스페이스를 뗐을 때 판정
            if (!holding && currentAmount > 0 && Input.GetKeyUp(KeyCode.Space))
            {
                ChangeState(MinigameState.Judge);
            }

            // 타임 아웃
            if (GetPlayDuration() >= parameters.maxPlayTime)
            {
                ChangeState(MinigameState.Judge);
            }
        }

        protected override void OnJudge()
        {
            // 목표량과의 오차 계산 (목표: 60)
            float error = Mathf.Abs(currentAmount - targetValue);
            
            // 허용 범위 내 점수 계산
            float normalizedError = Mathf.Clamp01(error / parameters.toleranceRange);

            // 오버슈트 추가 감점
            if (currentAmount > targetValue + parameters.toleranceRange)
            {
                float overshoot = currentAmount - (targetValue + parameters.toleranceRange);
                normalizedError += overshoot / parameters.toleranceRange * parameters.overshootPenalty;
                normalizedError = Mathf.Clamp01(normalizedError);
            }

            float accuracy = 1f - normalizedError;
            float score = parameters.scoringCurve.Evaluate(accuracy);

            gameResult = new MiniGameResult(score, GetPlayDuration(), isAborted);
            
            ChangeState(MinigameState.Result);
        }

        protected override void OnResult()
        {
            instructionText.gameObject.SetActive(false);
            if (flourParticles != null)
                flourParticles.Stop();
            
            resultPanel.SetActive(true);
            resultScoreText.text = $"점수: {gameResult.score:F1}";
            resultRankText.text = $"등급: {gameResult.rank}";
            if (resultCountdownText != null)
            {
                resultCountdownText.gameObject.SetActive(true);
                resultCountdownText.text = $"다음 게임까지: {parameters.resultDisplayTime:F1}초";
            }
            if (resultSkipText != null)
            {
                resultSkipText.gameObject.SetActive(true);
                resultSkipText.text = "스페이스바로 스킵";
            }
        }

        protected override void UpdateResult()
        {
            float remaining = parameters.resultDisplayTime - stateTimer;
            if (resultCountdownText != null)
            {
                resultCountdownText.text = $"다음 게임까지: {Mathf.Max(0f, remaining):F1}초";
            }
            // 스페이스바로 즉시 스킵
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeState(MinigameState.Cleanup);
                return;
            }
            // 자동 진행
            if (stateTimer >= parameters.resultDisplayTime)
            {
                ChangeState(MinigameState.Cleanup);
            }
        }

        protected override void OnCleanup()
        {
            if (isAborted)
            {
                gameResult = new MiniGameResult(0f, GetPlayDuration(), true);
            }
            FinishMinigame(gameResult);
        }

        protected override void OnAbort()
        {
            if (flourParticles != null)
                flourParticles.Stop();
            instructionText.text = "중단됨!";
        }
    }
}
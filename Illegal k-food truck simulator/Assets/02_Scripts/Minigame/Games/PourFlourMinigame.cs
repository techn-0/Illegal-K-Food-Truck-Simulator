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
        public Image fillBar;                   // 채우기 바
        public Image targetZone;                // 목표 구역 표시
        public ParticleSystem flourParticles;   // 밀가루 파티클
        public TextMeshProUGUI countdownText;
        public TextMeshProUGUI instructionText;
        public TextMeshProUGUI amountText;
        public GameObject resultPanel;
        public TextMeshProUGUI resultScoreText;
        public TextMeshProUGUI resultRankText;

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
            
            ChangeState(MinigameState.Prepare);
        }

        protected override void OnPrepare()
        {
            countdownText.gameObject.SetActive(true);
            instructionText.gameObject.SetActive(false);
            
            // 목표 구역 설정
            if (targetZone != null && fillBar != null)
            {
                float targetNormalized = parameters.targetAmount / (parameters.targetAmount + parameters.toleranceRange * 2);
                targetZone.fillAmount = parameters.toleranceRange * 2 / (parameters.targetAmount + parameters.toleranceRange * 2);
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

                // 포대 기울이기
                if (flourBagTransform != null)
                {
                    flourBagTransform.localRotation = Quaternion.Lerp(
                        originalBagRotation,
                        originalBagRotation * Quaternion.Euler(60, 0, 0),
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

            // UI 업데이트
            if (fillBar != null)
            {
                float maxAmount = parameters.targetAmount + parameters.toleranceRange * 2;
                fillBar.fillAmount = Mathf.Clamp01(currentAmount / maxAmount);
            }

            if (amountText != null)
            {
                amountText.text = $"{currentAmount:F1} / {parameters.targetAmount:F0}";
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
            // 목표량과의 오차 계산
            float error = Mathf.Abs(currentAmount - parameters.targetAmount);
            
            // 허용 범위 내 점수 계산
            float normalizedError = Mathf.Clamp01(error / parameters.toleranceRange);

            // 오버슈트 추가 감점
            if (currentAmount > parameters.targetAmount + parameters.toleranceRange)
            {
                float overshoot = currentAmount - (parameters.targetAmount + parameters.toleranceRange);
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
        }

        protected override void UpdateResult()
        {
            if (stateTimer >= parameters.resultDisplayTime)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    ChangeState(MinigameState.Cleanup);
                }
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


using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Minigame
{
    /// <summary>튀기기 미니게임 - Precise Release</summary>
    public class DeepFryMinigame : MinigameBase
    {
        [Header("References")]
        public DeepFryParameters parameters;
        public Transform basketTransform;       // 튀김 바구니
        public Transform oilTransform;          // 기름
        public ParticleSystem bubbleParticles;  // 거품 파티클
        public TextMeshProUGUI countdownText;
        public TextMeshProUGUI instructionText;
        public TextMeshProUGUI timerText;       // 타이머 표시
        public GameObject resultPanel;
        public TextMeshProUGUI resultScoreText;
        public TextMeshProUGUI resultRankText;

        private bool isFrying;
        private float fryingStartTime;
        private float releaseTime;
        private Vector3 basketOriginalPos;
        private Vector3 basketFryingPos;
        private MiniGameResult gameResult;

        protected override void OnEnter()
        {
            resultPanel.SetActive(false);
            isFrying = false;
            releaseTime = 0f;
            
            if (basketTransform != null)
            {
                basketOriginalPos = basketTransform.localPosition;
                basketFryingPos = basketOriginalPos + Vector3.down * 2f; // 기름에 담긴 위치
            }
            
            if (bubbleParticles != null)
                bubbleParticles.Stop();
            
            ChangeState(MinigameState.Prepare);
        }

        protected override void OnPrepare()
        {
            countdownText.gameObject.SetActive(true);
            instructionText.gameObject.SetActive(false);
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
            instructionText.text = "스페이스 바를 눌러 튀기기 시작!\n정확히 10초에 떼세요!";
        }

        protected override void UpdatePlay()
        {
            // 스페이스 홀드로 튀기기 시작
            if (Input.GetKeyDown(KeyCode.Space) && !isFrying)
            {
                isFrying = true;
                fryingStartTime = Time.unscaledTime;
                
                // 바구니를 기름에 담그기
                if (basketTransform != null)
                {
                    basketTransform.localPosition = basketFryingPos;
                }
                
                if (bubbleParticles != null)
                    bubbleParticles.Play();
                
                instructionText.text = $"튀기는 중... {parameters.targetTime}초에 떼세요!";
            }

            // 튀기는 중
            if (isFrying)
            {
                float currentFryTime = Time.unscaledTime - fryingStartTime;

                // 타이머 UI 업데이트
                if (timerText != null)
                {
                    timerText.text = $"{currentFryTime:F2}초";
                    
                    // 목표 시간에 가까워지면 색상 변경
                    if (Mathf.Abs(currentFryTime - parameters.targetTime) < 0.5f)
                    {
                        timerText.color = Color.yellow;
                    }
                    else if (Mathf.Abs(currentFryTime - parameters.targetTime) < 0.2f)
                    {
                        timerText.color = Color.green;
                    }
                    else
                    {
                        timerText.color = Color.white;
                    }
                }

                // 스페이스를 떼면 판정
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    releaseTime = currentFryTime;
                    ChangeState(MinigameState.Judge);
                }

                // 최대 대기 시간 초과
                if (currentFryTime >= parameters.maxWaitTime)
                {
                    releaseTime = currentFryTime;
                    ChangeState(MinigameState.Judge);
                }
            }
        }

        protected override void OnJudge()
        {
            // 목표 시간과의 오차 계산
            float error = Mathf.Abs(releaseTime - parameters.targetTime);
            
            float score;

            // 가우시안 분포 기반 점수 계산
            // f(x) = 100 * exp(-(x^2) / (2*sigma^2))
            float gaussianValue = Mathf.Exp(-(error * error) / (2f * parameters.sigma * parameters.sigma));
            score = 100f * gaussianValue;

            // 너무 빨리 뗀 경우 추가 감점
            if (releaseTime < parameters.targetTime - parameters.perfectRange)
            {
                float earlyError = parameters.targetTime - releaseTime;
                score *= 1f / parameters.earlyReleasePenalty;
            }

            // 완벽 판정 범위
            if (error <= parameters.perfectRange)
            {
                score = 100f;
            }

            gameResult = new MiniGameResult(score, releaseTime, isAborted);
            
            ChangeState(MinigameState.Result);
        }

        protected override void OnResult()
        {
            instructionText.gameObject.SetActive(false);
            
            if (bubbleParticles != null)
                bubbleParticles.Stop();
            
            // 바구니 올리기
            if (basketTransform != null)
            {
                basketTransform.localPosition = basketOriginalPos;
            }
            
            resultPanel.SetActive(true);
            resultScoreText.text = $"점수: {gameResult.score:F1}\n시간: {releaseTime:F2}초";
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
                gameResult = new MiniGameResult(0f, releaseTime, true);
            }
            FinishMinigame(gameResult);
        }

        protected override void OnAbort()
        {
            if (bubbleParticles != null)
                bubbleParticles.Stop();
            instructionText.text = "중단됨!";
        }
    }
}


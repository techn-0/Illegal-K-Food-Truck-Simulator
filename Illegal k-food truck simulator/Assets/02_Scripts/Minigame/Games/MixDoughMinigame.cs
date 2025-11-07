using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Minigame
{
    /// <summary>반죽 섞기 미니게임 - Circular Motion</summary>
    public class MixDoughMinigame : MinigameBase
    {
        [Header("References")]
        public MixDoughParameters parameters;
        public Transform doughBowlTransform;    // 반죽 볼
        public Transform stirrerTransform;      // 젓는 도구
        public Image progressBar;               // 진행도 바
        public TextMeshProUGUI countdownText;
        public TextMeshProUGUI instructionText;
        public TextMeshProUGUI rotationText;    // 회전 수 표시
        public TextMeshProUGUI timerText;       // 타이머 표시
        public GameObject resultPanel;
        public TextMeshProUGUI resultScoreText;
        public TextMeshProUGUI resultRankText;

        private Vector2 lastMousePos;
        private float totalRotation;            // 누적 각도
        private int completedRotations;         // 완료된 회전 수
        private float currentRotationAngle;     // 현재 회전 중인 각도
        private bool isDragging;
        private Vector2 centerScreenPos;
        private MiniGameResult gameResult;

        protected override void OnEnter()
        {
            resultPanel.SetActive(false);
            totalRotation = 0f;
            completedRotations = 0;
            currentRotationAngle = 0f;
            isDragging = false;
            
            // 화면 중앙 계산
            centerScreenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
            
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
            instructionText.text = "마우스로 원을 그려 반죽을 섞으세요!";
            lastMousePos = Input.mousePosition;
        }

        protected override void UpdatePlay()
        {
            // 마우스 클릭 감지
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector2 currentMousePos = Input.mousePosition;
                
                // 중앙 기준 각도 계산
                Vector2 lastDir = (lastMousePos - centerScreenPos).normalized;
                Vector2 currentDir = (currentMousePos - centerScreenPos).normalized;

                // 각도 차이 계산
                float angle = Vector2.SignedAngle(lastDir, currentDir);
                
                // 각도 누적
                currentRotationAngle += angle * parameters.mouseSensitivity;

                // 한 바퀴 완료 체크
                if (Mathf.Abs(currentRotationAngle) >= parameters.minRotationAngle)
                {
                    if (currentRotationAngle < 0)  // 음수(시계 반대 방향)가 정방향
                    {
                        completedRotations++;
                        currentRotationAngle = 0f;
                    }
                    else // 역회전 (양수, 시계 방향)
                    {
                        completedRotations = Mathf.Max(0, 
                            completedRotations - Mathf.RoundToInt(parameters.reverseRotationPenalty));
                        currentRotationAngle = 0f;
                    }
                }

                // 반죽 볼 회전 시각화
                if (doughBowlTransform != null)
                {
                    doughBowlTransform.Rotate(Vector3.up, -angle * parameters.mouseSensitivity * 2f, Space.Self);
                }

                lastMousePos = currentMousePos;
            }

            // UI 업데이트
            if (progressBar != null)
            {
                progressBar.fillAmount = (float)completedRotations / parameters.targetRotations;
            }

            if (rotationText != null)
            {
                rotationText.text = $"회전: {completedRotations} / {parameters.targetRotations}";
            }

            if (timerText != null)
            {
                float remaining = parameters.timeLimit - GetPlayDuration();
                timerText.text = $"시간: {Mathf.Max(0, remaining):F1}초";
            }

            // 성공 조건
            if (completedRotations >= parameters.targetRotations)
            {
                ChangeState(MinigameState.Judge);
            }

            // 타임 아웃
            if (GetPlayDuration() >= parameters.timeLimit)
            {
                ChangeState(MinigameState.Judge);
            }
        }

        protected override void OnJudge()
        {
            float duration = GetPlayDuration();
            float score;

            // 목표 회전 수 달성 여부
            if (completedRotations >= parameters.targetRotations)
            {
                // 시간에 따른 점수 계산
                if (duration <= parameters.perfectClearTime)
                {
                    score = 100f; // 완벽
                }
                else
                {
                    float overtime = duration - parameters.perfectClearTime;
                    float penalty = overtime * parameters.latePenaltyPerSecond;
                    score = Mathf.Max(60f, 100f - penalty); // 성공은 최소 60점
                }
            }
            else
            {
                // 실패 - 회전 수에 비례한 점수
                float completionRatio = (float)completedRotations / parameters.targetRotations;
                score = completionRatio * 50f; // 최대 50점
            }

            gameResult = new MiniGameResult(score, duration, isAborted);
            
            ChangeState(MinigameState.Result);
        }

        protected override void OnResult()
        {
            instructionText.gameObject.SetActive(false);
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
            instructionText.text = "중단됨!";
        }
    }
}

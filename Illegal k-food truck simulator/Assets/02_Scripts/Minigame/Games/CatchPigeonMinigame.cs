using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Minigame
{
    /// <summary>비둘기 잡기 미니게임 - 타이밍 바</summary>
    public class CatchPigeonMinigame : MinigameBase
    {
        [Header("References")]
        public CatchPigeonParameters parameters;
        public Transform barTransform;          // 왕복하는 바
        public Transform pigeonTransform;       // 비둘기 (중앙)
        public TextMeshProUGUI countdownText;
        public TextMeshProUGUI instructionText;
        public GameObject resultPanel;
        public TextMeshProUGUI resultScoreText;
        public TextMeshProUGUI resultRankText;
        // 결과 카운트다운/스킵 안내 텍스트 (결과 패널에 새로 추가하여 인스펙터에서 할당)
        public TextMeshProUGUI resultCountdownText;
        public TextMeshProUGUI resultSkipText;

        private float barPosition;              // -1 ~ 1
        private float barTime;
        private bool hasPressed;
        private float pressedPosition;
        private MiniGameResult gameResult;

        protected override void OnEnter()
        {
            // 카메라, 조명 초기화는 프리팹에 이미 설정되어 있음
            resultPanel.SetActive(false);
            instructionText.gameObject.SetActive(false);
            hasPressed = false;
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
            instructionText.text = "스페이스 바를 눌러 비둘기를 잡으세요!";
            barTime = 0f;
        }

        protected override void UpdatePlay()
        {
            // 바 이동 (AnimationCurve 또는 sin)
            barTime += Time.unscaledDeltaTime * parameters.barSpeed;
            float normalizedTime = Mathf.Repeat(barTime, 1f);
            barPosition = parameters.barMovementCurve.Evaluate(normalizedTime);

            // 바 위치 업데이트
            if (barTransform != null)
            {
                Vector3 pos = barTransform.localPosition;
                pos.x = barPosition * parameters.barRange;
                barTransform.localPosition = pos;
            }

            // 스페이스 입력
            if (Input.GetKeyDown(KeyCode.Space) && !hasPressed)
            {
                hasPressed = true;
                pressedPosition = barPosition;
                ChangeState(MinigameState.Judge);
            }

            // 타임 아웃
            if (parameters.playTimeLimit > 0 && GetPlayDuration() >= parameters.playTimeLimit)
            {
                hasPressed = true;
                pressedPosition = barPosition;
                ChangeState(MinigameState.Judge);
            }
        }

        protected override void OnJudge()
        {
            // 비둘기 위치 가져오기 (pigeonTransform이 없으면 중앙(0) 사용)
            float pigeonPosition = 0f;
            if (pigeonTransform != null)
            {
                pigeonPosition = pigeonTransform.localPosition.x / parameters.barRange;
            }
            
            // 비둘기 기준 거리 계산
            float distance = Mathf.Abs(pressedPosition - pigeonPosition);
            
            // 정규화 (0=완벽, 1=최악) - 최대 거리는 2 (한쪽 끝에서 반대쪽 끝까지)
            float normalizedError = Mathf.Clamp01(distance / 2f);

            // 정확도 계산 (1=완벽, 0=최악)
            float accuracy = 1f - normalizedError;
            
            // 감마 커브 적용하여 점수 계산을 더 엄격하게
            float adjustedAccuracy = Mathf.Pow(accuracy, parameters.scoreGamma);

            // 점수 계산
            float score = parameters.scoringCurve.Evaluate(adjustedAccuracy);

            gameResult = new MiniGameResult(score, GetPlayDuration(), isAborted);
            
            ChangeState(MinigameState.Result);
        }

        protected override void OnResult()
        {
            instructionText.gameObject.SetActive(false);
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeState(MinigameState.Cleanup);
                return;
            }
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
            instructionText.text = "중단됨!";
        }
    }
}

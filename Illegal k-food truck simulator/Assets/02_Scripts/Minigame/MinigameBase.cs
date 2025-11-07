using System;
using UnityEngine;

namespace Minigame
{
    /// <summary>미니게임 베이스 클래스 - 상태 머신 구현</summary>
    public abstract class MinigameBase : MonoBehaviour, IMinigame
    {
        protected MinigameState currentState = MinigameState.None;
        protected Action<MiniGameResult> onFinishedCallback;
        protected float stateTimer;
        protected float playStartTime;
        protected bool isAborted;

        public void Begin(Action<MiniGameResult> onFinished)
        {
            onFinishedCallback = onFinished;
            isAborted = false;
            ChangeState(MinigameState.Enter);
        }

        public void Abort()
        {
            isAborted = true;
            OnAbort();
            ChangeState(MinigameState.Cleanup);
        }

        protected virtual void Update()
        {
            if (currentState == MinigameState.None) return;

            stateTimer += Time.unscaledDeltaTime;

            switch (currentState)
            {
                case MinigameState.Enter:
                    UpdateEnter();
                    break;
                case MinigameState.Prepare:
                    UpdatePrepare();
                    break;
                case MinigameState.Play:
                    UpdatePlay();
                    break;
                case MinigameState.Judge:
                    UpdateJudge();
                    break;
                case MinigameState.Result:
                    UpdateResult();
                    break;
                case MinigameState.Cleanup:
                    UpdateCleanup();
                    break;
            }

            // ESC 키로 중단
            if (Input.GetKeyDown(KeyCode.Escape) && !isAborted)
            {
                Abort();
            }
        }

        protected void ChangeState(MinigameState newState)
        {
            ExitState(currentState);
            currentState = newState;
            stateTimer = 0f;
            EnterState(newState);
        }

        protected virtual void EnterState(MinigameState state)
        {
            switch (state)
            {
                case MinigameState.Enter:
                    OnEnter();
                    break;
                case MinigameState.Prepare:
                    OnPrepare();
                    break;
                case MinigameState.Play:
                    playStartTime = Time.unscaledTime;
                    OnPlayStart();
                    break;
                case MinigameState.Judge:
                    OnJudge();
                    break;
                case MinigameState.Result:
                    OnResult();
                    break;
                case MinigameState.Cleanup:
                    OnCleanup();
                    break;
            }
        }

        protected virtual void ExitState(MinigameState state) { }

        // 상태별 업데이트 (하위 클래스에서 필요시 오버라이드)
        protected virtual void UpdateEnter() { }
        protected virtual void UpdatePrepare() { }
        protected virtual void UpdatePlay() { }
        protected virtual void UpdateJudge() { }
        protected virtual void UpdateResult() { }
        protected virtual void UpdateCleanup() { }

        // 상태별 진입 이벤트 (하위 클래스에서 구현)
        protected abstract void OnEnter();
        protected abstract void OnPrepare();
        protected abstract void OnPlayStart();
        protected abstract void OnJudge();
        protected abstract void OnResult();
        protected abstract void OnCleanup();
        protected virtual void OnAbort() { }

        /// <summary>결과를 반환하고 정리</summary>
        protected void FinishMinigame(MiniGameResult result)
        {
            onFinishedCallback?.Invoke(result);
            Destroy(gameObject);
        }

        /// <summary>플레이 경과 시간</summary>
        protected float GetPlayDuration()
        {
            return Time.unscaledTime - playStartTime;
        }
    }
}


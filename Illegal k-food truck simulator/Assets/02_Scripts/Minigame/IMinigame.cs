using System;

namespace Minigame
{
    /// <summary>미니게임 인터페이스</summary>
    public interface IMinigame
    {
        /// <summary>미니게임 시작</summary>
        /// <param name="onFinished">완료 시 콜백</param>
        void Begin(Action<MiniGameResult> onFinished);

        /// <summary>미니게임 중단</summary>
        void Abort();
    }

    /// <summary>미니게임 상태</summary>
    public enum MinigameState
    {
        None,       // 초기 상태
        Enter,      // 프리팹 생성, 카메라/조명 초기화
        Prepare,    // UI 초기화, 카운트다운
        Play,       // 메인 로직, 입력 처리
        Judge,      // 오차 계산, 점수 산출
        Result,     // 결과 연출, 점수 표시
        Cleanup     // 프리팹 삭제, 입력 복귀
    }
}


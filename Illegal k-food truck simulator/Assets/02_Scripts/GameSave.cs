using System;

/// <summary>
/// 저장/로드할 게임 데이터 구조
/// </summary>
[Serializable]
public class GameSave
{
    public int currentDay = 1;
    public int money;
    public int todayEarnings;  // 오늘 하루 매출
    public int totalEarnings;  // 누적 총 매출
    public bool tutorialCompleted;
}

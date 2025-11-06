using System;
using System.Collections.Generic;

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
    
    // 해금된 레시피 ID 목록
    public List<string> unlockedRecipeIds = new List<string>();
    
    // 인벤토리 슬롯 데이터
    public List<InventorySlotData> inventorySlots = new List<InventorySlotData>();
}

/// <summary>
/// 인벤토리 슬롯 저장 데이터
/// </summary>
[Serializable]
public class InventorySlotData
{
    public string itemId;
    public int count;
    
    public InventorySlotData(string itemId, int count)
    {
        this.itemId = itemId;
        this.count = count;
    }
}

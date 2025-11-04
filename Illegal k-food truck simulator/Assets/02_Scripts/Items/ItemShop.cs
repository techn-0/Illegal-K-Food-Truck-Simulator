using UnityEngine;

/// <summary>
/// 상점에서 판매할 아이템 정보
/// </summary>
[System.Serializable]
public class ShopItemData
{
    [SerializeField] private ItemDefinition item;
    [SerializeField] private int price;
    [SerializeField] private int sellAmount = 1; // 한 번에 판매하는 수량
    
    public ItemDefinition Item => item;
    public int Price => price;
    public int SellAmount => sellAmount;
}

/// <summary>
/// 아이템 상점의 데이터를 관리하는 클래스
/// 단일 책임: 상점에서 판매할 아이템과 가격 정보 관리
/// </summary>
public class ItemShop : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private ShopItemData[] shopItems; // 상점에서 판매할 아이템들
    
    [Header("Inventory Reference")]
    [SerializeField] private Inventory targetInventory; // 인스펙터에서 할당할 대상 인벤토리
    
    /// <summary>
    /// 상점에서 판매하는 모든 아이템 반환
    /// </summary>
    public ShopItemData[] GetShopItems()
    {
        return shopItems;
    }
    
    /// <summary>
    /// 아이템 구매 시도
    /// </summary>
    /// <param name="shopItemData">구매할 상점 아이템 데이터</param>
    /// <returns>구매 성공 여부</returns>
    public bool TryPurchaseItem(ShopItemData shopItemData)
    {
        if (shopItemData?.Item == null) return false;
        
        // 돈이 충분한지 확인
        if (!PlayerMoneyManager.Instance.CanAfford(shopItemData.Price))
        {
            Debug.LogWarning($"골드가 부족합니다. 필요: {shopItemData.Price}골드");
            return false;
        }
        
        // 인벤토리에 공간이 있는지 확인
        Inventory playerInventory = targetInventory != null ? targetInventory : FindPlayerInventory();
        if (playerInventory == null)
        {
            Debug.LogError("플레이어 인벤토리를 찾을 수 없습니다.");
            return false;
        }
        
        // 인벤토리에 아이템을 추가할 수 있는지 확인
        int canAdd = CanAddToInventory(playerInventory, shopItemData.Item, shopItemData.SellAmount);
        if (canAdd < shopItemData.SellAmount)
        {
            Debug.LogWarning($"인벤토리 공간이 부족합니다. 필요 공간: {shopItemData.SellAmount}, 가능 공간: {canAdd}");
            return false;
        }
        
        // 돈 차감 (0원인 경우 건너뜀)
        if (shopItemData.Price > 0)
        {
            if (!PlayerMoneyManager.Instance.SpendMoney(shopItemData.Price))
            {
                Debug.LogError("돈 차감 중 오류가 발생했습니다.");
                return false;
            }
        }
        
        // 인벤토리에 아이템 추가
        int actualAdded = playerInventory.Add(shopItemData.Item, shopItemData.SellAmount);
        if (actualAdded != shopItemData.SellAmount)
        {
            // 예상과 다른 수량이 추가됨 - 돈 환불 (0원이 아닌 경우에만)
            if (shopItemData.Price > 0)
            {
                PlayerMoneyManager.Instance.AddMoney(shopItemData.Price);
            }
            Debug.LogError("인벤토리 추가 중 오류가 발생했습니다.");
            return false;
        }
        
        Debug.Log($"아이템 구매 완료: {shopItemData.Item.DisplayName} x{shopItemData.SellAmount} ({shopItemData.Price}골드)");
        return true;
    }
    
    /// <summary>
    /// 특정 아이템이 구매 가능한지 확인
    /// </summary>
    /// <param name="shopItemData">확인할 상점 아이템 데이터</param>
    /// <returns>구매 가능 여부</returns>
    public bool CanPurchaseItem(ShopItemData shopItemData)
    {
        if (shopItemData?.Item == null) return false;
        
        // 돈이 충분한지 확인
        if (!PlayerMoneyManager.Instance.CanAfford(shopItemData.Price)) return false;
        
        // 인벤토리 공간 확인
        Inventory playerInventory = targetInventory != null ? targetInventory : FindPlayerInventory();
        if (playerInventory == null) return false;
        
        int canAdd = CanAddToInventory(playerInventory, shopItemData.Item, shopItemData.SellAmount);
        return canAdd >= shopItemData.SellAmount;
    }
    
    /// <summary>
    /// 플레이어 인벤토리 찾기
    /// </summary>
    private Inventory FindPlayerInventory()
    {
        // 싱글톤 인스턴스 사용
        return Inventory.Instance;
    }
    
    /// <summary>
    /// 대상 인벤토리 설정
    /// </summary>
    /// <param name="inventory">설정할 인벤토리</param>
    public void SetTargetInventory(Inventory inventory)
    {
        targetInventory = inventory;
    }
    
    /// <summary>
    /// 현재 대상 인벤토리 반환
    /// </summary>
    /// <returns>현재 대상 인벤토리</returns>
    public Inventory GetTargetInventory()
    {
        return targetInventory != null ? targetInventory : FindPlayerInventory();
    }
    
    /// <summary>
    /// 인벤토리에 추가 가능한 아이템 수량 계산
    /// </summary>
    private int CanAddToInventory(Inventory inventory, ItemDefinition item, int amount)
    {
        if (inventory == null || item == null) return 0;
        
        int canAdd = 0;
        
        // 기존 슬롯에 추가 가능한 수량 계산
        foreach (var slot in inventory.Slots)
        {
            if (slot.IsSameItem(item) && !slot.IsFull)
            {
                int spaceInSlot = item.MaxStack - slot.Count;
                canAdd += spaceInSlot;
            }
        }
        
        // 빈 슬롯에 추가 가능한 수량 계산
        foreach (var slot in inventory.Slots)
        {
            if (slot.IsEmpty)
            {
                canAdd += item.MaxStack;
            }
        }
        
        return Mathf.Min(canAdd, amount);
    }
}

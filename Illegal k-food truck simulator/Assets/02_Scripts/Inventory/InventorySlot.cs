using UnityEngine;

/// <summary>
/// 인벤토리 슬롯 - 하나의 아이템과 수량을 저장
/// </summary>
[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemDefinition item; // 저장된 아이템
    [SerializeField] private int count; // 아이템 수량

    /// <summary>
    /// 저장된 아이템 정보
    /// </summary>
    public ItemDefinition Item => item;

    /// <summary>
    /// 아이템 수량
    /// </summary>
    public int Count => count;

    /// <summary>
    /// 슬롯이 비어있는지 확인
    /// </summary>
    public bool IsEmpty => item == null || count <= 0;

    /// <summary>
    /// 슬롯이 가득 찼는지 확인 (최대 스택에 도달했는지)
    /// </summary>
    public bool IsFull => item != null && count >= item.MaxStack;

    /// <summary>
    /// 빈 슬롯 생성자
    /// </summary>
    public InventorySlot()
    {
        this.item = null;
        this.count = 0;
    }

    /// <summary>
    /// 아이템과 수량으로 슬롯 생성자
    /// </summary>
    public InventorySlot(ItemDefinition item, int count)
    {
        this.item = item;
        this.count = count;
    }

    /// <summary>
    /// 슬롯에 아이템 설정
    /// </summary>
    public void SetItem(ItemDefinition newItem, int newCount)
    {
        this.item = newItem;
        this.count = newCount;
    }

    /// <summary>
    /// 슬롯에 아이템 추가 (같은 아이템일 때만)
    /// </summary>
    /// <param name="amount">추가할 수량</param>
    /// <returns>실제로 추가된 수량</returns>
    public int AddCount(int amount)
    {
        if (item == null) return 0;

        int maxAddable = item.MaxStack - count;
        int actualAdded = Mathf.Min(amount, maxAddable);
        count += actualAdded;

        return actualAdded;
    }

    /// <summary>
    /// 슬롯에서 아이템 제거
    /// </summary>
    /// <param name="amount">제거할 수량</param>
    /// <returns>실제로 제거된 수량</returns>
    public int RemoveCount(int amount)
    {
        int actualRemoved = Mathf.Min(amount, count);
        count -= actualRemoved;

        // 수량이 0 이하가 되면 슬롯을 비움
        if (count <= 0)
        {
            Clear();
        }

        return actualRemoved;
    }

    /// <summary>
    /// 슬롯을 비움
    /// </summary>
    public void Clear()
    {
        item = null;
        count = 0;
    }

    /// <summary>
    /// 같은 아이템인지 확인
    /// </summary>
    public bool IsSameItem(ItemDefinition otherItem)
    {
        return item != null && item == otherItem;
    }
}

using UnityEngine;

/// <summary>
/// 인벤토리 아이템 추가 테스트를 위한 컴포넌트
/// 인스펙터에서 여러 아이템을 지정하여 테스트할 수 있습니다.
/// </summary>
public class InventoryTester : MonoBehaviour
{
    [System.Serializable]
    public class TestItem
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField] private int amount = 1;

        public ItemDefinition Item => item;
        public int Amount => amount;
    }

    [Header("테스트할 인벤토리")]
    [SerializeField] private Inventory targetInventory;

    [Header("추가할 아이템들")]
    [SerializeField] private TestItem[] testItems;

    [Header("테스트 버튼")]
    [Space(10)]
    public bool addAllItems;
    public bool clearInventory;

    private void OnValidate()
    {
        // 인스펙터에서 addAllItems 체크박스를 체크하면 모든 아이템 추가
        if (addAllItems)
        {
            addAllItems = false;
            AddAllTestItems();
        }

        // 인스펙터에서 clearInventory 체크박스를 체크하면 인벤토리 초기화
        if (clearInventory)
        {
            clearInventory = false;
            ClearInventory();
        }
    }

    /// <summary>
    /// 모든 테스트 아이템을 인벤토리에 추가
    /// </summary>
    [ContextMenu("모든 아이템 추가")]
    public void AddAllTestItems()
    {
        if (targetInventory == null)
        {
            Debug.LogWarning("타겟 인벤토리가 설정되지 않았습니다.");
            return;
        }

        if (testItems == null || testItems.Length == 0)
        {
            Debug.LogWarning("테스트할 아이템이 없습니다.");
            return;
        }

        foreach (var testItem in testItems)
        {
            if (testItem.Item != null && testItem.Amount > 0)
            {
                int addedAmount = targetInventory.Add(testItem.Item, testItem.Amount);
                Debug.Log($"아이템 추가: {testItem.Item.DisplayName} x{addedAmount} (요청: {testItem.Amount})");
            }
        }
    }

    /// <summary>
    /// 특정 인덱스의 아이템만 추가
    /// </summary>
    public void AddTestItem(int index)
    {
        if (targetInventory == null)
        {
            Debug.LogWarning("타겟 인벤토리가 설정되지 않았습니다.");
            return;
        }

        if (index < 0 || index >= testItems.Length)
        {
            Debug.LogWarning($"잘못된 인덱스: {index}");
            return;
        }

        var testItem = testItems[index];
        if (testItem.Item != null && testItem.Amount > 0)
        {
            int addedAmount = targetInventory.Add(testItem.Item, testItem.Amount);
            Debug.Log($"아이템 추가: {testItem.Item.DisplayName} x{addedAmount} (요청: {testItem.Amount})");
        }
    }

    /// <summary>
    /// 인벤토리를 초기화 (모든 슬롯 비우기)
    /// </summary>
    [ContextMenu("인벤토리 초기화")]
    public void ClearInventory()
    {
        if (targetInventory == null)
        {
            Debug.LogWarning("타겟 인벤토리가 설정되지 않았습니다.");
            return;
        }

        // 기존 Inventory 클래스에 Clear 메서드가 없다면 각 슬롯을 개별적으로 비움
        for (int i = 0; i < targetInventory.Slots.Count; i++)
        {
            var slot = targetInventory.Slots[i] as InventorySlot;
            slot?.Clear();
        }

        Debug.Log("인벤토리가 초기화되었습니다.");
    }

    /// <summary>
    /// 현재 인벤토리 상태를 로그로 출력
    /// </summary>
    [ContextMenu("인벤토리 상태 출력")]
    public void LogInventoryStatus()
    {
        if (targetInventory == null)
        {
            Debug.LogWarning("타겟 인벤토리가 설정되지 않았습니다.");
            return;
        }

        Debug.Log("=== 인벤토리 상태 ===");
        for (int i = 0; i < targetInventory.Slots.Count; i++)
        {
            var slot = targetInventory.Slots[i];
            if (!slot.IsEmpty)
            {
                Debug.Log($"슬롯 {i}: {slot.Item.DisplayName} x{slot.Count}");
            }
            else
            {
                Debug.Log($"슬롯 {i}: 비어있음");
            }
        }
    }
}

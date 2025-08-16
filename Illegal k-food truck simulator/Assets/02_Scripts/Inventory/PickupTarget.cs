using UnityEngine;

/// <summary>
/// 필드에 놓인 아이템을 플레이어가 픽업할 수 있게 하는 컴포넌트
/// 인벤토리에 아이템을 추가하고 성공 시 오브젝트를 파괴합니다.
/// </summary>
public class PickupTarget : MonoBehaviour
{
    [Header("픽업할 아이템 정보")]
    [SerializeField] private ItemDefinition item;
    [SerializeField] private int amount = 1;

    /// <summary>
    /// 픽업할 아이템 정의 (읽기 전용)
    /// </summary>
    public ItemDefinition Item => item;

    /// <summary>
    /// 픽업할 아이템 수량 (읽기 전용)
    /// </summary>
    public int Amount => amount;

    /// <summary>
    /// 아이템을 인벤토리에 픽업을 시도합니다.
    /// </summary>
    /// <param name="inventory">아이템을 추가할 인벤토리</param>
    /// <returns>픽업 성공 시 true, 실패 시 false</returns>
    public bool TryPickup(Inventory inventory)
    {
        // 유효성 검사
        if (inventory == null || item == null || amount <= 0)
        {
            return false;
        }

        // 인벤토리에 아이템 추가 시도
        int addedAmount = inventory.Add(item, amount);

        // 요청한 수량이 모두 추가되었다면 성공
        if (addedAmount == amount)
        {
            // 픽업 성공 - 오브젝트 파괴
            Destroy(gameObject);
            return true;
        }

        // 인벤토리가 가득 차서 일부만 추가되었거나 전혀 추가되지 않음
        return false;
    }

    /// <summary>
    /// 에디터에서 디버그 정보 표시
    /// </summary>
    private void OnValidate()
    {
        // 수량이 0 이하가 되지 않도록 보정
        if (amount <= 0)
        {
            amount = 1;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 인벤토리 슬롯의 UI 표시를 담당하는 클래스
/// </summary>
public class ItemSlotView : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private Image icon; // 아이템 아이콘 이미지
    [SerializeField] private TextMeshProUGUI countText; // 수량 텍스트
    [SerializeField] private int slotIndex; // 슬롯 인덱스

    private Inventory inventory; // 연결된 인벤토리

    /// <summary>
    /// 슬롯 인덱스 (읽기 전용)
    /// </summary>
    public int SlotIndex => slotIndex;

    /// <summary>
    /// 인벤토리와 슬롯 인덱스를 연결
    /// </summary>
    /// <param name="inventory">연결할 인벤토리</param>
    /// <param name="index">슬롯 인덱스</param>
    public void Bind(Inventory inventory, int index)
    {
        this.inventory = inventory;
        this.slotIndex = index;
        
        // 초기 표시 갱신
        Refresh();
    }

    /// <summary>
    /// 슬롯 UI를 현재 데이터에 맞게 갱신
    /// </summary>
    public void Refresh()
    {
        if (inventory == null || slotIndex < 0 || slotIndex >= inventory.Capacity)
        {
            // 유효하지 않은 상태일 때 빈 슬롯으로 표시
            ShowEmptySlot();
            return;
        }

        var slot = inventory.Slots[slotIndex];
        
        if (slot.IsEmpty)
        {
            ShowEmptySlot();
        }
        else
        {
            ShowItemSlot(slot);
        }
    }

    /// <summary>
    /// 빈 슬롯 상태로 UI 표시
    /// </summary>
    private void ShowEmptySlot()
    {
        if (icon != null)
        {
            icon.gameObject.SetActive(false);
        }
        
        if (countText != null)
        {
            countText.text = "";
        }
    }

    /// <summary>
    /// 아이템이 있는 슬롯 상태로 UI 표시
    /// </summary>
    /// <param name="slot">표시할 슬롯 데이터</param>
    private void ShowItemSlot(InventorySlot slot)
    {
        if (icon != null && slot.Item != null)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = slot.Item.Icon;
        }
        
        if (countText != null)
        {
            // 수량이 1개보다 많을 때만 숫자 표시
            countText.text = slot.Count > 1 ? slot.Count.ToString() : "";
        }
    }
}

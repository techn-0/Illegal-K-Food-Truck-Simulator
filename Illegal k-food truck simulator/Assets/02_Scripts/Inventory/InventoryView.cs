using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 인벤토리 UI 전체를 관리하는 클래스
/// </summary>
public class InventoryView : MonoBehaviour
{
    [Header("인벤토리 참조")]
    [SerializeField] private Inventory inventory; // 표시할 인벤토리

    [Header("UI 참조")]
    [SerializeField] private Transform gridRoot; // 슬롯들이 배치될 부모 Transform (GridLayoutGroup)
    [SerializeField] private ItemSlotView slotPrefab; // 슬롯 UI 프리팹

    private List<ItemSlotView> slotViews; // 생성된 슬롯 UI들

    /// <summary>
    /// 오브젝트 활성화 시 초기화 및 이벤트 구독
    /// </summary>
    private void OnEnable()
    {
        // 슬롯 UI 생성
        CreateSlotViews();
        
        // 인벤토리 변경 이벤트 구독
        if (inventory != null)
        {
            inventory.OnChanged += Refresh;
            // 초기 표시 갱신
            Refresh();
        }
    }

    /// <summary>
    /// 오브젝트 비활성화 시 이벤트 구독 해제
    /// </summary>
    private void OnDisable()
    {
        // 메모리 누수 방지를 위한 이벤트 구독 해제
        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
        }
    }

    /// <summary>
    /// 12개의 슬롯 UI 생성 및 GridLayoutGroup에 배치
    /// </summary>
    private void CreateSlotViews()
    {
        // 기존 슬롯들 정리
        ClearSlotViews();

        if (gridRoot == null || slotPrefab == null || inventory == null)
        {
            Debug.LogWarning("InventoryView: 필수 참조가 누락되었습니다.");
            return;
        }

        slotViews = new List<ItemSlotView>();

        // 인벤토리 용량만큼 슬롯 UI 생성
        for (int i = 0; i < inventory.Capacity; i++)
        {
            // 슬롯 프리팹 인스턴스 생성
            ItemSlotView slotView = Instantiate(slotPrefab, gridRoot);
            
            // 슬롯과 인벤토리 연결
            slotView.Bind(inventory, i);
            
            // 생성된 슬롯 리스트에 추가
            slotViews.Add(slotView);
        }
    }

    /// <summary>
    /// 기존 슬롯 UI들 제거
    /// </summary>
    private void ClearSlotViews()
    {
        if (slotViews != null)
        {
            foreach (var slotView in slotViews)
            {
                if (slotView != null)
                {
                    DestroyImmediate(slotView.gameObject);
                }
            }
            slotViews.Clear();
        }
    }

    /// <summary>
    /// 모든 슬롯 UI를 현재 인벤토리 데이터에 맞게 갱신
    /// </summary>
    public void Refresh()
    {
        if (slotViews == null) return;

        // 모든 슬롯 UI 갱신
        foreach (var slotView in slotViews)
        {
            if (slotView != null)
            {
                slotView.Refresh();
            }
        }
    }

    /// <summary>
    /// 인벤토리 참조 변경 (런타임에서 다른 인벤토리로 교체할 때 사용)
    /// </summary>
    /// <param name="newInventory">새로운 인벤토리</param>
    public void SetInventory(Inventory newInventory)
    {
        // 기존 이벤트 구독 해제
        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
        }

        // 새 인벤토리 설정
        inventory = newInventory;

        // 새 이벤트 구독 및 UI 재생성
        if (inventory != null && gameObject.activeInHierarchy)
        {
            inventory.OnChanged += Refresh;
            CreateSlotViews();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 시스템 - 12개의 고정 슬롯을 가진 아이템 저장소
/// </summary>
public class Inventory : MonoBehaviour
{
    private const int CAPACITY = 11; // 인벤토리 용량 고정값

    [SerializeField] private List<InventorySlot> slots; // 인벤토리 슬롯들

    /// <summary>
    /// 인벤토리 내용이 변경될 때 호출되는 이벤트
    /// </summary>
    public event Action OnChanged;

    /// <summary>
    /// 인벤토리 슬롯들 (읽기 전용)
    /// </summary>
    public IReadOnlyList<InventorySlot> Slots => slots;

    /// <summary>
    /// 인벤토리 용량
    /// </summary>
    public int Capacity => CAPACITY;

    /// <summary>
    /// 초기화 - 빈 슬롯들로 인벤토리 구성
    /// </summary>
    private void Awake()
    {
        InitializeSlots();
    }

    /// <summary>
    /// 슬롯들을 초기화
    /// </summary>
    private void InitializeSlots()
    {
        slots = new List<InventorySlot>(CAPACITY);
        
        for (int i = 0; i < CAPACITY; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <param name="amount">추가할 수량</param>
    /// <returns>실제로 추가된 수량</returns>
    public int Add(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        int remainingAmount = amount;

        // 1단계: 같은 아이템이 있는 슬롯들에 먼저 추가 (스택 합치기)
        for (int i = 0; i < slots.Count && remainingAmount > 0; i++)
        {
            InventorySlot slot = slots[i];
            
            if (slot.IsSameItem(item) && !slot.IsFull)
            {
                int addedToSlot = slot.AddCount(remainingAmount);
                remainingAmount -= addedToSlot;
            }
        }

        // 2단계: 빈 슬롯에 나머지 아이템 추가
        for (int i = 0; i < slots.Count && remainingAmount > 0; i++)
        {
            InventorySlot slot = slots[i];
            
            if (slot.IsEmpty)
            {
                int amountToAdd = Mathf.Min(remainingAmount, item.MaxStack);
                slot.SetItem(item, amountToAdd);
                remainingAmount -= amountToAdd;
            }
        }

        // 실제로 추가된 수량 계산
        int actualAdded = amount - remainingAmount;
        
        if (actualAdded > 0)
        {
            OnChanged?.Invoke();
        }

        return actualAdded;
    }

    /// <summary>
    /// 특정 슬롯에서 아이템 제거
    /// </summary>
    /// <param name="index">슬롯 인덱스</param>
    /// <param name="amount">제거할 수량</param>
    /// <returns>실제로 제거된 수량</returns>
    public int RemoveAt(int index, int amount)
    {
        if (index < 0 || index >= slots.Count || amount <= 0) return 0;

        InventorySlot slot = slots[index];
        int removedAmount = slot.RemoveCount(amount);

        if (removedAmount > 0)
        {
            OnChanged?.Invoke();
        }

        return removedAmount;
    }

    /// <summary>
    /// 재료 키가 같은 아이템들의 총 개수 계산
    /// </summary>
    /// <param name="ingredientKey">찾을 재료 키</param>
    /// <returns>해당 재료 키를 가진 아이템들의 총 개수</returns>
    public int CountByIngredientKey(string ingredientKey)
    {
        if (string.IsNullOrEmpty(ingredientKey)) return 0;

        int totalCount = 0;

        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty && slot.Item.IngredientKey == ingredientKey)
            {
                totalCount += slot.Count;
            }
        }

        return totalCount;
    }

    /// <summary>
    /// 특정 아이템의 총 개수 계산
    /// </summary>
    /// <param name="item">찾을 아이템</param>
    /// <returns>해당 아이템의 총 개수</returns>
    public int CountItem(ItemDefinition item)
    {
        if (item == null) return 0;

        int totalCount = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsSameItem(item))
            {
                totalCount += slot.Count;
            }
        }

        return totalCount;
    }

    /// <summary>
    /// 인벤토리가 가득 찼는지 확인
    /// </summary>
    /// <returns>모든 슬롯이 차있으면 true</returns>
    public bool IsFull()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 특정 슬롯의 아이템 정보 가져오기
    /// </summary>
    /// <param name="index">슬롯 인덱스</param>
    /// <returns>슬롯 정보 (유효하지 않은 인덱스면 null)</returns>
    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Count) return null;
        return slots[index];
    }

    /// <summary>
    /// 인벤토리를 완전히 비움
    /// </summary>
    public void Clear()
    {
        foreach (InventorySlot slot in slots)
        {
            slot.Clear();
        }
        
        OnChanged?.Invoke();
    }
}

using UnityEngine;
using System;

/// <summary>
/// 개별 주문 정보를 관리하는 클래스
/// 단일 책임: 주문 데이터 및 타이머 관리
/// </summary>
[Serializable]
public class CustomerOrder
{
    [Header("Order Data")]
    public ItemDefinition orderItem;
    public int quantity;
    public float orderTimeLimit; // 주문 제한 시간 (초)

    [Header("Runtime Data")]
    [SerializeField] private float remainingTime;
    [SerializeField] private bool isActive;

    public float RemainingTime => remainingTime;
    public bool IsActive => isActive;
    public bool IsExpired => isActive && remainingTime <= 0f;

    public event Action<CustomerOrder> OnOrderExpired;

    public CustomerOrder(ItemDefinition item, int qty, float timeLimit = 30f)
    {
        orderItem = item;
        quantity = qty;
        orderTimeLimit = timeLimit;
        remainingTime = timeLimit;
        isActive = false;
    }

    /// <summary>
    /// 주문 활성화 (타이머 시작)
    /// </summary>
    public void ActivateOrder()
    {
        isActive = true;
        remainingTime = orderTimeLimit;
    }

    /// <summary>
    /// 주문 비활성화 (타이머 정지)
    /// </summary>
    public void DeactivateOrder()
    {
        isActive = false;
    }

    /// <summary>
    /// 타이머 업데이트
    /// </summary>
    public void UpdateTimer(float deltaTime)
    {
        if (!isActive) return;

        remainingTime -= deltaTime;
        
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            OnOrderExpired?.Invoke(this);
        }
    }

    /// <summary>
    /// 주문 완료 처리
    /// </summary>
    public void CompleteOrder()
    {
        DeactivateOrder();
    }

    /// <summary>
    /// 남은 시간 비율 (0-1)
    /// </summary>
    public float GetTimeRatio()
    {
        if (orderTimeLimit <= 0f) return 1f;
        return Mathf.Clamp01(remainingTime / orderTimeLimit);
    }
}

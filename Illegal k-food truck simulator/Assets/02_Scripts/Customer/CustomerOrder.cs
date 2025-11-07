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
    public float foodWaitTimeLimit; // 음식 대기 제한 시간 (초)

    [Header("Runtime Data")]
    [SerializeField] private float remainingTime;
    [SerializeField] private bool isActive;
    [SerializeField] private bool isFoodWaiting; // 음식 대기 중인지 (주문 완료 후)

    public float RemainingTime => remainingTime;
    public bool IsActive => isActive;
    public bool IsFoodWaiting => isFoodWaiting;
    public bool IsExpired => isActive && remainingTime <= 0f;

    public event Action<CustomerOrder> OnOrderExpired;
    public event Action<CustomerOrder> OnQueueWaitExpired; // 대기열 타임아웃

    public CustomerOrder(ItemDefinition item, int qty, float foodTimeLimit = 30f)
    {
        orderItem = item;
        quantity = qty;
        foodWaitTimeLimit = foodTimeLimit;
        remainingTime = foodTimeLimit;
        isActive = false;
        isFoodWaiting = false;
    }

    /// <summary>
    /// 주문 활성화 (음식 대기 타이머 시작)
    /// </summary>
    public void ActivateOrder()
    {
        isActive = true;
        isFoodWaiting = true;
        remainingTime = foodWaitTimeLimit;
    }

    /// <summary>
    /// 주문 비활성화 (타이머 정지)
    /// </summary>
    public void DeactivateOrder()
    {
        isActive = false;
        isFoodWaiting = false;
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
        float timeLimit = isFoodWaiting ? foodWaitTimeLimit : foodWaitTimeLimit;
        if (timeLimit <= 0f) return 1f;
        return Mathf.Clamp01(remainingTime / timeLimit);
    }
}

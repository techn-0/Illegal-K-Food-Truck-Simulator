using UnityEngine;
using System;

public class BusinessManager : MonoBehaviour
{
    public static bool IsBusinessActive { get; private set; } = false;

    public static event Action<bool> OnBusinessStateChanged;

    void Start()
    {
        if (CookingInteractor.Instance != null)
        {
            CookingInteractor.Instance.OnPlayerRangeChanged += HandlePlayerRangeChanged;
        }
    }

    void OnDestroy()
    {
        if (CookingInteractor.Instance != null)
        {
            CookingInteractor.Instance.OnPlayerRangeChanged -= HandlePlayerRangeChanged;
        }
    }

    private void HandlePlayerRangeChanged(bool inRange)
    {
        if (!inRange && IsBusinessActive)
        {
            ToggleBusinessState();
        }
    }

    public static void ToggleBusinessState()
    {
        if (!IsBusinessActive) // 장사 시작 시도
        {
            if (CookingInteractor.Instance != null && CookingInteractor.Instance.IsPlayerInCookingRange())
            {
                IsBusinessActive = true;
                Debug.Log("Business is now Active");
                OnBusinessStateChanged?.Invoke(IsBusinessActive);
            }
            else
            {
                Debug.Log("장사 시작 불가: 플레이어가 범위 내에 없습니다.");
            }
        }
        else // 장사 종료
        {
            IsBusinessActive = false;
            Debug.Log("Business is now Inactive");
            OnBusinessStateChanged?.Invoke(IsBusinessActive);
        }
    }
}

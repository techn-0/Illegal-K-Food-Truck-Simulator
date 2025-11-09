using UnityEngine;
using System;

public class BusinessManager : MonoBehaviour
{
    public static bool IsBusinessActive { get; private set; } = false;

    public static event Action<bool> OnBusinessStateChanged;

    // 최근 토글 시각을 저장하여 중복 토글(빠른 연속 호출)을 방지합니다.
    private static float _lastToggleTime = 0f;
    private const float ToggleDebounceSeconds = 0.25f; // 0.25초 이내 중복 호출 무시

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
        // 빠른 연속 호출은 무시
        if (Time.realtimeSinceStartup - _lastToggleTime < ToggleDebounceSeconds)
        {
            Debug.Log("ToggleBusinessState 호출이 너무 잦아 무시합니다.");
            return;
        }
        _lastToggleTime = Time.realtimeSinceStartup;

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

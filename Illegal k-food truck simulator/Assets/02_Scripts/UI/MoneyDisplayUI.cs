using UnityEngine;
using TMPro;

/// <summary>
/// 플레이어의 현재 돈을 표시하는 UI 컴포넌트
/// 단일 책임: 돈 표시만 담당
/// </summary>
public class MoneyDisplayUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("Display Settings")]
    [SerializeField] private string prefix = "돈: ";
    [SerializeField] private string suffix = "원";
    
    private void Start()
    {
        // 플레이어 돈 변경 이벤트 구독
        PlayerMoneyManager.OnMoneyChanged += UpdateMoneyDisplay;
        
        // 초기 돈 표시 (PlayerMoneyManager가 없다면 0원으로 표시)
        if (PlayerMoneyManager.Instance != null)
        {
            UpdateMoneyDisplay(PlayerMoneyManager.Instance.CurrentMoney);
        }
        else
        {
            UpdateMoneyDisplay(0);
        }
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        PlayerMoneyManager.OnMoneyChanged -= UpdateMoneyDisplay;
    }
    
    /// <summary>
    /// 돈 표시를 업데이트합니다
    /// </summary>
    /// <param name="amount">표시할 금액</param>
    private void UpdateMoneyDisplay(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"{prefix}{amount:N0}{suffix}";
        }
    }
}

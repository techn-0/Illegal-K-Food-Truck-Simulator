using UnityEngine;
using TMPro;

/// <summary>
/// 현재 Day를 화면에 표시하는 UI
/// </summary>
public class DayDisplayUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI dayText;
    
    [Header("Display Settings")]
    [SerializeField] private string prefix = "Day ";

    private void Start()
    {
        UpdateDayDisplay();
    }

    private void OnEnable()
    {
        UpdateDayDisplay();
    }

    private void UpdateDayDisplay()
    {
        if (dayText != null && GameManager.Instance != null && GameManager.Instance.Save != null)
        {
            dayText.text = $"{prefix}{GameManager.Instance.Save.currentDay}";
        }
    }
}


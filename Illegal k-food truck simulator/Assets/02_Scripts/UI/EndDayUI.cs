using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 하루 종료 확인 및 요약 UI를 관리
/// </summary>
public class EndDayUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject summaryPanel;

    [Header("Confirm Panel")]
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    [Header("Summary Panel")]
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private Button summaryConfirmButton;

    private void Start()
    {
        // 버튼 이벤트 연결
        if (confirmYesButton != null)
            confirmYesButton.onClick.AddListener(OnConfirmYes);
        
        if (confirmNoButton != null)
            confirmNoButton.onClick.AddListener(OnConfirmNo);
        
        if (summaryConfirmButton != null)
            summaryConfirmButton.onClick.AddListener(OnSummaryConfirm);

        // 초기에는 모든 패널 숨김
        HideAll();
    }

    /// <summary>
    /// 하루 종료 확인 UI 표시
    /// </summary>
    public void ShowConfirm()
    {
        HideAll();
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 예 버튼 클릭 (하루 요약 표시)
    /// </summary>
    private void OnConfirmYes()
    {
        ShowSummary();
    }

    /// <summary>
    /// 아니오 버튼 클릭 (UI 닫기)
    /// </summary>
    private void OnConfirmNo()
    {
        HideAll();
    }

    /// <summary>
    /// 하루 요약 UI 표시
    /// </summary>
    private void ShowSummary()
    {
        HideAll();
        
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(true);

            // 오늘 매출과 총 매출 표시
            if (summaryText != null && GameManager.Instance != null)
            {
                int todayEarnings = GameManager.Instance.Save.todayEarnings;
                int totalEarnings = GameManager.Instance.Save.totalEarnings + todayEarnings; // 오늘 매출 포함
                
                summaryText.text = $"오늘의 매출\n{todayEarnings:N0}원\n\n총 매출\n{totalEarnings:N0}원";
            }
        }
    }

    /// <summary>
    /// 요약 확인 버튼 클릭 (저장 및 다음날 진행)
    /// </summary>
    private void OnSummaryConfirm()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndDayAndSave();
        }
    }

    /// <summary>
    /// 모든 UI 패널 숨김
    /// </summary>
    private void HideAll()
    {
        if (confirmPanel != null) confirmPanel.SetActive(false);
        if (summaryPanel != null) summaryPanel.SetActive(false);
    }
}

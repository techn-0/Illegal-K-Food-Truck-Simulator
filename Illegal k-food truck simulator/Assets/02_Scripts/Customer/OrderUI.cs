using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemQuantity;
    public Button sellButton;
    
    [Header("Timer UI")]
    public Image timerFillImage; // 타이머 바 (인스펙터에서 설정)
    public TMP_Text timerText; // 타이머 텍스트 (인스펙터에서 설정)

    private Camera mainCamera;
    private ItemDefinition orderItem;
    private int orderQuantity;
    private CustomerOrderSystem customerOrderSystem;

    private void Start()
    {
        mainCamera = Camera.main;
        customerOrderSystem = GetComponentInParent<CustomerOrderSystem>();
    }

    private void Update()
    {
        // 판매 가능 여부에 따라 버튼 상태 업데이트
        UpdateSellButton();
        
        // 타이머 UI 업데이트
        UpdateTimerUI();
    }

    private void LateUpdate()
    {
        // UI가 항상 카메라를 바라보도록 설정
        if (mainCamera != null)
        {
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0; // 수직 축 회전을 방지
            transform.rotation = Quaternion.LookRotation(-direction);
        }
    }

    public void Setup(ItemDefinition item, int quantity)
    {
        orderItem = item;
        orderQuantity = quantity;
        
        if (item != null)
        {
            itemIcon.sprite = item.Icon;
            itemName.text = item.DisplayName;
            itemQuantity.text = quantity.ToString();
        }
    }

    private void UpdateSellButton()
    {
        if (orderItem == null || SaleService.Instance == null)
        {
            sellButton.interactable = false;
            return;
        }

        // 판매 가능 여부에 따라 버튼 활성화
        bool canSell = SaleService.Instance.CanSell(orderItem, orderQuantity);
        sellButton.interactable = canSell;

        // 버튼 색상 변경 (시각적 피드백)
        var colors = sellButton.colors;
        colors.normalColor = canSell ? Color.white : Color.gray;
        sellButton.colors = colors;
    }

    /// <summary>
    /// 타이머 UI 업데이트
    /// </summary>
    private void UpdateTimerUI()
    {
        if (customerOrderSystem == null) return;

        var currentOrder = customerOrderSystem.GetCurrentOrder();
        if (currentOrder == null || !currentOrder.IsActive) return;

        // 타이머 바 업데이트
        if (timerFillImage != null)
        {
            float timeRatio = currentOrder.GetTimeRatio();
            timerFillImage.fillAmount = timeRatio;
            
            // 시간에 따른 색상 변경
            if (timeRatio > 0.5f)
                timerFillImage.color = Color.green;
            else if (timeRatio > 0.25f)
                timerFillImage.color = Color.yellow;
            else
                timerFillImage.color = Color.red;
        }

        // 타이머 텍스트 업데이트
        if (timerText != null)
        {
            int remainingSeconds = Mathf.CeilToInt(currentOrder.RemainingTime);
            timerText.text = remainingSeconds.ToString();
        }
    }

    /// <summary>
    /// 판매 버튼 클릭 시 호출되는 메서드 (에디터에서 설정)
    /// </summary>
    public void OnSellButtonClicked()
    {
        if (orderItem == null || SaleService.Instance == null) return;

        // SaleService를 통해 판매 처리
        bool saleSuccess = SaleService.Instance.ProcessSale(orderItem, orderQuantity, GetItemPrice());

        // 판매 성공시 손님에게 알림
        if (saleSuccess && customerOrderSystem != null)
        {
            customerOrderSystem.OnOrderCompleted();
        }
    }

    /// <summary>
    /// 아이템 가격 계산 (기본값 또는 아이템에서 가져오기)
    /// </summary>
    private int GetItemPrice()
    {
        // ItemDefinition에 가격 정보가 있다면 사용, 없으면 기본값
        // 이 부분은 ItemDefinition 구조에 따라 수정 필요
        return 100; // 임시 기본값
    }
}

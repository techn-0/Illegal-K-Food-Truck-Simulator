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

    private Camera mainCamera;
    private ItemDefinition orderItem;
    private int orderQuantity;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 판매 가능 여부에 따라 버튼 상태 업데이트
        UpdateSellButton();
    }

    private void LateUpdate()
    {
        // UI가 항상 카메라를 바라보도록 설정
        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0; // 수직 축 회전을 방지
        transform.rotation = Quaternion.LookRotation(-direction);
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
    /// 판매 버튼 클릭 시 호출되는 메서드 (에디터에서 설정)
    /// </summary>
    public void OnSellButtonClicked()
    {
        if (orderItem == null) return;

        // SaleService의 정적 메서드를 통해 판매 처리
        bool saleSuccess = SaleService.TrySellItem(orderItem, orderQuantity);

        // 판매 성공시 UI 제거
        if (saleSuccess)
        {
            Destroy(gameObject);
        }
    }
}

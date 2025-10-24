using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점에서 개별 아이템을 표시하는 UI 컴포넌트
/// 단일 책임: 하나의 아이템 정보 표시 및 구매 버튼 처리
/// </summary>
public class ItemShopItemUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private TextMeshProUGUI itemAmountText; // 판매 수량 표시
    [SerializeField] private Button purchaseButton;
    
    private ShopItemData shopItemData;
    private ItemShop itemShop;
    
    private void Start()
    {
        // 구매 버튼 이벤트 연결
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        }
    }
    
    private void Update()
    {
        // 매 프레임마다 구매 가능 상태 업데이트
        UpdatePurchaseButton();
    }
    
    /// <summary>
    /// 아이템 정보 설정
    /// </summary>
    /// <param name="shopItem">상점 아이템 데이터</param>
    /// <param name="shop">아이템 상점 참조</param>
    public void Setup(ShopItemData shopItem, ItemShop shop)
    {
        shopItemData = shopItem;
        itemShop = shop;
        
        if (shopItemData?.Item == null) return;
        
        // UI 정보 설정
        var item = shopItemData.Item;
        
        if (itemIcon != null && item.Icon != null)
        {
            itemIcon.sprite = item.Icon;
        }
        
        if (itemNameText != null)
        {
            itemNameText.text = item.DisplayName;
        }
        
        if (itemPriceText != null)
        {
            itemPriceText.text = $"{shopItemData.Price} 골드";
        }
        
        if (itemAmountText != null)
        {
            itemAmountText.text = $"x{shopItemData.SellAmount}";
        }
        
        // 초기 상태 업데이트
        UpdatePurchaseButton();
    }
    
    /// <summary>
    /// 구매 버튼 상태 업데이트
    /// </summary>
    private void UpdatePurchaseButton()
    {
        if (shopItemData?.Item == null || itemShop == null) return;
        
        bool canPurchase = itemShop.CanPurchaseItem(shopItemData);
        
        // 구매 버튼 상태 설정
        if (purchaseButton != null)
        {
            purchaseButton.interactable = canPurchase;
            
            var buttonText = purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (canPurchase)
                {
                    buttonText.text = "구매";
                }
                else
                {
                    // 구매 불가능한 이유 확인
                    if (!PlayerMoneyManager.Instance.CanAfford(shopItemData.Price))
                    {
                        buttonText.text = "골드 부족";
                    }
                    else
                    {
                        buttonText.text = "공간 부족";
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 구매 버튼 클릭 이벤트
    /// </summary>
    private void OnPurchaseButtonClicked()
    {
        if (shopItemData?.Item == null || itemShop == null) return;
        
        bool purchaseSuccess = itemShop.TryPurchaseItem(shopItemData);
        
        if (purchaseSuccess)
        {
            Debug.Log($"아이템 구매 성공: {shopItemData.Item.DisplayName} x{shopItemData.SellAmount}");
            // 구매 성공 시 버튼 상태가 자동으로 Update에서 업데이트됨
        }
        else
        {
            Debug.Log($"아이템 구매 실패: {shopItemData.Item.DisplayName}");
        }
    }
}

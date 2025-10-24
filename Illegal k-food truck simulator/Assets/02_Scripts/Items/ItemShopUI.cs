using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 아이템 상점 UI를 관리하는 클래스
/// 단일 책임: 상점 UI 표시 및 구매 버튼 상호작용
/// </summary>
public class ItemShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform itemContainer; // 아이템들이 들어갈 컨테이너
    [SerializeField] private GameObject itemShopItemPrefab; // 아이템 UI 프리팹
    [SerializeField] private TextMeshProUGUI playerMoneyText; // 플레이어 돈 표시 텍스트
    [SerializeField] private Button closeButton; // 상점 닫기 버튼 (선택사항)
    
    private ItemShop itemShop;
    
    private void Start()
    {
        // 상점 참조 찾기
        var shopInteractor = FindObjectOfType<ItemShopInteractor>();
        if (shopInteractor != null)
        {
            itemShop = shopInteractor.GetItemShop();
        }
        
        // 돈 변경 이벤트 구독
        PlayerMoneyManager.OnMoneyChanged += OnMoneyChanged;
        
        // 닫기 버튼 이벤트 연결
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }
    }
    
    private void OnEnable()
    {
        // 상점 참조가 없다면 다시 찾기
        if (itemShop == null)
        {
            var shopInteractor = FindObjectOfType<ItemShopInteractor>();
            if (shopInteractor != null)
            {
                itemShop = shopInteractor.GetItemShop();
            }
        }
        
        // UI가 활성화될 때마다 상점 아이템 생성
        CreateShopItems();
        UpdateMoneyDisplay();
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        PlayerMoneyManager.OnMoneyChanged -= OnMoneyChanged;
    }
    
    /// <summary>
    /// 상점 아이템들을 생성하여 UI에 표시
    /// </summary>
    private void CreateShopItems()
    {
        if (itemShop == null || itemContainer == null || itemShopItemPrefab == null) return;
        
        // 기존 아이템들 제거
        ClearShopItems();
        
        // 상점 아이템들을 UI로 생성
        var shopItems = itemShop.GetShopItems();
        foreach (var shopItem in shopItems)
        {
            if (shopItem.Item != null)
            {
                GameObject itemUI = Instantiate(itemShopItemPrefab, itemContainer);
                var shopItemUI = itemUI.GetComponent<ItemShopItemUI>();
                if (shopItemUI != null)
                {
                    shopItemUI.Setup(shopItem, itemShop);
                }
            }
        }
    }
    
    /// <summary>
    /// 기존 상점 아이템 UI들 제거
    /// </summary>
    private void ClearShopItems()
    {
        if (itemContainer == null) return;
        
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    /// <summary>
    /// 돈 표시 업데이트
    /// </summary>
    private void UpdateMoneyDisplay()
    {
        if (playerMoneyText != null && PlayerMoneyManager.Instance != null)
        {
            playerMoneyText.text = $"골드: {PlayerMoneyManager.Instance.CurrentMoney}";
        }
    }
    
    /// <summary>
    /// 돈 변경 이벤트 핸들러
    /// </summary>
    private void OnMoneyChanged(int newAmount)
    {
        UpdateMoneyDisplay();
    }
    
    /// <summary>
    /// 상점 닫기
    /// </summary>
    private void CloseShop()
    {
        gameObject.SetActive(false);
        
        // CursorManager 호출
        CursorManager cursorManager = FindObjectOfType<CursorManager>();
        if (cursorManager != null)
        {
            cursorManager.OnUIWindowClosed();
        }
        
        // 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

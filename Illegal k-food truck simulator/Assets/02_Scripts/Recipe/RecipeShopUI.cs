using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 레시피 상점 UI를 관리하는 클래스
/// 단일 책임: 상점 UI 표시 및 구매 버튼 상호작용
/// </summary>
public class RecipeShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform recipeContainer; // 레시피 아이템들이 들어갈 컨테이너
    [SerializeField] private GameObject recipeItemPrefab; // 레시피 아이템 UI 프리팹
    [SerializeField] private TextMeshProUGUI playerMoneyText; // 플레이어 돈 표시 텍스트
    [SerializeField] private Button closeButton; // 닫기 버튼
    
    private RecipeShop recipeShop;
    
    private void Start()
    {
        // 상점 참조 찾기
        var shopInteractor = FindObjectOfType<RecipeShopInteractor>();
        if (shopInteractor != null)
        {
            recipeShop = shopInteractor.GetRecipeShop();
        }
        
        // 닫기 버튼 이벤트 연결
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }
        
        // 돈 변경 이벤트 구독
        PlayerMoneyManager.OnMoneyChanged += OnMoneyChanged;
        
        // 레시피 해금 이벤트 구독
        RecipeUnlockManager.OnUnlockedRecipesChanged += RefreshShopItems;
    }
    
    private void OnEnable()
    {
        // UI가 활성화될 때마다 상점 아이템 생성
        CreateShopItems();
        UpdateMoneyDisplay();
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        PlayerMoneyManager.OnMoneyChanged -= OnMoneyChanged;
        RecipeUnlockManager.OnUnlockedRecipesChanged -= RefreshShopItems;
    }
    
    /// <summary>
    /// 상점 아이템들을 생성하여 UI에 표시
    /// </summary>
    private void CreateShopItems()
    {
        if (recipeShop == null || recipeContainer == null || recipeItemPrefab == null) return;
        
        // 기존 아이템들 제거
        ClearShopItems();
        
        // 상점 레시피들을 UI로 생성
        var shopRecipes = recipeShop.GetShopRecipes();
        foreach (var shopItem in shopRecipes)
        {
            if (shopItem.Recipe != null)
            {
                GameObject itemUI = Instantiate(recipeItemPrefab, recipeContainer);
                var shopItemUI = itemUI.GetComponent<RecipeShopItemUI>();
                if (shopItemUI != null)
                {
                    shopItemUI.Setup(shopItem, recipeShop);
                }
            }
        }
    }
    
    /// <summary>
    /// 기존 상점 아이템 UI들 제거
    /// </summary>
    private void ClearShopItems()
    {
        if (recipeContainer == null) return;
        
        foreach (Transform child in recipeContainer)
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
    /// 상점 아이템 새로고침 (레시피 해금 상태 변경 시)
    /// </summary>
    private void RefreshShopItems()
    {
        if (gameObject.activeInHierarchy)
        {
            CreateShopItems();
        }
    }
    
    /// <summary>
    /// 상점 닫기
    /// </summary>
    private void CloseShop()
    {
        gameObject.SetActive(false);
    }
}

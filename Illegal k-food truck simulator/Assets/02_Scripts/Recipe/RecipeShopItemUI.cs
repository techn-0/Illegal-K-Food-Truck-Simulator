using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점에서 개별 레시피 아이템을 표시하는 UI 컴포넌트
/// 단일 책임: 하나의 레시피 아이템 정보 표시 및 구매 버튼 처리
/// </summary>
public class RecipeShopItemUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image recipeIcon;
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI recipePriceText;
    [SerializeField] private TextMeshProUGUI recipeDescriptionText; // 선택사항
    [SerializeField] private Button purchaseButton;
    [SerializeField] private GameObject alreadyOwnedIndicator; // 이미 보유 표시
    
    private ShopRecipeItem shopItem;
    private RecipeShop recipeShop;
    
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
    /// 레시피 아이템 정보 설정
    /// </summary>
    /// <param name="item">상점 레시피 아이템</param>
    /// <param name="shop">레시피 상점 참조</param>
    public void Setup(ShopRecipeItem item, RecipeShop shop)
    {
        shopItem = item;
        recipeShop = shop;
        
        if (shopItem?.Recipe == null) return;
        
        // UI 정보 설정
        var recipe = shopItem.Recipe;
        
        if (recipeIcon != null && recipe.DishImage != null)
        {
            recipeIcon.sprite = recipe.DishImage;
        }
        
        if (recipeNameText != null)
        {
            recipeNameText.text = recipe.RecipeName;
        }
        
        if (recipePriceText != null)
        {
            recipePriceText.text = $"{shopItem.Price} 골드";
        }
        
        if (recipeDescriptionText != null)
        {
            // 레시피 설명 (재료 정보 등)
            string description = "필요 재료:\n";
            foreach (var ingredient in recipe.RequiredIngredients)
            {
                description += $"• {ingredient.Ingredient.DisplayName} x{ingredient.RequiredAmount}\n";
            }
            recipeDescriptionText.text = description;
        }
        
        // 초기 상태 업데이트
        UpdatePurchaseButton();
    }
    
    /// <summary>
    /// 구매 버튼 상태 업데이트
    /// </summary>
    private void UpdatePurchaseButton()
    {
        if (shopItem?.Recipe == null || recipeShop == null) return;
        
        bool isAlreadyOwned = RecipeUnlockManager.Instance.IsRecipeUnlocked(shopItem.Recipe);
        bool canPurchase = recipeShop.CanPurchaseRecipe(shopItem.Recipe);
        
        // 이미 보유한 레시피 표시
        if (alreadyOwnedIndicator != null)
        {
            alreadyOwnedIndicator.SetActive(isAlreadyOwned);
        }
        
        // 구매 버튼 상태 설정
        if (purchaseButton != null)
        {
            if (isAlreadyOwned)
            {
                // 이미 보유한 경우
                purchaseButton.interactable = false;
                var buttonText = purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "보유중";
                }
            }
            else if (canPurchase)
            {
                // 구매 가능한 경우
                purchaseButton.interactable = true;
                var buttonText = purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "구매";
                }
            }
            else
            {
                // 구매 불가능한 경우 (골드 부족)
                purchaseButton.interactable = false;
                var buttonText = purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "골드 부족";
                }
            }
        }
    }
    
    /// <summary>
    /// 구매 버튼 클릭 이벤트
    /// </summary>
    private void OnPurchaseButtonClicked()
    {
        if (shopItem?.Recipe == null || recipeShop == null) return;
        
        bool purchaseSuccess = recipeShop.TryPurchaseRecipe(shopItem.Recipe);
        
        if (purchaseSuccess)
        {
            Debug.Log($"레시피 구매 성공: {shopItem.Recipe.RecipeName}");
            // 구매 성공 시 버튼 상태가 자동으로 Update에서 업데이트됨
        }
        else
        {
            Debug.Log($"레시피 구매 실패: {shopItem.Recipe.RecipeName}");
        }
    }
}

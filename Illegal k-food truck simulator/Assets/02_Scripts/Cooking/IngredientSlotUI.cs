using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI ingredientNameText;
    
    private RecipeIngredient ingredient;
    private Inventory playerInventory;
    
    void Start()
    {
        // 플레이어 인벤토리 참조 찾기
        playerInventory = FindFirstObjectByType<Inventory>();
    }
    
    public void SetupIngredient(RecipeIngredient recipeIngredient)
    {
        ingredient = recipeIngredient;
        
        // 재료 정보 설정
        ingredientIcon.sprite = ingredient.Ingredient.Icon;
        ingredientNameText.text = ingredient.Ingredient.DisplayName;
        
        // 수량 정보 업데이트
        UpdateAmountDisplay();
    }
    
    void Update()
    {
        // 인벤토리 수량 변화에 따른 업데이트
        UpdateAmountDisplay();
    }
    
    private void UpdateAmountDisplay()
    {
        if (playerInventory == null || ingredient == null) return;
        
        int currentAmount = playerInventory.GetItemCount(ingredient.Ingredient);
        int requiredAmount = ingredient.RequiredAmount;
        
        amountText.text = $"{currentAmount}/{requiredAmount}";
        
        // 충분한 재료가 있는지에 따라 색상 변경
        amountText.color = currentAmount >= requiredAmount ? Color.green : Color.red;
    }
}

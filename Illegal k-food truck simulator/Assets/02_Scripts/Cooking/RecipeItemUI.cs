using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeItemUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image dishImage;
    [SerializeField] private TextMeshProUGUI dishNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI cookingTimeText;
    [SerializeField] private Transform ingredientsContainer;
    [SerializeField] private GameObject ingredientSlotPrefab;
    [SerializeField] private Button cookButton;
    
    private RecipeDefinition recipe;
    private CookingManager cookingManager;
    private bool isInitialized = false;
    
    void Awake()
    {
        cookingManager = CookingManager.Instance;
    }
    
    void Start()
    {
        cookButton.onClick.AddListener(OnCookButtonClicked);
        isInitialized = true;
        
        // Start에서 초기화가 완료된 후 버튼 상태 업데이트
        if (recipe != null)
        {
            UpdateCookButton();
        }
    }
    
    public void SetupRecipe(RecipeDefinition recipeDefinition)
    {
        recipe = recipeDefinition;
        
        // 기본 정보 설정
        dishImage.sprite = recipe.DishImage;
        dishNameText.text = recipe.RecipeName;
        priceText.text = $"가격: {recipe.Price}원";
        cookingTimeText.text = $"조리시간: {recipe.CookingTime}초";
        
        // 재료 목록 생성
        CreateIngredientSlots();
        
        // Start가 호출된 후에만 버튼 상태 업데이트
        if (isInitialized)
        {
            UpdateCookButton();
        }
    }
    
    void Update()
    {
        // 초기화가 완료된 후에만 매 프레임마다 요리 가능 여부 확인
        if (isInitialized)
        {
            UpdateCookButton();
        }
    }
    
    private void CreateIngredientSlots()
    {
        foreach (var ingredient in recipe.RequiredIngredients)
        {
            GameObject ingredientSlot = Instantiate(ingredientSlotPrefab, ingredientsContainer);
            IngredientSlotUI slotUI = ingredientSlot.GetComponent<IngredientSlotUI>();
            slotUI.SetupIngredient(ingredient);
        }
    }
    
    private void UpdateCookButton()
    {
        bool canCook = cookingManager.CanCookRecipe(recipe);
        cookButton.interactable = canCook;
        
        // 버튼 색상 변경 (선택사항)
        var colors = cookButton.colors;
        colors.normalColor = canCook ? Color.white : Color.gray;
        cookButton.colors = colors;
    }
    
    private void OnCookButtonClicked()
    {
        cookingManager.StartCooking(recipe);
    }
}

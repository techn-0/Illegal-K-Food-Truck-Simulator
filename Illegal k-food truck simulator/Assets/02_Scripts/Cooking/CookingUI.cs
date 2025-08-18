using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cookingPanel;
    [SerializeField] private Button cookingButton;
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private GameObject recipeItemPrefab;
    
    [Header("Cooking Timer UI")]
    [SerializeField] private GameObject cookingTimerPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI cookingDishNameText;
    
    private CookingManager cookingManager;
    private float currentCookingTime;
    private float totalCookingTime;
    
    void Start()
    {
        cookingManager = CookingManager.Instance;
        
        // 쿠킹 버튼 이벤트 연결
        cookingButton.onClick.AddListener(ToggleCookingPanel);
        
        // 쿠킹 매니저 이벤트 구독
        cookingManager.OnCookingStarted += OnCookingStarted;
        cookingManager.OnCookingCompleted += OnCookingCompleted;
        cookingManager.OnCookingFailed += OnCookingFailed;
        
        // 초기 상태 설정
        cookingPanel.SetActive(false);
        cookingTimerPanel.SetActive(false);
        
        // 레시피 목록 생성
        CreateRecipeList();
    }
    
    void Update()
    {
        if (cookingManager.IsCooking)
        {
            UpdateCookingTimer();
        }
    }
    
    void OnDestroy()
    {
        if (cookingManager != null)
        {
            cookingManager.OnCookingStarted -= OnCookingStarted;
            cookingManager.OnCookingCompleted -= OnCookingCompleted;
            cookingManager.OnCookingFailed -= OnCookingFailed;
        }
    }
    
    private void ToggleCookingPanel()
    {
        cookingPanel.SetActive(!cookingPanel.activeSelf);
    }
    
    private void CreateRecipeList()
    {
        var recipes = cookingManager.GetAvailableRecipes();
        
        foreach (var recipe in recipes)
        {
            GameObject recipeItem = Instantiate(recipeItemPrefab, recipeContainer);
            RecipeItemUI recipeItemUI = recipeItem.GetComponent<RecipeItemUI>();
            recipeItemUI.SetupRecipe(recipe);
        }
    }
    
    private void OnCookingStarted(RecipeDefinition recipe, float cookingTime)
    {
        cookingPanel.SetActive(false);
        cookingTimerPanel.SetActive(true);
        
        totalCookingTime = cookingTime;
        currentCookingTime = cookingTime;
        
        cookingDishNameText.text = $"{recipe.RecipeName} 조리 중...";
        timerSlider.value = 1f;
    }
    
    private void OnCookingCompleted(RecipeDefinition recipe)
    {
        cookingTimerPanel.SetActive(false);
        // 완성 메시지 표시 (선택사항)
        Debug.Log($"{recipe.RecipeName} 조리 완성!");
    }
    
    private void OnCookingFailed(string message)
    {
        Debug.Log(message);
        // 실패 메시지 UI 표시 (선택사항)
    }
    
    private void UpdateCookingTimer()
    {
        currentCookingTime -= Time.deltaTime;
        
        if (currentCookingTime <= 0)
        {
            currentCookingTime = 0;
        }
        
        float progress = 1f - (currentCookingTime / totalCookingTime);
        timerSlider.value = progress;
        
        timerText.text = $"{Mathf.Ceil(currentCookingTime)}초";
    }
}

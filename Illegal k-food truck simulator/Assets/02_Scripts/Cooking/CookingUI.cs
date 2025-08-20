using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cookingPanel;
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private GameObject recipeItemPrefab;
    
    [Header("Cooking Timer UI")]
    [SerializeField] private GameObject cookingTimerPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI cookingDishNameText;
    
    private CookingManager cookingManager;
    private CookingTimer cookingTimer;
    
    void Awake()
    {
        cookingManager = CookingManager.Instance;
    }
    
    void Start()
    {
        // 쿠킹 매니저 이벤트 구독
        cookingManager.OnCookingStarted += OnCookingStarted;
        cookingManager.OnCookingCompleted += OnCookingCompleted;
        cookingManager.OnCookingFailed += OnCookingFailed;
        
        // 쿠킹 타이머 가져오기 및 이벤트 구독
        cookingTimer = cookingManager.GetCookingTimer();
        cookingTimer.OnTimerUpdated += OnTimerUpdated;
        
        // 초기 상태 설정
        // cookingPanel.SetActive(false);
        // cookingTimerPanel.SetActive(false);

        // 레시피 목록 생성
        CreateRecipeList();
    }
    
    void OnDestroy()
    {
        if (cookingManager != null)
        {
            cookingManager.OnCookingStarted -= OnCookingStarted;
            cookingManager.OnCookingCompleted -= OnCookingCompleted;
            cookingManager.OnCookingFailed -= OnCookingFailed;
        }
        
        if (cookingTimer != null)
        {
            cookingTimer.OnTimerUpdated -= OnTimerUpdated;
        }
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
        
        cookingDishNameText.text = $"{recipe.RecipeName} 조리 중...";
        timerSlider.value = 0f; // 진행도는 0부터 시작
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
    
    private void OnTimerUpdated(float remainingTime, float progress)
    {
        // 타이머 UI 업데이트
        timerSlider.value = progress;
        timerText.text = $"{Mathf.Ceil(remainingTime)}초";
    }
}


using UnityEngine;
using System.Collections;
using System;

public class CookingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RecipeDefinition[] availableRecipes;
    
    public static CookingManager Instance { get; private set; }
    
    public event Action<RecipeDefinition, float> OnCookingStarted;
    public event Action<RecipeDefinition> OnCookingCompleted;
    public event Action<string> OnCookingFailed;
    
    private bool isCooking = false;
    private CookingTimer cookingTimer;
    
    // Inventory 싱글톤을 직접 참조
    private Inventory PlayerInventory => Inventory.Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            cookingTimer = new CookingTimer();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Inventory 인스턴스 확인
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance not found! Make sure Inventory exists in the scene.");
        }
        
        // 타이머 이벤트 구독
        cookingTimer.OnTimerCompleted += OnTimerCompleted;
    }
    
    void Update()
    {
        // 타이머 업데이트
        if (cookingTimer.IsRunning)
        {
            cookingTimer.UpdateTimer(Time.deltaTime);
        }
    }
    
    void OnDestroy()
    {
        if (cookingTimer != null)
        {
            cookingTimer.OnTimerCompleted -= OnTimerCompleted;
        }
    }
    
    public RecipeDefinition[] GetAvailableRecipes()
    {
        // RecipeUnlockManager가 있으면 해금된 레시피만 반환
        if (RecipeUnlockManager.Instance != null)
        {
            return RecipeUnlockManager.Instance.GetUnlockedRecipes();
        }
        
        // RecipeUnlockManager가 없을 때는 경고 로그 출력 후 빈 배열 반환
        Debug.LogWarning("RecipeUnlockManager.Instance가 null입니다! 해금된 레시피를 확인할 수 없습니다.");
        return new RecipeDefinition[0]; // 빈 배열 반환 (null 방지)
    }
    
    public bool CanCookRecipe(RecipeDefinition recipe)
    {
        if (isCooking) return false;
        
        // Inventory 인스턴스 확인
        if (PlayerInventory == null)
        {
            Debug.LogError("PlayerInventory is null!");
            return false;
        }
        
        // 요리 가능 범위 확인
        if (CookingInteractor.Instance == null || !CookingInteractor.Instance.IsPlayerInCookingRange())
        {
            return false;
        }
        
        // 레시피가 해금되어 있는지 확인
        if (RecipeUnlockManager.Instance != null && !RecipeUnlockManager.Instance.IsRecipeUnlocked(recipe))
        {
            return false;
        }
        
        foreach (var ingredient in recipe.RequiredIngredients)
        {
            if (!PlayerInventory.HasItem(ingredient.Ingredient, ingredient.RequiredAmount))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public void StartCooking(RecipeDefinition recipe)
    {
        // Inventory 인스턴스 확인
        if (PlayerInventory == null)
        {
            OnCookingFailed?.Invoke("인벤토리를 찾을 수 없습니다!");
            Debug.LogError("PlayerInventory is null!");
            return;
        }
        
        // 요리 가능 범위 확인
        if (CookingInteractor.Instance == null || !CookingInteractor.Instance.IsPlayerInCookingRange())
        {
            OnCookingFailed?.Invoke("요리 가능 범위가 아닙니다!");
            return;
        }
        
        // 레시피 해금 상태 확인
        if (RecipeUnlockManager.Instance != null && !RecipeUnlockManager.Instance.IsRecipeUnlocked(recipe))
        {
            OnCookingFailed?.Invoke("해금되지 않은 레시피입니다!");
            return;
        }
        
        // 재료 소모
        foreach (var ingredient in recipe.RequiredIngredients)
        {
            PlayerInventory.RemoveItem(ingredient.Ingredient, ingredient.RequiredAmount);
        }
        
        isCooking = true;
        cookingTimer.StartTimer(recipe, recipe.CookingTime);
        OnCookingStarted?.Invoke(recipe, recipe.CookingTime);
    }
    
    private void OnTimerCompleted(RecipeDefinition recipe)
    {
        // Inventory 인스턴스 확인
        if (PlayerInventory == null)
        {
            Debug.LogError("PlayerInventory is null when cooking completed!");
            isCooking = false;
            return;
        }
        
        // 요리 완성 - 결과물을 인벤토리에 추가
        PlayerInventory.AddItem(recipe.ResultDish, recipe.ResultAmount);
        
        isCooking = false;
        OnCookingCompleted?.Invoke(recipe);
    }
    
    public bool IsCooking => isCooking;
    public CookingTimer GetCookingTimer() => cookingTimer;
}

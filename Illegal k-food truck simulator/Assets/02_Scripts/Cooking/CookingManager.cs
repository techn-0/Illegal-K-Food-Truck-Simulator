using UnityEngine;
using System.Collections;
using System;

public class CookingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private RecipeDefinition[] availableRecipes;
    
    public static CookingManager Instance { get; private set; }
    
    public event Action<RecipeDefinition, float> OnCookingStarted;
    public event Action<RecipeDefinition> OnCookingCompleted;
    public event Action<string> OnCookingFailed;
    
    private bool isCooking = false;
    private CookingTimer cookingTimer;
    
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
        return availableRecipes;
    }
    
    public bool CanCookRecipe(RecipeDefinition recipe)
    {
        if (isCooking) return false;
        
        foreach (var ingredient in recipe.RequiredIngredients)
        {
            if (!playerInventory.HasItem(ingredient.Ingredient, ingredient.RequiredAmount))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public void StartCooking(RecipeDefinition recipe)
    {
        if (!CanCookRecipe(recipe))
        {
            OnCookingFailed?.Invoke("재료가 부족합니다!");
            return;
        }
        
        // 재료 소모
        foreach (var ingredient in recipe.RequiredIngredients)
        {
            playerInventory.RemoveItem(ingredient.Ingredient, ingredient.RequiredAmount);
        }
        
        isCooking = true;
        cookingTimer.StartTimer(recipe, recipe.CookingTime);
        OnCookingStarted?.Invoke(recipe, recipe.CookingTime);
    }
    
    private void OnTimerCompleted(RecipeDefinition recipe)
    {
        // 요리 완성 - 결과물을 인벤토리에 추가
        playerInventory.AddItem(recipe.ResultDish, recipe.ResultAmount);
        
        isCooking = false;
        OnCookingCompleted?.Invoke(recipe);
    }
    
    public bool IsCooking => isCooking;
    public CookingTimer GetCookingTimer() => cookingTimer;
}

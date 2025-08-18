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
    
    private Coroutine currentCookingCoroutine;
    private bool isCooking = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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
        currentCookingCoroutine = StartCoroutine(CookingCoroutine(recipe));
        OnCookingStarted?.Invoke(recipe, recipe.CookingTime);
    }
    
    private IEnumerator CookingCoroutine(RecipeDefinition recipe)
    {
        yield return new WaitForSeconds(recipe.CookingTime);
        
        // 요리 완성 - 결과물을 인벤토리에 추가
        playerInventory.AddItem(recipe.ResultDish, recipe.ResultAmount);
        
        isCooking = false;
        currentCookingCoroutine = null;
        OnCookingCompleted?.Invoke(recipe);
    }
    
    public bool IsCooking => isCooking;
}

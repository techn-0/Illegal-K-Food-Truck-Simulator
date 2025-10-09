using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 플레이어가 해금한 레시피들을 관리하는 매니저
/// 단일 책임: 레시피 해금 상태 관리
/// </summary>
public class RecipeUnlockManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    public static RecipeUnlockManager Instance { get; private set; }
    
    [Header("Default Unlocked Recipes")]
    [SerializeField] private RecipeDefinition[] defaultUnlockedRecipes; // 기본으로 해금된 레시피들
    
    private HashSet<RecipeDefinition> unlockedRecipes = new HashSet<RecipeDefinition>();
    
    /// <summary>
    /// 레시피가 해금될 때 호출되는 이벤트
    /// </summary>
    public static event Action<RecipeDefinition> OnRecipeUnlocked;
    
    /// <summary>
    /// 해금된 레시피 목록이 변경될 때 호출되는 이벤트
    /// </summary>
    public static event Action OnUnlockedRecipesChanged;
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 기본 레시피들 해금
        UnlockDefaultRecipes();
    }
    
    /// <summary>
    /// 기본으로 해금된 레시피들을 해금 상태로 설정
    /// </summary>
    private void UnlockDefaultRecipes()
    {
        if (defaultUnlockedRecipes == null) return;
        
        foreach (var recipe in defaultUnlockedRecipes)
        {
            if (recipe != null)
            {
                unlockedRecipes.Add(recipe);
            }
        }
        
        OnUnlockedRecipesChanged?.Invoke();
    }
    
    /// <summary>
    /// 레시피가 해금되어 있는지 확인
    /// </summary>
    /// <param name="recipe">확인할 레시피</param>
    /// <returns>해금 상태</returns>
    public bool IsRecipeUnlocked(RecipeDefinition recipe)
    {
        if (recipe == null) return false;
        return unlockedRecipes.Contains(recipe);
    }
    
    /// <summary>
    /// 레시피를 해금
    /// </summary>
    /// <param name="recipe">해금할 레시피</param>
    /// <returns>해금 성공 여부 (이미 해금된 경우 false)</returns>
    public bool UnlockRecipe(RecipeDefinition recipe)
    {
        if (recipe == null || IsRecipeUnlocked(recipe)) return false;
        
        unlockedRecipes.Add(recipe);
        OnRecipeUnlocked?.Invoke(recipe);
        OnUnlockedRecipesChanged?.Invoke();
        
        Debug.Log($"레시피 해금: {recipe.RecipeName}");
        return true;
    }
    
    /// <summary>
    /// 해금된 모든 레시피 배열로 반환
    /// </summary>
    /// <returns>해금된 레시피 배열</returns>
    public RecipeDefinition[] GetUnlockedRecipes()
    {
        var recipeArray = new RecipeDefinition[unlockedRecipes.Count];
        unlockedRecipes.CopyTo(recipeArray);
        return recipeArray;
    }
    
    /// <summary>
    /// 해금된 레시피 개수
    /// </summary>
    public int UnlockedRecipeCount => unlockedRecipes.Count;
}

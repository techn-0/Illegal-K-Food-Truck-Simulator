using UnityEngine;
using System;
using Minigame;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Cooking/Recipe Definition")]
public class RecipeDefinition : ScriptableObject
{
    [Header("Recipe Info")]
    [SerializeField] private string recipeId; // 레시피 고유 ID
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite dishImage;
    [SerializeField] private float cookingTime = 10f;
    [SerializeField] private int basePrice = 100;
    
    [Header("Ingredients")]
    [SerializeField] private RecipeIngredient[] requiredIngredients;
    
    [Header("Result")]
    [SerializeField] private ItemDefinition resultDish;
    [SerializeField] private int resultAmount = 1;
    
    [Header("Minigames")]
    [SerializeField] private MinigameId[] minigameSequence; // 순차적으로 진행될 미니게임
    
    public string RecipeId => recipeId;
    public string RecipeName => recipeName;
    public Sprite DishImage => dishImage;
    public float CookingTime => cookingTime;
    public int BasePrice => basePrice;
    public RecipeIngredient[] RequiredIngredients => requiredIngredients;
    public ItemDefinition ResultDish => resultDish;
    public int ResultAmount => resultAmount;
    public MinigameId[] MinigameSequence => minigameSequence;
    
    /// <summary>랭크에 따른 최종 가격 계산</summary>
    public int GetPriceByRank(char rank)
    {
        float multiplier = rank switch
        {
            'S' => 1.5f,
            'A' => 1.2f,
            'B' => 1.0f,
            'C' => 0.8f,
            'F' => 0.5f,
            _ => 1.0f
        };
        return Mathf.RoundToInt(basePrice * multiplier);
    }
}

[System.Serializable]
public class RecipeIngredient
{
    [SerializeField] private ItemDefinition ingredient;
    [SerializeField] private int requiredAmount;
    
    public ItemDefinition Ingredient => ingredient;
    public int RequiredAmount => requiredAmount;
}

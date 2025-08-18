using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Cooking/Recipe Definition")]
public class RecipeDefinition : ScriptableObject
{
    [Header("Recipe Info")]
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite dishImage;
    [SerializeField] private float cookingTime = 10f;
    [SerializeField] private int price = 100;
    
    [Header("Ingredients")]
    [SerializeField] private RecipeIngredient[] requiredIngredients;
    
    [Header("Result")]
    [SerializeField] private ItemDefinition resultDish;
    [SerializeField] private int resultAmount = 1;
    
    public string RecipeName => recipeName;
    public Sprite DishImage => dishImage;
    public float CookingTime => cookingTime;
    public int Price => price;
    public RecipeIngredient[] RequiredIngredients => requiredIngredients;
    public ItemDefinition ResultDish => resultDish;
    public int ResultAmount => resultAmount;
}

[System.Serializable]
public class RecipeIngredient
{
    [SerializeField] private ItemDefinition ingredient;
    [SerializeField] private int requiredAmount;
    
    public ItemDefinition Ingredient => ingredient;
    public int RequiredAmount => requiredAmount;
}

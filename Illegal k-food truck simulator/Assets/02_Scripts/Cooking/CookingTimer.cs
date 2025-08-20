using UnityEngine;
using System;

public class CookingTimer
{
    private float totalCookingTime;
    private float remainingTime;
    private bool isRunning;
    private RecipeDefinition currentRecipe;
    
    public event Action<float, float> OnTimerUpdated; // (remainingTime, progress)
    public event Action<RecipeDefinition> OnTimerCompleted;
    
    public bool IsRunning => isRunning;
    public float RemainingTime => remainingTime;
    public float TotalTime => totalCookingTime;
    public float Progress => isRunning ? 1f - (remainingTime / totalCookingTime) : 0f;
    public RecipeDefinition CurrentRecipe => currentRecipe;
    
    public void StartTimer(RecipeDefinition recipe, float cookingTime)
    {
        currentRecipe = recipe;
        totalCookingTime = cookingTime;
        remainingTime = cookingTime;
        isRunning = true;
    }
    
    public void UpdateTimer(float deltaTime)
    {
        if (!isRunning) return;
        
        remainingTime -= deltaTime;
        
        OnTimerUpdated?.Invoke(remainingTime, Progress);
        
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isRunning = false;
            OnTimerCompleted?.Invoke(currentRecipe);
        }
    }
    
    public void StopTimer()
    {
        isRunning = false;
        remainingTime = 0;
        currentRecipe = null;
    }
}
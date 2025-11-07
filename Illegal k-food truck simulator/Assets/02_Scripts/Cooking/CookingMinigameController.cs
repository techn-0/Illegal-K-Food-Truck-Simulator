using UnityEngine;
using Minigame;
using System.Collections.Generic;
using System;

/// <summary>
/// 조리 미니게임을 순차적으로 실행하고 결과를 관리
/// </summary>
public class CookingMinigameController : MonoBehaviour
{
    public static CookingMinigameController Instance { get; private set; }

    private RecipeDefinition currentRecipe;
    private int currentMinigameIndex;
    private List<MiniGameResult> minigameResults = new List<MiniGameResult>();
    private Action<char, int> onAllMinigamesCompleted;

    private void Awake()
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

    /// <summary>
    /// 레시피에 대한 미니게임 시퀀스 시작
    /// </summary>
    public void StartCookingSequence(RecipeDefinition recipe, Action<char, int> onCompleted)
    {
        currentRecipe = recipe;
        currentMinigameIndex = 0;
        minigameResults.Clear();
        onAllMinigamesCompleted = onCompleted;

        if (recipe.MinigameSequence == null || recipe.MinigameSequence.Length == 0)
        {
            Debug.LogError("레시피에 미니게임이 설정되지 않았습니다!");
            onCompleted?.Invoke('F', recipe.BasePrice);
            return;
        }

        StartNextMinigame();
    }

    private void StartNextMinigame()
    {
        if (currentMinigameIndex >= currentRecipe.MinigameSequence.Length)
        {
            // 모든 미니게임 완료
            FinishCooking();
            return;
        }

        MinigameId gameId = currentRecipe.MinigameSequence[currentMinigameIndex];
        MiniGameManager.Instance.StartMinigame(gameId, OnMinigameCompleted);
    }

    private void OnMinigameCompleted(MiniGameResult result)
    {
        minigameResults.Add(result);
        currentMinigameIndex++;
        
        // 다음 미니게임 시작
        StartNextMinigame();
    }

    private void FinishCooking()
    {
        // 평균 점수 계산
        float totalScore = 0f;
        foreach (var result in minigameResults)
        {
            totalScore += result.score;
        }
        float averageScore = totalScore / minigameResults.Count;

        // 최종 랭크 결정
        char finalRank = CalculateRank(averageScore);

        // 랭크에 따른 가격 계산
        int finalPrice = currentRecipe.GetPriceByRank(finalRank);

        Debug.Log($"조리 완료! 평균 점수: {averageScore:F1}, 랭크: {finalRank}, 가격: {finalPrice}");

        // 재료 소비
        ConsumeIngredients();

        // 완성된 요리를 인벤토리에 추가
        if (Inventory.Instance != null)
        {
            Inventory.Instance.AddItem(currentRecipe.ResultDish, currentRecipe.ResultAmount);
        }

        // 콜백 호출
        onAllMinigamesCompleted?.Invoke(finalRank, finalPrice);

        // 초기화
        currentRecipe = null;
        minigameResults.Clear();
    }

    private void ConsumeIngredients()
    {
        if (Inventory.Instance == null || currentRecipe == null) return;

        foreach (var ingredient in currentRecipe.RequiredIngredients)
        {
            Inventory.Instance.RemoveItem(ingredient.Ingredient, ingredient.RequiredAmount);
        }
    }

    private char CalculateRank(float score)
    {
        if (score >= 90f) return 'S';
        if (score >= 75f) return 'A';
        if (score >= 60f) return 'B';
        if (score >= 40f) return 'C';
        return 'F';
    }
}


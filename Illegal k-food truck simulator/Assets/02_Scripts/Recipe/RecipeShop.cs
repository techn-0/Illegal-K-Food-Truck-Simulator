using UnityEngine;

/// <summary>
/// 상점에서 판매할 레시피 정보
/// </summary>
[System.Serializable]
public class ShopRecipeItem
{
    [SerializeField] private RecipeDefinition recipe;
    [SerializeField] private int price;
    
    public RecipeDefinition Recipe => recipe;
    public int Price => price;
}

/// <summary>
/// 레시피 상점의 데이터를 관리하는 클래스
/// 단일 책임: 상점에서 판매할 레시피와 가격 정보 관리
/// </summary>
public class RecipeShop : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private ShopRecipeItem[] shopRecipes; // 상점에서 판매할 레시피들
    
    /// <summary>
    /// 상점에서 판매하는 모든 레시피 아이템 반환
    /// </summary>
    public ShopRecipeItem[] GetShopRecipes()
    {
        return shopRecipes;
    }
    
    /// <summary>
    /// 레시피 구매 시도
    /// </summary>
    /// <param name="recipe">구매할 레시피</param>
    /// <returns>구매 성공 여부</returns>
    public bool TryPurchaseRecipe(RecipeDefinition recipe)
    {
        if (recipe == null) return false;
        
        // 해당 레시피가 상점에 있는지 확인
        ShopRecipeItem shopItem = FindShopItem(recipe);
        if (shopItem == null) return false;
        
        // 이미 해금된 레시피인지 확인
        if (RecipeUnlockManager.Instance.IsRecipeUnlocked(recipe))
        {
            Debug.LogWarning($"이미 해금된 레시피입니다: {recipe.RecipeName}");
            return false;
        }
        
        // 돈이 충분한지 확인
        if (!PlayerMoneyManager.Instance.CanAfford(shopItem.Price))
        {
            Debug.LogWarning($"골드가 부족합니다. 필요: {shopItem.Price}골드");
            return false;
        }
        
        // 돈 차감
        if (!PlayerMoneyManager.Instance.SpendMoney(shopItem.Price))
        {
            Debug.LogError("돈 차감 중 오류가 발생했습니다.");
            return false;
        }
        
        // 레시피 해금
        bool unlockSuccess = RecipeUnlockManager.Instance.UnlockRecipe(recipe);
        if (!unlockSuccess)
        {
            // 해금 실패 시 돈 환불
            PlayerMoneyManager.Instance.AddMoney(shopItem.Price);
            Debug.LogError("레시피 해금 중 오류가 발생했습니다.");
            return false;
        }
        
        Debug.Log($"레시피 구매 완료: {recipe.RecipeName} ({shopItem.Price}골드)");
        return true;
    }
    
    /// <summary>
    /// 특정 레시피가 구매 가능한지 확인
    /// </summary>
    /// <param name="recipe">확인할 레시피</param>
    /// <returns>구매 가능 여부</returns>
    public bool CanPurchaseRecipe(RecipeDefinition recipe)
    {
        if (recipe == null) return false;
        
        // 상점에 있는 레시피인지 확인
        ShopRecipeItem shopItem = FindShopItem(recipe);
        if (shopItem == null) return false;
        
        // 이미 해금된 레시피는 구매 불가
        if (RecipeUnlockManager.Instance.IsRecipeUnlocked(recipe)) return false;
        
        // 돈이 충분한지 확인
        return PlayerMoneyManager.Instance.CanAfford(shopItem.Price);
    }
    
    /// <summary>
    /// 레시피에 해당하는 상점 아이템 찾기
    /// </summary>
    private ShopRecipeItem FindShopItem(RecipeDefinition recipe)
    {
        if (shopRecipes == null) return null;
        
        foreach (var shopItem in shopRecipes)
        {
            if (shopItem.Recipe == recipe)
            {
                return shopItem;
            }
        }
        
        return null;
    }
}

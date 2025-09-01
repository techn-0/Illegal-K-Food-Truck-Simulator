using UnityEngine;

/// <summary>
/// 판매 관련 로직을 처리하는 서비스 클래스
/// 단일 책임: 판매 가능 여부 확인 및 판매 처리만 담당
/// </summary>
public class SaleService : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    public static SaleService Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private Inventory playerInventory;
    
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
        // 인벤토리가 할당되지 않았다면 찾아서 할당
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<Inventory>();
        }
    }
    
    /// <summary>
    /// 특정 아이템을 판매할 수 있는지 확인
    /// </summary>
    /// <param name="item">판매할 아이템</param>
    /// <param name="quantity">판매할 수량</param>
    /// <returns>판매 가능 여부</returns>
    public bool CanSell(ItemDefinition item, int quantity)
    {
        if (item == null || quantity <= 0) return false;
        if (playerInventory == null) return false;
        
        return playerInventory.CountItem(item) >= quantity;
    }
    
    /// <summary>
    /// 아이템을 판매하고 돈을 지급
    /// </summary>
    /// <param name="item">판매할 아이템</param>
    /// <param name="quantity">판매할 수량</param>
    /// <param name="pricePerUnit">단위당 가격</param>
    /// <returns>판매 성공 여부</returns>
    public bool ProcessSale(ItemDefinition item, int quantity, int pricePerUnit)
    {
        if (!CanSell(item, quantity)) return false;
        
        // 인벤토리에서 아이템 제거
        int removedAmount = playerInventory.RemoveItem(item, quantity);
        
        if (removedAmount != quantity)
        {
            Debug.LogError($"판매 처리 중 오류: 요청 수량({quantity})과 제거된 수량({removedAmount})이 다릅니다.");
            return false;
        }
        
        // 돈 지급
        int totalPrice = quantity * pricePerUnit;
        bool moneyAdded = PlayerMoneyManager.Instance.AddMoney(totalPrice);
        
        if (!moneyAdded)
        {
            Debug.LogError("돈 지급 중 오류가 발생했습니다.");
            // 실패시 아이템을 다시 인벤토리에 추가 (롤백)
            playerInventory.Add(item, removedAmount);
            return false;
        }
        
        Debug.Log($"{item.DisplayName} x{quantity} 판매 완료! 수익: {totalPrice}원");
        return true;
    }
    
    /// <summary>
    /// 레시피의 결과물을 판매
    /// </summary>
    /// <param name="recipe">판매할 레시피</param>
    /// <param name="quantity">판매할 수량</param>
    /// <returns>판매 성공 여부</returns>
    public bool SellRecipeResult(RecipeDefinition recipe, int quantity)
    {
        if (recipe == null || recipe.ResultDish == null) return false;
        
        return ProcessSale(recipe.ResultDish, quantity, recipe.Price);
    }
    
    /// <summary>
    /// 정적 메서드: 아이템 판매 시도 (프리팹에서 호출 가능)
    /// </summary>
    /// <param name="item">판매할 아이템</param>
    /// <param name="quantity">판매할 수량</param>
    /// <returns>판매 성공 여부</returns>
    public static bool TrySellItem(ItemDefinition item, int quantity)
    {
        if (Instance == null)
        {
            Debug.LogError("SaleService 인스턴스를 찾을 수 없습니다.");
            return false;
        }
        
        // 레시피 정의에서 해당 아이템의 가격을 찾아서 판매
        RecipeDefinition[] allRecipes = Resources.FindObjectsOfTypeAll<RecipeDefinition>();
        foreach (var recipe in allRecipes)
        {
            if (recipe.ResultDish == item)
            {
                return Instance.SellRecipeResult(recipe, quantity);
            }
        }

        Debug.LogWarning($"{item.DisplayName}에 대한 레시피를 찾을 수 없습니다.");
        return false;
    }
}

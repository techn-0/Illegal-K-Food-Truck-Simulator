using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class OrderUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemQuantity;
    public Button acceptOrderButton; // 이름 변경: sellButton -> acceptOrderButton
    
    [Header("Timer UI")]
    public Image timerFillImage;
    public TMP_Text timerText;
    public GameObject timerPanel; // Panel_Timer
    
    [Header("Order UI Container (Optional)")]
    public GameObject orderUIContainer; // Panel_Order

    private Camera mainCamera;
    private RecipeDefinition orderedRecipe; // ItemDefinition 대신 RecipeDefinition 사용
    private int orderQuantity;
    private CustomerOrderSystem customerOrderSystem;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        mainCamera = Camera.main;
        customerOrderSystem = GetComponentInParent<CustomerOrderSystem>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        UpdateOrderUIVisibility();
        UpdateAcceptButton();
        UpdateTimerUI();
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(-direction);
        }
    }

    /// <summary>레시피로 UI 설정</summary>
    public void SetupWithRecipe(RecipeDefinition recipe, int quantity)
    {
        orderedRecipe = recipe;
        orderQuantity = quantity;
        
        if (recipe != null)
        {
            itemIcon.sprite = recipe.DishImage;
            itemName.text = recipe.RecipeName;
            itemQuantity.text = quantity.ToString();
        }
    }

    /// <summary>기존 ItemDefinition 호환성 유지</summary>
    public void Setup(ItemDefinition item, int quantity)
    {
        orderQuantity = quantity;
        
        // CustomerOrderSystem에서 직접 레시피 가져오기
        if (customerOrderSystem == null)
            customerOrderSystem = GetComponentInParent<CustomerOrderSystem>();
        
        if (customerOrderSystem != null)
        {
            orderedRecipe = customerOrderSystem.GetOrderedRecipe();
        }
        
        // orderedRecipe가 없으면 아이템으로 찾기 시도
        if (orderedRecipe == null && item != null)
        {
            orderedRecipe = FindRecipeForItem(item);
        }
        
        // UI 업데이트: 레시피 정보가 있으면 레시피 정보 사용, 없으면 아이템 정보 사용
        if (orderedRecipe != null)
        {
            itemIcon.sprite = orderedRecipe.DishImage;
            itemName.text = orderedRecipe.RecipeName;
            itemQuantity.text = quantity.ToString();
            Debug.Log($"[OrderUI] Setup 완료: {orderedRecipe.RecipeName} (레시피 정보 사용)");
        }
        else if (item != null)
        {
            itemIcon.sprite = item.Icon;
            itemName.text = item.DisplayName;
            itemQuantity.text = quantity.ToString();
            Debug.Log($"[OrderUI] Setup 완료: {item.DisplayName} (아이템 정보 사용)");
        }
    }

    private RecipeDefinition FindRecipeForItem(ItemDefinition item)
    {
        if (CookingManager.Instance == null) return null;
        
        var recipes = CookingManager.Instance.GetAvailableRecipes();
        foreach (var recipe in recipes)
        {
            if (recipe.ResultDish == item)
                return recipe;
        }
        return null;
    }

    private void UpdateOrderUIVisibility()
    {
        if (customerOrderSystem == null) return;

        bool isFirstInQueue = OrderManager.Instance != null && OrderManager.Instance.IsFirstInQueue(customerOrderSystem);
        var currentOrder = customerOrderSystem.GetCurrentOrder();
        bool isOrderActive = currentOrder != null && currentOrder.IsActive;

        bool showFullOrder = isFirstInQueue || isOrderActive;
        bool showTimerOnly = customerOrderSystem.IsWaitingInQueue() && !isFirstInQueue;

        if (orderUIContainer != null) orderUIContainer.SetActive(showFullOrder);
        if (timerPanel != null) timerPanel.SetActive(showFullOrder || showTimerOnly);

        // 전체 UI 컨테이너의 활성화/비활성화 로직
        GameObject mainContainer = orderUIContainer != null ? orderUIContainer.transform.parent.gameObject : gameObject;
        bool shouldShowAnything = showFullOrder || showTimerOnly;

        if (shouldShowAnything && !mainContainer.activeSelf)
        {
            mainContainer.SetActive(true);
            mainContainer.transform.localScale = Vector3.one * 0.8f;
            canvasGroup.alpha = 0f;
            mainContainer.transform.DOScale(1f, 0.25f);
            canvasGroup.DOFade(1f, 0.25f);
        }
        else if (!shouldShowAnything && mainContainer.activeSelf)
        {
            mainContainer.transform.DOScale(0.8f, 0.15f);
            canvasGroup.DOFade(0f, 0.15f).OnComplete(() => mainContainer.SetActive(false));
        }
    }

    public void HandleOrderPlacement()
    {
        UpdateOrderUIVisibility();
        UpdateTimerUI();
    }

    private void UpdateAcceptButton()
    {
        if (orderedRecipe == null || Inventory.Instance == null)
        {
            if (acceptOrderButton != null)
                acceptOrderButton.interactable = false;
            return;
        }

        // 재료가 충분한지 확인
        bool hasIngredients = HasRequiredIngredients();
        if (acceptOrderButton != null)
        {
            acceptOrderButton.interactable = hasIngredients;

            var colors = acceptOrderButton.colors;
            colors.normalColor = hasIngredients ? Color.white : Color.gray;
            acceptOrderButton.colors = colors;
        }
    }

    private bool HasRequiredIngredients()
    {
        if (orderedRecipe == null || Inventory.Instance == null) return false;

        foreach (var ingredient in orderedRecipe.RequiredIngredients)
        {
            if (!Inventory.Instance.HasItem(ingredient.Ingredient, ingredient.RequiredAmount))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateTimerUI()
    {
        if (customerOrderSystem == null) return;

        bool isFirstInQueue = OrderManager.Instance != null && OrderManager.Instance.IsFirstInQueue(customerOrderSystem);
        bool isWaitingInQueue = customerOrderSystem.IsWaitingInQueue();
        var currentOrder = customerOrderSystem.GetCurrentOrder();

        // 첫 번째 손님이라면 대기열 타이머 대신 음식 대기 타이머를 표시해야 한다.
        bool useQueueTimer = isWaitingInQueue && !isFirstInQueue;

        if (useQueueTimer)
        {
            float remainingQueueTime = customerOrderSystem.GetRemainingQueueTime();
            float queueTimeLimit = customerOrderSystem.queueWaitTimeLimit;
            float queueTimeRatio = Mathf.Clamp01(remainingQueueTime / queueTimeLimit);
            
            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = queueTimeRatio;
                
                if (queueTimeRatio > 0.5f)
                    timerFillImage.color = Color.cyan;
                else if (queueTimeRatio > 0.25f)
                    timerFillImage.color = Color.yellow;
                else
                    timerFillImage.color = Color.red;
            }
            
            if (timerText != null)
            {
                int remainingSeconds = Mathf.CeilToInt(remainingQueueTime);
                timerText.text = remainingSeconds.ToString();
            }
        }
        else
        {
            // 음식 대기 타이머 (주문 활성화 전이라도 첫 번째 손님이면 foodWaitTimeLimit 기준으로 표시)
            float remainingFoodTime;
            float foodTimeLimit = customerOrderSystem.foodWaitTimeLimit;
            float timeRatio;

            if (currentOrder != null && currentOrder.IsActive)
            {
                remainingFoodTime = currentOrder.RemainingTime;
                timeRatio = currentOrder.GetTimeRatio();
            }
            else
            {
                // 활성화 전: 아직 주문 수락 안 했더라도 전체 제한 시간을 그대로 보여줌
                remainingFoodTime = foodTimeLimit; // 시작 상태 (감소 로직 없음)
                timeRatio = 1f;
            }

            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = timeRatio;
                if (timeRatio > 0.5f)
                    timerFillImage.color = Color.green;
                else if (timeRatio > 0.25f)
                    timerFillImage.color = Color.yellow;
                else
                    timerFillImage.color = Color.red;
            }

            if (timerText != null)
            {
                int remainingSeconds = Mathf.CeilToInt(remainingFoodTime);
                timerText.text = remainingSeconds.ToString();
            }
        }
    }

    /// <summary>주문 수락 버튼 클릭 시 호출 (미니게임 시작)</summary>
    public void OnAcceptOrderButtonClicked()
    {
        if (orderedRecipe == null || CookingMinigameController.Instance == null) return;

        if (!HasRequiredIngredients())
        {
            Debug.Log("재료가 부족합니다!");
            return;
        }

        // 미니게임 시퀀스 시작
        CookingMinigameController.Instance.StartCookingSequence(orderedRecipe, OnCookingCompleted);
    }

    private void OnCookingCompleted(char rank, int finalPrice)
    {
        Debug.Log($"조리 완료! 랭크: {rank}, 판매가: {finalPrice}");

        // 판매 처리
        if (SaleService.Instance != null)
        {
            SaleService.Instance.ProcessSale(orderedRecipe.ResultDish, orderQuantity, finalPrice);
        }

        // 손님에게 음식 전달 완료
        if (customerOrderSystem != null)
        {
            customerOrderSystem.OnOrderCompleted();
        }
    }

    // 기존 메서드 호환성 유지
    public void OnSellButtonClicked()
    {
        OnAcceptOrderButtonClicked();
    }
}

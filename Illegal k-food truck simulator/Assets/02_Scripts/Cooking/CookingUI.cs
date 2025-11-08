using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CookingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cookingPanel;
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private GameObject recipeItemPrefab;
    
    [Header("Business Button")]
    [SerializeField] private Button businessToggleButton;
    [SerializeField] private TextMeshProUGUI businessButtonText;
    
    [Header("Cooking Timer UI")]
    [SerializeField] private GameObject cookingTimerPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI cookingDishNameText;
    
    private CookingManager cookingManager;
    private CookingTimer cookingTimer;
    private bool playerInRange = false;
    
    private CanvasGroup panelCanvasGroup;
    private CanvasGroup timerCanvasGroup;
    
    void Awake()
    {
        cookingManager = CookingManager.Instance;
        
        // CanvasGroup 설정
        if (cookingPanel != null)
        {
            panelCanvasGroup = cookingPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = cookingPanel.AddComponent<CanvasGroup>();
        }
        
        if (cookingTimerPanel != null)
        {
            timerCanvasGroup = cookingTimerPanel.GetComponent<CanvasGroup>();
            if (timerCanvasGroup == null)
                timerCanvasGroup = cookingTimerPanel.AddComponent<CanvasGroup>();
        }
    }
    
    void Start()
    {
        // 쿠킹 매니저 이벤트 구독
        cookingManager.OnCookingStarted += OnCookingStarted;
        cookingManager.OnCookingCompleted += OnCookingCompleted;
        cookingManager.OnCookingFailed += OnCookingFailed;
        
        // 쿠킹 타이머 가져오기 및 이벤트 구독
        cookingTimer = cookingManager.GetCookingTimer();
        cookingTimer.OnTimerUpdated += OnTimerUpdated;
        
        // 레시피 해금 이벤트 구독
        RecipeUnlockManager.OnUnlockedRecipesChanged += OnUnlockedRecipesChanged;
        
        // CookingInteractor 이벤트 구독
        if (CookingInteractor.Instance != null)
        {
            CookingInteractor.Instance.OnPlayerRangeChanged += OnPlayerRangeChanged;
        }
        
        // BusinessManager 이벤트 구독
        BusinessManager.OnBusinessStateChanged += OnBusinessStateChanged;
        
        // 장사 버튼 클릭 이벤트 연결 (코드로만 관리)
        if (businessToggleButton != null)
        {
            // 기존 리스너를 모두 제거하고 새로 추가 (Inspector 설정과 중복 방지)
            businessToggleButton.onClick.RemoveAllListeners();
            businessToggleButton.onClick.AddListener(OnBusinessButtonClicked);
        }
        
        // 초기 상태 설정
        // cookingPanel.SetActive(false);
        // cookingTimerPanel.SetActive(false);

        // 레시피 목록 생성
        CreateRecipeList();
        
        // 버튼 초기 상태 업데이트
        UpdateBusinessButton();
    }
    
    void OnEnable()
    {
        // 쿠킹 패널 애니메이션
        if (cookingPanel != null && panelCanvasGroup != null)
        {
            cookingPanel.transform.localScale = Vector3.one * 0.9f;
            panelCanvasGroup.alpha = 0f;
            cookingPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            panelCanvasGroup.DOFade(1f, 0.3f);
        }
    }
    
    void OnDestroy()
    {
        if (cookingManager != null)
        {
            cookingManager.OnCookingStarted -= OnCookingStarted;
            cookingManager.OnCookingCompleted -= OnCookingCompleted;
            cookingManager.OnCookingFailed -= OnCookingFailed;
        }
        
        if (cookingTimer != null)
        {
            cookingTimer.OnTimerUpdated -= OnTimerUpdated;
        }
        
        // 레시피 해금 이벤트 구독 해제
        RecipeUnlockManager.OnUnlockedRecipesChanged -= OnUnlockedRecipesChanged;
        
        // CookingInteractor 이벤트 구독 해제
        if (CookingInteractor.Instance != null)
        {
            CookingInteractor.Instance.OnPlayerRangeChanged -= OnPlayerRangeChanged;
        }
        
        // BusinessManager 이벤트 구독 해제
        BusinessManager.OnBusinessStateChanged -= OnBusinessStateChanged;
    }
    
    private void CreateRecipeList()
    {
        // 기존 레시피 아이템들 제거
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }
        
        var recipes = cookingManager.GetAvailableRecipes();
        
        foreach (var recipe in recipes)
        {
            GameObject recipeItem = Instantiate(recipeItemPrefab, recipeContainer);
            RecipeItemUI recipeItemUI = recipeItem.GetComponent<RecipeItemUI>();
            recipeItemUI.SetupRecipe(recipe);
        }
    }
    
    private void OnCookingStarted(RecipeDefinition recipe, float cookingTime)
    {
        cookingTimerPanel.SetActive(true);
        cookingTimerPanel.transform.localScale = Vector3.one * 0.8f;
        timerCanvasGroup.alpha = 0f;
        cookingTimerPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        timerCanvasGroup.DOFade(1f, 0.3f);
        
        cookingDishNameText.text = $"{recipe.RecipeName} 조리 중...";
        timerSlider.value = 0f; // 진행도는 0부터 시작
    }
    
    private void OnCookingCompleted(RecipeDefinition recipe)
    {
        cookingTimerPanel.transform.DOScale(0.8f, 0.2f);
        timerCanvasGroup.DOFade(0f, 0.2f).OnComplete(() => cookingTimerPanel.SetActive(false));
        
        // 완성 메시지 표시 (선택사항)
        Debug.Log($"{recipe.RecipeName} 조리 완성!");
    }
    
    private void OnCookingFailed(string message)
    {
        Debug.Log(message);
        // 실패 메시지 UI 표시 (선택사항)
    }
    
    private void OnTimerUpdated(float remainingTime, float progress)
    {
        // 타이머 UI 업데이트
        timerSlider.value = progress;
        timerText.text = $"{Mathf.Ceil(remainingTime)}초";
    }
    
    /// <summary>
    /// 해금된 레시피가 변경되었을 때 호출되는 이벤트 핸들러
    /// </summary>
    private void OnUnlockedRecipesChanged()
    {
        CreateRecipeList();
    }
    
    private void OnPlayerRangeChanged(bool inRange)
    {
        playerInRange = inRange;
        UpdateBusinessButton();
    }
    
    private void OnBusinessStateChanged(bool isActive)
    {
        UpdateBusinessButton();
    }
    
    private void UpdateBusinessButton()
    {
        if (businessToggleButton == null || businessButtonText == null)
            return;
        
        // CookingInteractor에서 직접 범위 상태 확인
        bool inRange = CookingInteractor.Instance != null && CookingInteractor.Instance.IsPlayerInCookingRange();
        
        // 플레이어가 범위 내에 있을 때만 버튼 활성화
        businessToggleButton.interactable = inRange;
        
        // 장사 상태에 따라 텍스트 변경
        if (BusinessManager.IsBusinessActive)
        {
            businessButtonText.text = "장사 종료";
        }
        else
        {
            businessButtonText.text = "장사 시작";
        }
        
        // 버튼 색상 변경 (선택사항)
        var colors = businessToggleButton.colors;
        colors.normalColor = inRange ? Color.white : Color.gray;
        businessToggleButton.colors = colors;
    }
    
    private void OnBusinessButtonClicked()
    {
        BusinessManager.ToggleBusinessState();
    }
}

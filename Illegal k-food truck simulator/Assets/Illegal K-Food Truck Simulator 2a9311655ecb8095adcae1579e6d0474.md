# Illegal K-Food Truck Simulator

# 🐔불법 K-푸드 트럭 시뮬레이터

> 닭 대신 비둘기를 파는 불법 푸드트럭 게임
> 

---

# 개요

## 프로젝트 개요

| 항목 | 내용 |
| --- | --- |
| **개발 기간** | 2025.09-2025.11 |
| **엔진** | Unity 6 |
| **장르** | 라이트 경영 + 생활 시뮬레이션(+ 미니 스텔스 추가 예정) |
| **시점 / 톤** | 쿼터뷰 / 로우폴리 병맛 유머 |
| 개발 인원 | 1인 개발 |
| **모티브** | 초등학교 시절 도시괴담 “학교 앞 치킨은 비둘기로 만든다” |
| **GitHub** | [https://github.com/techn-0/Illegal-K-Food-Truck-Simulator](https://github.com/techn-0/Illegal-K-Food-Truck-Simulator) |
| **플레이 영상** | [https://youtu.be/DVLwJ0lvbDw?si=6gKffxfFAceazt7A](https://youtu.be/DVLwJ0lvbDw?si=6gKffxfFAceazt7A) |

---

## 스토리 개요

플레이어는 어느 날 꿈속에서 **정체불명의 할머니**에게  **치킨 레시피**를 전수받는다.

비법의 핵심은 닭 대신 비둘기를 튀기는 것

도시에서 비둘기를 잡아 치킨으로 속여 팔며 돈을 벌수록,

도시는 점점 **비둘기**로 가득해지는데…

99일을 버티면 할머니와 마을의 진실이 드러난다….

![Illegal k-food truck simulator 2025-11-10 오후 11_57_46.png](Illegal_k-food_truck_simulator_2025-11-10_%EC%98%A4%ED%9B%84_11_57_46.png)

---

# 시스템

## 시스템 구조 요약

- 스크립트 디렉터리  전체구조
    
    ```markdown
    Assets/02_Scripts
    ├── BirdMover.cs
    ├── CarController.cs
    ├── CursorManager.cs
    ├── GameManager.cs
    ├── GameSave.cs
    ├── SaveManager.cs
    ├── Business
    │   └── BusinessManager.cs
    ├── Cooking
    │   ├── CookingInteractor.cs
    │   ├── CookingManager.cs
    │   ├── CookingMinigameController.cs
    │   ├── CookingTimer.cs
    │   ├── CookingUI.cs
    │   ├── IngredientSlotUI.cs
    │   ├── RecipeDefinition.cs
    │   └── RecipeItemUI.cs
    ├── Customer
    │   ├── CustomerOrder.cs
    │   ├── CustomerOrderSystem.cs
    │   ├── OrderManager.cs
    │   └── OrderUI.cs
    ├── Dialogue
    │   ├── ChoiceParser.cs
    │   ├── CSVLoader.cs
    │   ├── DialogueData.cs
    │   ├── DialogueManager.cs
    │   ├── DialogueTarget.cs
    │   ├── DialogueTester.cs
    │   ├── DialogueView.cs
    │   ├── example_dialogue.csv
    │   ├── GRANDMA.csv
    │   ├── PlayerDialogueInteractor.cs
    │   └── SceneLoader.cs
    ├── Inventory
    │   ├── Inventory.cs
    │   ├── InventorySlot.cs
    │   ├── InventoryTester.cs
    │   ├── InventoryView.cs
    │   └── ItemSlotView.cs
    ├── Items
    │   ├── ItemDefinition.cs
    │   ├── ItemShop.cs
    │   ├── ItemShopInteractor.cs
    │   ├── ItemShopItemUI.cs
    │   ├── ItemShopUI.cs
    │   ├── ItemType.cs
    │   └── PickupTarget.cs
    ├── MainMenu
    │   └── BT_MoveScen.cs
    ├── Minigame
    │   ├── Games
    │   ├── IMinigame.cs
    │   ├── MinigameBase.cs
    │   ├── MinigameId.cs
    │   ├── MiniGameManager.cs
    │   ├── MinigameParametersBase.cs
    │   ├── MiniGameResult.cs
    │   ├── MinigameResultItemUI.cs
    │   ├── MinigameResultUI.cs
    │   ├── MinigameTestTrigger.cs
    │   └── README.md
    ├── Player
    │   ├── BedInteractor.cs
    │   ├── PlayerAnimationModel.cs
    │   ├── PlayerAnimationPresenter.cs
    │   ├── PlayerAnimationView.cs
    │   ├── PlayerController.cs
    │   ├── PlayerMoneyManager.cs
    │   └── PlayerPickupInteractor.cs
    ├── Recipe
    │   ├── RecipeShop.cs
    │   ├── RecipeShopInteractor.cs
    │   ├── RecipeShopItemUI.cs
    │   ├── RecipeShopUI.cs
    │   └── RecipeUnlockManager.cs
    ├── Sales
    │   └── SaleService.cs
    └── UI
        ├── DayDisplayUI.cs
        ├── EndDayUI.cs
        └── MoneyDisplayUI.cs
    ```
    

### 핵심 스크립트 요약

| 시스템 | 핵심 클래스 | 설명 |
| --- | --- | --- |
| **인벤토리** | `Inventory.cs`, `InventorySlot.cs` | 슬롯 기반 스택형 인벤토리의 추가/제거·조회 및 저장용 직렬화 제공 |
| **요리 미니게임** | `CookingManager`, `MinigameBase`, `MiniGameManager` | 레시피 잠금·재료 보유 검사 후 재료 소비, 요리 미니게임 진행 및 완료 처리 |
| **레시피 해금** | `RecipeUnlockManager` | ScriptableObject ID 매핑 기반 레시피 잠금/해제 및 복원 관리 |
| **판매 / 경제** | `SaleService`, `PlayerMoneyManager` | 판매 가능 판정, 거래 처리
판매시 해당 물품 인벤토리에서 치감, 수익 반영 |
| **저장 / 불러오기** | `SaveManager`, `GameManager` | 
JSON으로 게임 저장, 씬 로드 시 인벤토리, 레시피·재화 복원  |

## **주요 시스템**

**▶ 플레이어**

- 이동: `WASD`
- 상호작용: `E` (대화, 상점, 요리 등)
- 인벤토리: `I`
- 요리 UI: `C`
- 대화 진행: `Space` 또는 마우스 클릭

**▶ 인벤토리 / 아이템**

- `Inventory`, `InventorySlot`, `InventoryView`
- 아이템 스택 합치기·분리 가능
- 변경 이벤트(`OnChanged`)로 UI 자동 갱신

**▶ 요리 / 레시피**

- 재료 조합 → 조리 시간 → 결과물 → 판매
- 비둘기 고기를 닭 슬롯에 대체 사용 가능

**▶ 손님 / 판매**

- `CustomerOrderSystem`, `OrderManager`, `SaleService`
- 장사 시작 시 주변의 손님들이 가까운 순으로 큐에 등록됨
- 손님이 줄 서서 주문, 제한 시간 내 요리 제공
- 제한 시간 내 제공 실패 시 손님이 떠남

**▶ 상점 / 돈**

- `ItemShop` 및 `ItemShopUI`
- 구매 시 돈 차감 후 인벤토리에 아이템 지급 / 레시피 해금
- `PlayerMoneyManager`로 자금 통합 관리

**▶ 대화**

- `DialogueManager`, `CSVLoader`, `ChoiceParser`
- CSV 기반 분기 대화
- 선택지별 대화 분기 가능
- 대화 보상으로 아이템, 레시피, 돈 할당 가능

---

## **시스템 간 연동 구조**

| 이벤트 흐름 | 데이터 흐름 |
| --- | --- |
| `RecipeUnlockManager` → `CookingUI` | 해금된 레시피 목록 표시 |
| `CookingUI` → `CookingManager` | 플레이어가 요리 선택 |
| `CookingManager` → `MiniGameManager` | 미니게임 실행 및 결과 판정 |
| `MiniGameResult` → `SaleService` | 완성품 판매 가능 여부 확인 |
| `SaleService` → `PlayerMoneyManager` → `MoneyDisplayUI` | 수익 반영 및 UI 업데이트 |
| `GameManager` ↔ `SaveManager` | 저장 / 불러오기 처리 |

## **게임 시스템 플로우**

![Untitled (4).png](90c51114-c6d9-4518-9239-35b9bbe85b61.png)

---

# 📌 코드 하이라이트

## 1. 손님 대기열 — 거리 정렬 및 위치 계산 (`OrderManager.cs`)

> **핵심 포인트:** 동적으로 추가되는 손님들을 트럭 기준 거리순으로 실시간 정렬하고, 각 손님에게 줄 서기 위치를 자동 계산해 NavMesh 이동 목표로 전달

```csharp
public void EnqueueCustomer(CustomerOrderSystem customer)
{
    if (customer == null || _queuedCustomers.Contains(customer)) return;

    _queuedCustomers.Add(customer);

    // 거리 기반 정렬 옵션이 켜져 있으면 트럭과 가까운 순으로 재정렬
    if (sortByDistance && OrderPoint != null)
        SortQueueByDistance();

    RebuildQueue();          // List → Queue 재구성
    UpdateQueuePositions();  // 전체 손님 위치 재배분
}

private void SortQueueByDistance()
{
    _queuedCustomers = _queuedCustomers
        .OrderBy(c => Vector3.Distance(c.transform.position, OrderPoint.position))
        .ToList();
}

private Vector3 CalculateQueuePosition(int queueIndex)
{
    // 트럭 방향(forward/right/up)을 로컬 queueDirection에 투영해 월드 공간 방향 계산
    Vector3 actualDir = OrderPoint.forward  * queueDirection.z
                      + OrderPoint.right    * queueDirection.x
                      + OrderPoint.up       * queueDirection.y;

    // 첫 번째 손님은 OrderPoint 바로 앞, 이후 손님은 간격(customerSpacing)씩 뒤로 배치
    Vector3 pos = OrderPoint.position;
    if (queueIndex > 0)
        pos -= actualDir.normalized * (queueIndex * customerSpacing);

    return pos;
}
```

**설계 의도**
- 손님이 추가될 때마다 거리순 재정렬 + Queue 재구성을 하나의 흐름으로 묶어 항상 일관된 줄 서기 상태를 보장
- `queueDirection`을 월드 좌표가 아닌 트럭의 로컬 방향으로 계산해, 트럭 배치 방향에 상관없이 올바른 줄 방향이 자동 적용됨

---

## 2. 스택형 인벤토리 추가 로직 (`Inventory.cs`)

> **핵심 포인트:** 기존 슬롯 스택 채우기 → 빈 슬롯 확장 순서로 아이템을 최적 배치하며, 변경 시 이벤트(`OnChanged`)를 발행해 UI를 자동 갱신

```csharp
public event Action OnChanged; // 인벤토리 변경 시 구독자(View)에게 자동 통보

public int Add(ItemDefinition item, int amount)
{
    if (item == null || amount <= 0) return 0;
    int remaining = amount;

    // 1단계: 같은 아이템 슬롯에 우선 채워 스택 낭비 방지
    for (int i = 0; i < slots.Count && remaining > 0; i++)
    {
        var slot = slots[i];
        if (slot.IsSameItem(item) && !slot.IsFull)
            remaining -= slot.AddCount(remaining);
    }

    // 2단계: 빈 슬롯에 나머지 배치
    for (int i = 0; i < slots.Count && remaining > 0; i++)
    {
        var slot = slots[i];
        if (slot.IsEmpty)
        {
            int toAdd = Mathf.Min(remaining, item.MaxStack);
            slot.SetItem(item, toAdd);
            remaining -= toAdd;
        }
    }

    int actualAdded = amount - remaining;
    if (actualAdded > 0)
        OnChanged?.Invoke(); // View에 변경 알림 (MVP 이벤트 연결)

    return actualAdded;
}

// IngredientKey 기반 집계: 비둘기 고기 → 닭 슬롯 대체 사용 지원
public int CountByIngredientKey(string ingredientKey)
{
    int total = 0;
    foreach (var slot in slots)
        if (!slot.IsEmpty && slot.Item.IngredientKey == ingredientKey)
            total += slot.Count;
    return total;
}
```

**설계 의도**
- 2단계 배치로 슬롯 낭비 없이 아이템을 채움
- `IngredientKey` 기반 집계로 "비둘기 고기 = 닭 재료"처럼 동일 역할의 서로 다른 아이템을 코드 수정 없이 대체 가능하게 설계

---

## 3. 요리 파이프라인 — 재료 검증 → 소모 → 미니게임 (`CookingManager.cs`)

> **핵심 포인트:** 레시피 해금 상태·재료 보유량 검증 → 재료 소모 → 타이머 기반 요리 진행 → 완료 이벤트로 결과물 지급까지 단방향 파이프라인으로 구성

```csharp
public bool CanCookRecipe(RecipeDefinition recipe)
{
    if (isCooking) return false;
    // 해금 상태 확인 (RecipeUnlockManager에 위임)
    if (RecipeUnlockManager.Instance != null &&
        !RecipeUnlockManager.Instance.IsRecipeUnlocked(recipe)) return false;
    // 재료 보유량 확인
    foreach (var ingredient in recipe.RequiredIngredients)
        if (!PlayerInventory.HasItem(ingredient.Ingredient, ingredient.RequiredAmount))
            return false;
    return true;
}

public void StartCooking(RecipeDefinition recipe)
{
    // 재료 소모 (원자적 처리 — 전부 있을 때만 호출됨)
    foreach (var ingredient in recipe.RequiredIngredients)
        PlayerInventory.RemoveItem(ingredient.Ingredient, ingredient.RequiredAmount);

    isCooking = true;
    cookingTimer.StartTimer(recipe, recipe.CookingTime);
    OnCookingStarted?.Invoke(recipe, recipe.CookingTime); // UI에 요리 시작 알림
}

private void OnTimerCompleted(RecipeDefinition recipe)
{
    PlayerInventory.Add(recipe.ResultDish, recipe.ResultAmount); // 결과물 지급
    isCooking = false;
    OnCookingCompleted?.Invoke(recipe); // 판매 시스템 등 구독자에게 완료 알림
}
```

**설계 의도**
- `CanCookRecipe`와 `StartCooking`을 분리해 UI에서 버튼 활성화 판단과 실제 요리 로직을 독립적으로 호출 가능
- 이벤트(`OnCookingStarted / OnCookingCompleted`)로 요리 결과를 외부에 전달해 CookingManager가 UI·판매 시스템에 직접 의존하지 않음

---

## 4. ScriptableObject 기반 레시피 — 랭크별 가격 계산 (`RecipeDefinition.cs`)

> **핵심 포인트:** `[CreateAssetMenu]`로 에디터에서 레시피 에셋을 직접 생성·수정 가능하고, C# 8 switch expression으로 요리 랭크에 따른 가격 배율을 간결하게 처리

```csharp
[CreateAssetMenu(fileName = "New Recipe", menuName = "Cooking/Recipe Definition")]
public class RecipeDefinition : ScriptableObject
{
    [SerializeField] private string recipeId;      // 저장/복원에 쓰이는 고유 ID
    [SerializeField] private RecipeIngredient[] requiredIngredients;
    [SerializeField] private ItemDefinition resultDish;
    [SerializeField] private MinigameId[] minigameSequence; // 미니게임 진행 순서

    // 랭크별 판매 가격: S(+50%) ~ F(-50%), switch expression으로 간결하게 표현
    public int GetPriceByRank(char rank)
    {
        float multiplier = rank switch
        {
            'S' => 1.5f, 'A' => 1.2f, 'B' => 1.0f,
            'C' => 0.8f, 'F' => 0.5f, _   => 1.0f
        };
        return Mathf.RoundToInt(basePrice * multiplier);
    }
}

[System.Serializable]
public class RecipeIngredient
{
    [SerializeField] private ItemDefinition ingredient;
    [SerializeField] private int requiredAmount;
    public ItemDefinition Ingredient   => ingredient;
    public int            RequiredAmount => requiredAmount;
}
```

**설계 의도**
- ScriptableObject 에셋 단위로 레시피를 관리해 빌드 없이 콘텐츠 추가·수정 가능
- `RecipeId` 문자열로 저장/복원하는 구조 덕분에 에셋 참조가 깨져도 ID 기반 폴백 로딩으로 복원 가능

---

## 5. ID 기반 레시피 해금 복원 — 폴백 로딩 패턴 (`RecipeUnlockManager.cs`)

> **핵심 포인트:** 씬 전환 후 ScriptableObject 레퍼런스가 끊어질 수 있는 상황에 대비해, 저장된 RecipeId 문자열로 에셋을 재탐색하는 폴백 로딩 패턴 적용

```csharp
public void LoadUnlockedRecipes(List<string> recipeIds)
{
    isDataLoaded = true;

    // allRecipes 인스펙터 세팅 누락 시 동적 폴백 (씬 이동에 강건함)
    if (allRecipes == null || allRecipes.Length == 0)
    {
        allRecipes = Resources.FindObjectsOfTypeAll<RecipeDefinition>();
        Debug.LogWarning($"allRecipes 동적 로드: {allRecipes.Length}개");
    }

    unlockedRecipes.Clear();
    foreach (string id in recipeIds)
    {
        RecipeDefinition recipe = FindRecipeById(id);
        if (recipe != null) unlockedRecipes.Add(recipe);
        else Debug.LogWarning($"레시피를 찾을 수 없습니다: {id}");
    }

    OnUnlockedRecipesChanged?.Invoke(); // CookingUI 등 구독자에게 변경 통보
}

private RecipeDefinition FindRecipeById(string id)
{
    // 1차: 인스펙터에 등록된 allRecipes 배열에서 탐색
    if (allRecipes != null)
        foreach (var r in allRecipes)
            if (r != null && r.RecipeId == id) return r;

    // 2차 폴백: 메모리에 로드된 모든 RecipeDefinition에서 탐색
    foreach (var r in Resources.FindObjectsOfTypeAll<RecipeDefinition>())
        if (r != null && r.RecipeId == id) return r;

    return null;
}
```

**설계 의도**
- 저장 데이터(JSON)는 ScriptableObject 레퍼런스 대신 문자열 ID를 저장해 씬 전환·에셋 이동에도 안전하게 복원
- 2단계 폴백으로 인스펙터 설정 실수 시에도 게임 데이터 손실 없이 진행

---

## 6. 판매 트랜잭션 — 롤백 처리 (`SaleService.cs`)

> **핵심 포인트:** 아이템 제거와 돈 지급을 순서대로 처리하고, 돈 지급 실패 시 아이템을 인벤토리에 다시 반환하는 롤백 로직으로 데이터 일관성을 보장

```csharp
public bool ProcessSale(ItemDefinition item, int quantity, int pricePerUnit)
{
    if (!CanSell(item, quantity)) return false;

    // Step 1: 인벤토리에서 아이템 제거
    int removed = playerInventory.RemoveItem(item, quantity);
    if (removed != quantity)
    {
        Debug.LogError($"수량 불일치: 요청 {quantity} / 제거 {removed}");
        return false;
    }

    // Step 2: 돈 지급 — 실패하면 아이템 롤백
    int totalPrice = quantity * pricePerUnit;
    if (!PlayerMoneyManager.Instance.AddMoney(totalPrice))
    {
        playerInventory.Add(item, removed); // 롤백
        Debug.LogError("돈 지급 실패 — 아이템 반환");
        return false;
    }

    Debug.Log($"{item.DisplayName} x{quantity} 판매 완료! 수익: {totalPrice}원");
    return true;
}
```

**설계 의도**
- 인벤토리 차감과 재화 지급을 원자적 트랜잭션처럼 처리
- 중간 실패 시 이미 제거된 아이템을 즉시 되돌려 데이터 불일치 상태를 방지

---

## 7. 씬 전환 후 데이터 복원 — Coroutine 지연 패턴 (`GameManager.cs`)

> **핵심 포인트:** 씬 로드 직후 모든 싱글톤 매니저의 초기화 완료를 보장하기 위해 Coroutine으로 여러 프레임 대기 후 저장 데이터를 순차 복원

```csharp
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // 게임 씬 진입 시에만 저장 데이터 복원 시작
    if (scene.name == gameSceneName && Save != null)
        StartCoroutine(RestoreGameData());
}

private IEnumerator RestoreGameData()
{
    // 여러 프레임 + 짧은 시간 대기: 씬 내 모든 Awake/Start 완료 보장
    yield return null;
    yield return null;
    yield return new WaitForSeconds(0.1f);

    // 레시피 해금 상태 복원
    if (RecipeUnlockManager.Instance != null)
        RecipeUnlockManager.Instance.LoadUnlockedRecipes(Save.unlockedRecipeIds);

    // 인벤토리 상태 복원
    if (Inventory.Instance != null)
        Inventory.Instance.LoadFromSaveData(Save.inventorySlots);
}
```

**설계 의도**
- `SceneManager.sceneLoaded`는 씬 오브젝트 생성 직후 호출되므로 바로 매니저를 참조하면 null이 발생
- 2프레임 + 0.1초 대기로 모든 `Awake` / `Start`가 완료된 타이밍에 복원을 시작해 초기화 순서 문제 해결

---

## 8. MVP 패턴 — 플레이어 애니메이션 분리 (`PlayerAnimation` 트리오)

> **핵심 포인트:** 애니메이션 로직을 Model(상태) · View(Animator 제어) · Presenter(연결)로 분리해 NPC 등 다른 캐릭터도 같은 구조로 재사용 가능하도록 설계

```csharp
// Model: 순수 상태 데이터만 보유, Unity API에 의존하지 않음
public class PlayerAnimationModel
{
    public bool IsWalking { get; private set; }
    public void SetWalking(bool value) => IsWalking = value;
}

// View: Animator와 직접 소통, 외부에서 직접 호출 금지
public class PlayerAnimationView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private const string IsWalkFParam = "isWalkF";

    public void SetWalkingAnimation(bool isWalking)
        => animator?.SetBool(IsWalkFParam, isWalking);
}

// Presenter: Model 업데이트 후 View에 렌더 요청 — 단방향 흐름
public class PlayerAnimationPresenter
{
    private readonly PlayerAnimationModel _model;
    private readonly PlayerAnimationView  _view;

    public PlayerAnimationPresenter(PlayerAnimationModel m, PlayerAnimationView v)
        => (_model, _view) = (m, v);

    public void UpdateWalkingState(bool isWalking)
    {
        _model.SetWalking(isWalking);          // 1. 상태 갱신
        _view.SetWalkingAnimation(_model.IsWalking); // 2. 렌더 반영
    }
}
```

**설계 의도**
- Model이 Unity API(`MonoBehaviour`, `Animator`)에 의존하지 않아 단독 유닛 테스트 가능
- Presenter를 통한 단방향 흐름으로 View가 Model을 직접 참조하는 순환 의존 방지

---

## 9. 이벤트 기반 인벤토리 UI 갱신 (`InventoryView.cs`)

> **핵심 포인트:** `OnEnable` / `OnDisable`에서 이벤트 구독·해제를 쌍으로 관리해 메모리 누수를 방지하고, DOTween 애니메이션으로 UI 등장을 자연스럽게 처리

```csharp
private void OnEnable()
{
    // 팝업 등장 애니메이션 (DOTween)
    transform.localScale = Vector3.one * 0.9f;
    canvasGroup.alpha = 0f;
    transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    canvasGroup.DOFade(1f, 0.3f);

    inventory = Inventory.Instance;
    CreateSlotViews(); // 슬롯 UI 동적 생성

    if (inventory != null)
    {
        inventory.OnChanged += Refresh; // 인벤토리 변경 → UI 자동 갱신 구독
        Refresh();
    }
}

private void OnDisable()
{
    // 구독 해제 — 씬 전환/오브젝트 파괴 시 누수 방지
    if (inventory != null)
        inventory.OnChanged -= Refresh;
}

public void Refresh()
{
    if (slotViews == null) return;
    foreach (var slotView in slotViews)
        slotView?.Refresh();
}
```

**설계 의도**
- `Inventory.OnChanged` 이벤트 하나로 아이템 추가·제거·사용 등 모든 변경이 UI에 즉시 반영
- `OnEnable`/`OnDisable` 쌍 구독으로 오브젝트 활성화 상태에서만 이벤트 수신, 불필요한 Update 폴링 제거

---

## 10. CSV 기반 분기 대화 시스템

> **핵심 포인트:** CSV 한 파일로 대화 흐름 전체를 정의하고, `"텍스트|nextId;텍스트|nextId"` 포맷의 선택지를 런타임에 파싱해 분기 진행. 대화 완료 시 돈·아이템·레시피 보상을 인스펙터 설정만으로 지급

```csharp
// CSVLoader — 따옴표 안의 \n은 행 구분자로 처리하지 않아 다행 대화 내용도 한 셀에 기술 가능
else if (c == '\n' && !inQuotes)
{ rows.Add(current.ToString()); current.Clear(); }

// ChoiceParser — "예|102;아니요|201" 포맷을 (text, nextId) 튜플 리스트로 변환
foreach (string part in raw.Split(';'))
{
    string[] kv = part.Trim().Split('|');
    if (kv.Length == 2 && int.TryParse(kv[1].Trim(), out int id))
        choices.Add((kv[0].Trim(), id));
}

// DialogueManager — ID로 노드 탐색, 선택지 유무에 따라 분기 또는 순차 진행
private void ShowDialogueLine(DialogueLine line)
{
    currentLine = line;
    view.RenderLine(line); // View에 텍스트·초상화 렌더링 위임
    if (line.isChoice)
        view.ShowChoices(ChoiceParser.Parse(line.choicesRaw), OnChoiceSelected);
    else
        view.ClearChoices();
}

// DialogueTarget — 대화 종료 콜백에서 보상 일괄 지급 후 씬 전환까지 연계
public void OnDialogueComplete()
{
    GiveMoneyReward();
    GiveItemRewards();
    UnlockRecipeRewards();
    onDialogueComplete?.Invoke();
    if (loadSceneAfterDialogue) StartCoroutine(LoadSceneAfterDelay());
}
```

**설계 의도**
- CSV 파일만 교체하면 코드 수정 없이 새 대화·보상 이벤트 추가 가능
- `ChoiceParser`를 독립 정적 클래스로 분리해 파싱 로직을 재사용 가능하도록 설계
- `DialogueTarget` 컴포넌트를 NPC에 붙이고 인스펙터에서 보상을 설정하면 대화마다 다른 보상 부여 가능

---

# 기술적 챌린지

## 손님 대기열 위치 계산 및 이동 로직

![Illegal k-food truck simulator 2025-11-10 오후 11_57_46.png](Illegal_k-food_truck_simulator_2025-11-10_%EC%98%A4%ED%9B%84_11_57_46%201.png)

손님이 자신의 대기열 위치를 찾고 이동하는 과정은 `OrderManager`클래스와 `CustomerOrderSystem`클래스의 협업을 통해 이루어지도록 구현함

### 1. 손님 대기열 등록

손님이 `OrderPoint`범위에 진입하면 `CustomerOrderSystem`클래스의 `JoinQueue`메서드를 호출

이 `JoinQueue` 메서드는 `OrderManager`의 `EnqueueCustomer`메서드를 통해 손님을 대기열에 추가

### 2. 대기열 정렬

`OrderManager` 가 손님을 대기열에 추가한 후, `sortByDistance`옵션에 따라 손님들을 트럭과의 거가 가까운 순으로 정렬. 정렬된 손님 목록은 내부적으로 `Queue`로 재구성됨

### 3. 대기열 위치 계산

손님들의 대기 위치는 `OrderManager`의 `CalculateQueuePosition`메서드를 통해 계산됨. 이 메서드는 다음 요소를 기반으로 각 손님의 위치를 결정함

- OrderPoint의 위치
- queueDirection (줄 서는 방향)
- customerSpacing (손님 간 간격)

첫 번째 손님은 OrderPoint 바로 앞에 위치하며, 이후 손님들은 `queueDirection` 방향으로 `customerSpacing`만큼 떨어진 위치에 배치됨

### 4. 손님 이동

계산된 위치는 `CustomerOrderSystem`의 `SetTargetPosition`메서드를 통해 각 손님에게 전달됨, 손님은 `NavMeshAgent`를 사용하여 해당 위치로 이동함

### 5. 도착 확인 및 주문 처리

손님이 목표 위치에 도달하면 `CustomerOrderSystem`의 `CheckArrival` 메서드가 호출되어 도착 여부를 확인

첫 번째 손님은 도착 후 주문을 시작하며, 이후 손님들은 대기열에서 자신의 차례를 기다림

## UI 설계 구조 및 MVP 패턴 연동 방식

![Illegal k-food truck simulator 2025-11-10 오후 11_56_48.png](Illegal_k-food_truck_simulator_2025-11-10_%EC%98%A4%ED%9B%84_11_56_48.png)

![Illegal k-food truck simulator 2025-11-10 오후 11_55_29.png](Illegal_k-food_truck_simulator_2025-11-10_%EC%98%A4%ED%9B%84_11_55_29.png)

![Illegal k-food truck simulator 2025-11-10 오후 11_54_42.png](Illegal_k-food_truck_simulator_2025-11-10_%EC%98%A4%ED%9B%84_11_54_42.png)

UI 설계는 MVP 패턴을 기반으로 구현했으며, 각 컴포넌트는 명확한 역할을 가지고 상호작용함.

UI와 비즈니스 로직을 분리하여 유지 보수성과 확장성을 높이는 데 집중하여 설계함

### 1. Model (데이터 및 비즈니스 로직)

- Model은 게임의 핵심 데이터를 관리하며, UI와 독립적으로 동작
- `Inventory`클래스는 플레이어의 아이템 데이터를 관리함, 슬롯 데이터는 `InventorySlot`을 통해 세부적으로 관리됨
- `DialogueManager`는 대화 데이터를 로드하고, 현재 대화 상태를 관리함

### 2. View (UI 컴포넌트)

- View는 사용자와 상호작용하는 UI 요소를 담당, 데이터를 표시하거나 사용자 입력을 수집함
- `InventoryView`는 `ItemSlotView`를 통해 인벤토리 슬롯을 시각적으로 표시
- `DialogueView`는 대화 내용을 화면에 렌더링하고, 선택지를 동적으로 생성
- `CookingUI`는 요리 미니게임의 진행 상황을 표시하며, 플레이어의 입력을 처리함

### 3. Presenter (로직 및 연결)

- Presenter는 Model과 View 간의 중재자 역할을 하며, 비즈니스 로직을 처리하고 View를 업데이트함
- `InventoryPresenter`는 `Inventory`데이터를 `InventoryView`에 전달하고, 사용자의 아이템 선택 이벤트를 처리함
- `DialoguePresenter`는 `DialogueManager`에서 대화 데이터를 가져와 `DialogueView`에 전달하며, 선택지 선택 이벤트를 처리함

### 4. MVP 패턴의 연동

- Model에서 데이터를 가져와 Presenter를 통해 View에 전달하게 구현함
    - ex) `InventoryPresenter`가 `Inventory`데이터를 가져와 `InventoryView`에 슬롯 정보 전달
- View에서 발생한 사용자 입력 이벤트는 Presenter를 통해 처리됨
    - ex) `DialogueView`에서 선택지가 선택 시 `DialoguePresenter`가 이를 처리 후 다음 대화 로드
- Presenter는 키 입력 등을 통해 View의 활성화 및 비활성화를 제어하며, UI 간의 전환을 관리
    - ex) `UIManager`가 `DialogueView`와 `InventoryView`등의 전환 제어

---

# **트러블슈팅**

## **1. UI–로직 결합 문제를 MVP 패턴으로  분리**

개발 초기, 인벤토리와 대화 UI가 로직과 직접 연결되어 있어 기능을 추가하거나 수정할 때마다 UI까지 함께 수정해야 하는 구조적 문제를 발견, 이를 해결하기 위해 두 시스템을 MVP 패턴으로 전면 리팩터링함.

모델은 순수 데이터만 관리하고, 프레젠터가 변화 이벤트를 받아 뷰를 갱신하는 구조로 분리

그 결과 인벤토리는 모델에서 아이템 변경 이벤트를 발행하면 프레젠터가 이를 받아 UI를 자동으로 갱신하는 구조가 되었고, 대화 시스템은 CSV 파일만 교체해도 신규 대화 이벤트를 추가할 수 있어 확장성이 향상됨.

또한 플레이어 애니메이션을 상태·렌더링·제어 구조로 나누면서 NPC도 같은 로직을 재사용할 수 있는 구조가 되었음 

---

## **2. 반복 빌드 문제를 ScriptableObject 데이터 구조로 개선**

레시피와 아이템 데이터가 코드에 하드코딩되어 있어 콘텐츠 데이터를 수정할 때마다 빌드를 반복해야 하는 비효율이 발생함.

이를 해결하기 위해 레시피와 아이템을 모두 ScriptableObject로 분리하고 ID 기반 참조 구조로 재설계

그 결과 ScriptableObject 자산만 생성하거나 수정해도 코드 변경 없이 즉시 게임에 반영할 수 있게 되었으며, 콘텐츠 제작 속도가 크게 향상되었음

---

# **배운 점**

- 이벤트 기반 구조로 **UI-로직 결합도 최소화**
- ScriptableObject 설계의 **데이터 내구성** 이해 (ID 매핑, 폴백 로딩)
- 씬 초기화/복원 타이밍 문제 해결을 통해 **Unity 비동기 구조 감각 습득**
- 대기열·판매 루프 등 **비결정적 이벤트의 동기화** 경험
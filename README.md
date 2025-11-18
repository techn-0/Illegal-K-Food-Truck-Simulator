# 🐔불법 K-푸드트럭 시뮬레이터

## Illegal K-Food Truck Simulator

> “닭 대신 비둘기를 파는 불법 푸드트럭 게임”
> 
> 
> Unity 6 기반, 1개월 MVP 개인 프로젝트
> 
> **아키텍처·데이터 설계·UI 이벤트 시스템**을 직접 구축
> 

---

# 📋 개요

## 프로젝트 개요

| 항목 | 내용 |
| --- | --- |
| **개발 기간** | 약 1개월 |
| **엔진 / 렌더링** | Unity 6 (URP) |
| **장르** | 라이트 경영 + 생활 시뮬레이션(+ 미니 스텔스 추가 예정) |
| **시점 / 톤** | 쿼터뷰 / 로우폴리 병맛 유머 |
| **담당 역할** | 기획 · 프로그래밍 · 디자인 전담 |
| **영감** | 초등학교 시절 도시괴담 “학교 앞 치킨은 비둘기로 만든다” |

노션에서 보기:[ @Illegal K-Food Truck Simulator ](https://www.notion.so/Illegal-K-Food-Truck-Simulator-2a9311655ecb8095adcae1579e6d0474)

---

## 스토리 개요

플레이어는 어느 날 꿈속에서 **정체불명의 할머니**에게  **치킨 레시피**를 전수받는다.

비법의 핵심은 닭 대신 비둘기를 튀기는 것

도시에서 비둘기를 잡아 치킨으로 속여 팔며 돈을 벌수록,

도시는 점점 **비둘기**로 가득해 지는데…

99일을 버티면 할머니와 마을의 진실이 드러난다….

## 플레이 영상 / 스크린샷

### 유튜브 - https://www.youtube.com/watch?v=DVLwJ0lvbDw

### 스크린샷

<img width="1920" height="1080" alt="Illegal k-food truck simulator 2025-11-10 오후 11_54_27" src="https://github.com/user-attachments/assets/fc08e58e-7d91-4607-961a-c4cb6bc376d1" />
<img width="1920" height="1080" alt="Illegal k-food truck simulator 2025-11-10 오후 11_57_46" src="https://github.com/user-attachments/assets/1519d588-2274-4333-b2f7-6a5f9cea4166" />
<img width="1920" height="1080" alt="Illegal k-food truck simulator 2025-11-10 오후 11_55_29" src="https://github.com/user-attachments/assets/840cd08e-4f4f-492c-bb7d-5d6e77dfc455" />


---

# ⚙️ 시스템

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
| **인벤토리** | `Inventory.cs`, `InventorySlot.cs` | 슬롯 기반 아이템 관리, JSON 저장/복원 |
| **요리 미니게임** | `CookingManager`, `MinigameBase`, `MiniGameManager` | 조리 시퀀스 실행, 결과 판정, UI 오버레이 |
| **레시피 해금** | `RecipeUnlockManager` | ScriptableObject ID 매핑 기반 해금/복원 |
| **판매 / 경제** | `SaleService`, `PlayerMoneyManager` | 판매 가능 판정, 수익 반영, 롤백 처리 |
| **UI 이벤트** | `MoneyDisplayUI`, `EndDayUI` | Passive View + 이벤트 구독 구조 |
| **저장 / 불러오기** | `SaveManager`, `GameManager` | JSON 직렬화, 씬 재로딩 복원 |

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

## **게임 시스템 플로우**

<img width="1724" height="1524" alt="Untitled (4) (1)" src="https://github.com/user-attachments/assets/424ccb3e-e5c5-4837-80ec-05937c46ca5c" />


## **시스템 간 연동 구조**

| 이벤트 흐름 | 데이터 흐름 |
| --- | --- |
| `RecipeUnlockManager` → `CookingUI` | 해금된 레시피 목록 표시 |
| `CookingUI` → `CookingManager` | 플레이어가 요리 선택 |
| `CookingManager` → `MiniGameManager` | 미니게임 실행 및 판정 |
| `MiniGameResult` → `SaleService` | 완성품 판매 가능 여부 확인 |
| `SaleService` → `PlayerMoneyManager` → `MoneyDisplayUI` | 수익 반영 및 UI 업데이트 |
| `GameManager` ↔ `SaveManager` | 저장 / 불러오기 처리 |

---

# 🚩 기술적 챌린지

## 손님 대기열 위치 계산 및 이동 로직

손님이 자신의 대기열 위치를 찾고 이동하는 과정은 `OrderManager`클래스와 `CustomerOrderSystem`클래스의 협업을 통해 이루어지도록 구현했습니다.

<img width="1920" height="1080" alt="Illegal k-food truck simulator 2025-11-10 오후 11_57_46 (1)" src="https://github.com/user-attachments/assets/d48f9b8e-0e13-49f2-8212-32132241efc1" />


### 1. 손님 대기열 등록

손님이 `OrderPoint`범위에 진입하면 `CustomerOrderSystem`클래스의 `JoinQueue`메서드를 호출합니다. 이 `JoinQueue` 메서드는 `OrderManager`의 `EnqueueCustomer`메서드를 통해 손님을 대기열에 추가합니다.

### 2. 대기열 정렬

`OrderManager` 가 손님을 대기열에 추가한 후, `sortByDistance`옵션에 따라 손님들을 트럭과의 거가 가까운 순으로 정렬합니다. 정렬된 손님 목록은 내부적으로 `Queue`로 재구성됩니다.

### 3. 대기열 위치 계산

손님들의 대기 위치는 `OrderManager`의 `CalculateQueuePosition`메서드를 통해 계산됩니다. 이 메서드는 다음 요소를 기반으로 각 손님의 위치를 결정합니다.

- OrderPoint의 위치
- queueDirection (줄 서는 방향)
- customerSpacing (손님 간 간격)

첫 번째 손님은 OrderPoint 바로 앞에 위치하며, 이후 손님들은 `queueDirection` 방향으로 `customerSpacing`만큼 떨어진 위치에 배치됩니다.

### 4. 손님 이동

계산된 위치는 `CustomerOrderSystem`의 `SetTargetPosition`메서드를 통해 각 손님에게 전달됩니다. 손님은 `NavMeshAgent`를 사용하여 해당 위치로 이동합니다.

### 5. 도착 확인 및 주문 처리

손님이 목표 위치에 도달하면 CustomerOrderSystem의 CheckArrival 메서드가 호출되어 도착 여부를 확인합니다. 첫 번째 손님은 도착 후 주문을 시작하며, 이후 손님들은 대기열에서 자신의 차례를 기다립니다.

## UI 설계 구조 및 MVP 패턴 연동 방식

UI 설계는 MVP 패턴을 기반으로 구현했으며, 각 컴포넌트는 명확한 역할을 가지고 상호작용합니다. 이 구조는 UI와 비즈니스 로직을 분리하여 유지 보수성과 확장성을 높이는 데 집중하였습니다.
<img width="1920" height="1080" alt="Illegal k-food truck simulator 2025-11-10 오후 11_56_48" src="https://github.com/user-attachments/assets/e090e20a-5404-44d1-94ec-cdf95f3fd6a7" />
<img width="1920" height="1080" alt="Illegal k-food truck simulator 2025-11-10 오후 11_55_29 (1)" src="https://github.com/user-attachments/assets/b4af910e-d591-4ff8-bdbe-8c04e176f9c9" />
<img width="1920" height="1080" alt="Illegal k-food truck simulator 2025-11-10 오후 11_54_42" src="https://github.com/user-attachments/assets/868c5bb3-046a-4235-88b5-b86aecc00f58" />


### 1. Model (데이터 및 비즈니스 로직)

- Model은 게임의 핵심 데이터를 관리하며, UI와 독립적으로 동작합니다.
- `Inventory`클래스는 플레이어의 아이템 데이터를 관리하며, 슬롯 데이터는 `InventorySlot`을 통해 세부적으로 관리됩니다.
- `DialogueManager`는 대화 데이터를 로드하고, 현재 대화 상태를 관리합니다.

### 2. View (UI 컴포넌트)

- View는 사용자와 상호작용하는 UI 요소를 담당하며, 데이터를 표시하거나 사용자 입력을 수집합니다.
- `InventoryView`는 `ItemSlotView`를 통해 인벤토리 슬롯을 시각적으로 표시합니다.
- `DialogueView`는 대화 내용을 화면에 렌더링하고, 선택지를 동적으로 생성합니다.
- `CookingUI`는 요리 미니게임의 진행 상황을 표시하며, 플레이어의 입력을 처리합니다.

### 3. Presenter (로직 및 연결)

- Presenter는 Model과 View 간의 중재자 역할을 하며, 비즈니스 로직을 처리하고 View를 업데이트합니다.
- `InventoryPresenter`는 `Inventory`데이터를 `InventoryView`에 전달하고, 사용자의 아이템 선택 이벤트를 처리합니다.
- `DialoguePresenter`는 `DialogueManager`에서 대화 데이터를 가져와 `DialogueView`에 전달하며, 선택지 선택 이벤트를 처리합니다.

### 4. MVP 패턴의 연동

- Model에서 데이터를 가져와 Presenter를 통해 View에 전달하게 구현했습니다.
    - ex) `InventoryPresenter`가 `Inventory`데이터를 가져와 `InventoryView`에 슬롯 정보 전달
- View에서 발생한 사용자 입력 이벤트는 Presenter를 통해 처리됩니다.
    - ex) `DialogueView`에서 선택지가 선택 시 `DialoguePresenter`가 이를 처리 후 다음 대화 로드
- Presenter는 키 입력 등을 통해 View의 활성화 및 비활성화를 제어하며, UI 간의 전환을 관리합니다.
    - ex) `UIManager`가 `DialogueView`와 `InventoryView`등의 전환 제어

# **💡트러블슈팅**

## **1. UI–로직 결합 문제를 MVP 패턴으로  분리**

개발 초기에는 인벤토리와 대화 UI가 로직과 직접 연결되어 있어 기능을 추가하거나 수정할 때마다 UI까지 함께 수정해야 하는 구조적 문제가 있었습니다. 이를 해결하기 위해 두 시스템을 MVP 패턴으로 전면 리팩터링 했습니다. 모델은 순수 데이터만 관리하고, 프레젠터가 변화 이벤트를 받아 뷰를 갱신하는 구조로 분리했습니다. 

그 결과 인벤토리는 모델에서 아이템 변경 이벤트를 발행하면 프레젠터가 이를 받아 UI를 자동으로 갱신하는 구조가 되었고, 대화 시스템은 CSV 파일만 교체해도 신규 대화 이벤트를 추가할 수 있어 확장성이 향상되었습니다. 또한 플레이어 애니메이션을 상태·렌더링·제어 구조로 나누면서 NPC도 같은 로직을 재사용할 수 있게 구조가 정돈되었습니다.

---

## **2. 반복 빌드 문제를 ScriptableObject 데이터 구조로 개선**

레시피와 아이템 데이터가 코드에 하드코딩되어 있어 콘텐츠 데이터를 수정할 때마다 빌드를 반복해야 하는 비효율이 있었습니다. 이를 해결하기 위해 레시피와 아이템을 모두 ScriptableObject로 분리하고 ID 기반 참조 구조로 재설계했습니다. 이 방식으로 전환한 뒤에는 ScriptableObject 자산만 생성하거나 수정해도 코드 변경 없이 즉시 게임에 반영할 수 있게 되었으며, 결과적으로 콘텐츠 제작 속도가 크게 향상되고 유지 보수성이 높아졌습니다.


---

# **✍️ 배운 점**

- 이벤트 기반 구조로 **UI-로직 결합도 최소화**
- ScriptableObject 설계의 **데이터 내구성** 이해 (ID 매핑, 폴백 로딩)
- 씬 초기화/복원 타이밍 문제 해결을 통해 **Unity 비동기 구조 감각 습득**
- 대기열·판매 루프 등 **비결정적 이벤트의 동기화** 경험
---

## **🎯 개선 계획**

| 목표 | 내용 |
| --- | --- |
| 미니게임 파이프라인 확장 | 레시피 난이도·랜덤 요소 추가 |
| 자동화 테스트 | `PlayMode Test` 기반 매니저 검증 |
| 의존성 주입 도입 | 싱글톤 → 인터페이스 기반 DI 리팩터링 |
| 데이터 검증 툴 | 레시피 중복 ID, 잘못된 레퍼런스 자동 탐지 |
| 손님 AI 확장 | 인내심·선호도 변수 추가, 시간대별 손님 밀도 변동 |
| 스텔스 콘텐츠 확장 | 위생 단속 이벤트, 손님의 의심도 시스템 추가 |

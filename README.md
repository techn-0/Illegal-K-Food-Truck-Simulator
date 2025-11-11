# 🐔 Illegal K-Food Truck Simulator

> **“닭 대신 비둘기?”**  
> 도시의 어둠 속에서 불법 푸드트럭을 몰며, 비둘기 고기로 만든 치킨을 팔아 돈을 버는 병맛 시뮬레이션 게임.  
> 꿈속에서 만난 할머니에게 배운 레시피의 진실은 생각보다 끔찍하다.

---

## 🎯 프로젝트 목표

- **1개월 MVP 완성**  
  이동 → 요리 → 판매 → 대화 루프를 구현하고, Steam 데모 수준의 플레이어블 빌드 확보.  
- **한국적 병맛 + 도시괴담 융합**  
  초등학생 시절 들었던 괴담 *“학교 앞 홍보용 치킨은 비둘기 고기로 만든다”* 에서 착안.  
- **확장 가능한 시스템 구조 구축**  
  인벤토리, 레시피, 상점, 대화, 손님 시스템을 모듈화하여 차후 업데이트에 활용.

---

## 🧩 게임 개요

| 항목 | 내용 |
|------|------|
| **장르** | 라이트 경영 + 생활 시뮬레이션 + 미니 스텔스 |
| **시점** | 3인칭  |
| **플랫폼** | PC (Windows, Steam) |
| **톤앤매너** | 저폴리·셀셰이딩, 병맛 패러디|
| **엔진** | Unity 6 (URP) |
| **개발자** | 정휘건 (Solo Developer) |

---

## 🧙 스토리 컨셉

플레이어는 어느 날 **꿈속에서 낯선 할머니**로부터 치킨 레시피를 전수받는다.  
그 비법의 핵심은 — “닭 대신 비둘기를 써도 된다”는 것.

도시에서 비둘기를 잡아 치킨으로 속여 팔며 돈을 벌지만,  
사람들이 그 치킨을 먹을수록 도시는 점점 **비둘기로 뒤덮인다.**

사실 꿈의 할머니는 **비둘기의 악마**였고,  
비둘기 치킨을 먹은 사람들은 하나둘 **비둘기로 변해간다.**

> 엔딩: 장사 99일(또는 999일)을 버티면 진실이 드러난다.

---

## 🔁 핵심 게임 루프

```mermaid
graph LR
  A[재료 확보 (마트 / 포획)] --> B[요리 & 판매]
  B --> C[손님 반응 및 수익]
  C --> D[업그레이드 / 트럭 관리]
  D --> E[단속 · 이벤트 발생]
  E --> A
```

---

## ⚙️ 주요 시스템

### ▶ 플레이어
- 이동: `WASD` / 게임패드 좌스틱  
- 상호작용: `E` (대화, 상점, 요리 등)  
- 인벤토리: `I`  
- 요리 UI: `C`  
- 대화 진행: `Space` 또는 마우스 클릭  

### ▶ 인벤토리 / 아이템
- `Inventory`, `InventorySlot`, `InventoryView`  
- 12칸 고정, 스택 합치기·분리 가능  
- 변경 이벤트(`OnChanged`)로 UI 자동 갱신  

### ▶ 요리 / 레시피
- `RecipeDefinition (SO)`로 구성  
- 재료 조합 → 조리 시간 → 결과물 → 판매  
- 비둘기 고기를 닭 슬롯에 대체 사용 가능  

### ▶ 손님 / 판매
- `CustomerOrderSystem`, `OrderManager`, `SaleService`  
- 손님이 줄 서서 주문, 제한 시간 내 요리 제공  
- 비둘기 요리 시 반응 랜덤화 (불평 / 무난 / “싸다!”)

### ▶ 상점 / 돈
- `ItemShop` 및 `ItemShopUI`  
- 구매 시 돈 차감, 재고 갱신  
- `PlayerMoneyManager`로 자금 통합 관리  

### ▶ 대화
- `DialogueManager`, `CSVLoader`, `ChoiceParser`  
- TextAsset CSV 기반 분기 대화  
- “할머니”, “손님”, “단속반” 등 NPC 대화 구현 가능  

---

## 🏙️ 게임 시스템 요소

| 구분 | 설명 |
|------|------|
| **합법 루트** | 마트/시장 재료 구매 (안전하지만 비쌈) |
| **불법 루트** | 비둘기 포획, 농장 서리 (위생 하락, 단속 위험) |
| **위생 점수** | 낮을수록 단속 확률 및 식중독 이벤트 증가 |
| **단속 이벤트** | 경찰 시야 콘 기반 미니 스텔스 |
| **트럭 관리** | 연료, 저장공간, 외형(네온, 스티커) 업그레이드 |

---

## 🧠 개발 일정 (1개월 MVP)

| 주차 | 목표 | 세부 내용 |
|------|------|-----------|
| 1주차 | 이동·카메라·맵 | 쿼터뷰 컨트롤러, 도심 맵, 비둘기 스폰 |
| 2주차 | 재료 시스템 | 인벤토리, 위생 변수, 포획 로직 |
| 3주차 | 요리·판매 루프 | 조리 UI, 손님 AI, 비둘기 대체 로직 |
| 4주차 | 이벤트 & Polish | 단속, 축제, FX, 버그픽스 |

---

## 📂 폴더 구조 (요약)

```
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


---

## 🧰 기술 스택

- **Engine:** Unity 6 (URP)  
- **Language:** C#  
- **3D Tools:** Blender (Low-Poly Kitbash)  
- **Audio:** Audacity, BFXR  
- **Version Control:** Git + GitHub  
- **UI:** TextMeshPro, Input System (New)  
- **AI:** NavMeshAgent 기반 손님 이동  

---

## 🎮 향후 확장 계획

- 트럭 커스터마이징 (네온 간판, 데칼, 장식)  
- 손님 친밀도 및 시장 할인 이벤트  
- 식중독 / 위생 디버프 시스템  
- 엔딩 분기 (선한 루트 vs 악마 루트)  

---


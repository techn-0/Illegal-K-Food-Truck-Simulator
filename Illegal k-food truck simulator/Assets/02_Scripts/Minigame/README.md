# 미니게임 시스템 설정 가이드

## 📋 개요
유니티 6 기반 모듈형 3D 미니게임 시스템입니다.
총 4종의 미니게임(비둘기 잡기, 밀가루 담기, 반죽 섞기, 튀기기)을 제공합니다.

---

## 🎯 에디터 설정 단계

### 1단계: ScriptableObject 파라미터 생성

각 미니게임마다 파라미터 설정 파일을 생성해야 합니다.

#### 1-1. 비둘기 잡기 파라미터
1. 프로젝트 창에서 `Assets/02_Scripts/Minigame/Parameters` 폴더에 우클릭
2. `Create > Minigame > Parameters > Catch Pigeon` 선택
3. 파일명: `CatchPigeonParameters`
4. Inspector에서 설정:
   - **Bar Speed**: 1.5 (바 이동 속도)
   - **Bar Range**: 5 (바 이동 범위)
   - **Perfect Zone**: 0.2 (완벽 판정 범위)
   - **Score Gamma**: 2.0 (점수 계산 감마값)
   - **Scoring Curve**: 기본값 유지

#### 1-2. 밀가루 담기 파라미터
1. `Create > Minigame > Parameters > Pour Flour`
2. 파일명: `PourFlourParameters`
3. Inspector 설정:
   - **Target Amount**: 100 (목표량)
   - **Tolerance Range**: 10 (허용 범위)
   - **Flow Rate**: 20 (초당 유량)
   - **Overshoot Penalty**: 2.0 (오버슈트 감점)

#### 1-3. 반죽 섞기 파라미터
1. `Create > Minigame > Parameters > Mix Dough`
2. 파일명: `MixDoughParameters`
3. Inspector 설정:
   - **Target Rotations**: 10 (목표 회전 수)
   - **Time Limit**: 5 (제한 시간)
   - **Min Rotation Angle**: 300 (한 바퀴 인정 각도)
   - **Late Penalty Per Second**: 5 (지연 감점)

#### 1-4. 튀기기 파라미터
1. `Create > Minigame > Parameters > Deep Fry`
2. 파일명: `DeepFryParameters`
3. Inspector 설정:
   - **Target Time**: 10 (목표 시간)
   - **Sigma**: 0.5 (가우시안 표준편차)
   - **Perfect Range**: 0.1 (완벽 판정 범위)
   - **Max Wait Time**: 15 (최대 대기 시간)

---

### 2단계: 미니게임 프리팹 생성

각 미니게임마다 3D 프리팹을 만들어야 합니다.

#### 기본 구조 (모든 미니게임 공통)
```
MinigamePrefab (Root GameObject)
├─ Camera (전용 카메라)
│  └─ Culling Mask: "Minigame" 레이어만
├─ Directional Light (조명)
├─ WorldCanvas (World Space Canvas)
│  ├─ CountdownText (TMP)
│  ├─ InstructionText (TMP)
│  ├─ ProgressUI (게임별로 다름)
│  └─ ResultPanel
│     ├─ ResultScoreText (TMP)
│     └─ ResultRankText (TMP)
└─ GameObjects (게임별 3D 오브젝트)
```

#### 2-1. 비둘기 잡기 프리팹 만들기

1. **빈 GameObject 생성**
   - Hierarchy > 우클릭 > Create Empty
   - 이름: `CatchPigeonMinigame`
   - Layer: `Minigame` (없으면 생성)

2. **카메라 추가**
   - `CatchPigeonMinigame` 우클릭 > Camera
   - Position: (0, 5, -10), Rotation: (20, 0, 0)
   - Culling Mask: "Minigame" 레이어만 선택
   - Clear Flags: Solid Color, Background: 검정색

3. **조명 추가**
   - 우클릭 > Light > Directional Light
   - Rotation: (50, -30, 0)

4. **3D 오브젝트 추가**
   - **도마** (Cutting Board):
     - Cube 생성, Scale: (8, 0.2, 6)
     - Position: (0, 0, 0)
     - Material: 나무 재질
   
   - **비둘기** (Pigeon):
     - Sphere 또는 3D 모델 배치
     - Position: (0, 1, 0) - 도마 위
     - Tag: "Target"
   
   - **바** (Bar):
     - Cube 생성, Scale: (0.5, 3, 0.2)
     - Position: (0, 1.5, 0)
     - Material: 밝은 색상 (시각적으로 잘 보이게)

5. **World Space Canvas 생성**
   - 우클릭 > UI > Canvas
   - 이름: `WorldCanvas`
   - Canvas Component:
     - Render Mode: `World Space`
     - Position: (0, 6, 0)
     - Scale: (0.01, 0.01, 0.01)
     - Width: 1000, Height: 600

6. **UI 요소 추가**
   - Canvas 우클릭 > UI > Text - TextMeshPro
   
   - **CountdownText**:
     - Position: (0, 200, 0)
     - Font Size: 120
     - Alignment: Center
   
   - **InstructionText**:
     - Position: (0, -100, 0)
     - Font Size: 40
     - Alignment: Center
   
   - **ResultPanel** (Panel):
     - Anchor: Stretch All
     - Background: 반투명 검정 (Alpha: 200)
     - 자식으로 ResultScoreText, ResultRankText 추가

7. **스크립트 연결**
   - `CatchPigeonMinigame` 오브젝트 선택
   - Add Component > `CatchPigeonMinigame` 스크립트
   - Inspector에서 참조 연결:
     - **Parameters**: 1단계에서 만든 ScriptableObject
     - **Bar Transform**: 바 오브젝트
     - **Pigeon Transform**: 비둘기 오브젝트
     - **Countdown Text**: CountdownText
     - **Instruction Text**: InstructionText
     - **Result Panel**: ResultPanel
     - **Result Score Text**: ResultScoreText
     - **Result Rank Text**: ResultRankText

8. **프리팹 저장**
   - `CatchPigeonMinigame` 오브젝트를 `Assets/04_Prefabs/Minigames/` 폴더로 드래그
   - Hierarchy에서 삭제

#### 2-2. 밀가루 담기 프리팹

1. **기본 구조** (위와 동일하게 시작)
   - 이름: `PourFlourMinigame`

2. **3D 오브젝트**
   - **밀가루 포대** (Flour Bag):
     - Cube 또는 3D 모델
     - Position: (0, 3, 0)
     - 기울어질 수 있도록 Pivot 설정
   
   - **비커** (Beaker):
     - Cylinder, Scale: (1, 2, 1)
     - Position: (0, 0, 0)
   
   - **채우기 바** (Fill Bar):
     - Canvas에 UI > Image 추가
     - Image Type: Filled (Vertical)
     - Fill Amount: 0 (스크립트에서 제어)
   
   - **목표 구역** (Target Zone):
     - Image, 색상: 반투명 초록

3. **파티클 시스템**
   - GameObject > Effects > Particle System
   - 이름: `FlourParticles`
   - Shape: Cone, Angle: 20
   - Emission: Rate over Time = 50
   - Start Color: 흰색/베이지
   - Play On Awake: OFF (스크립트에서 제어)

4. **스크립트 연결**
   - `PourFlourMinigame` 스크립트 추가
   - 모든 참조 연결 (Parameters, UI, 3D 오브젝트, ParticleSystem)

5. **프리팹 저장**

#### 2-3. 반죽 섞기 프리팹

1. **기본 구조**
   - 이름: `MixDoughMinigame`

2. **3D 오브젝트**
   - **스테인리스 볼** (Mixing Bowl):
     - Cylinder 또는 3D 모델
     - Position: (0, 0, 0)
   
   - **반죽** (Dough):
     - Sphere, 볼 안쪽에 배치
     - Material: 반죽 느낌 (황갈색)
   
   - **젓는 도구** (Stirrer):
     - 선택사항, 시각적 효과용

3. **UI 추가**
   - **Progress Bar**: 회전 진행도
   - **Rotation Text**: "회전: 3 / 10"
   - **Timer Text**: "시간: 3.5초"

4. **스크립트 연결**
   - `MixDoughMinigame` 스크립트 추가
   - 참조 연결

5. **프리팹 저장**

#### 2-4. 튀기기 프리팹

1. **기본 구조**
   - 이름: `DeepFryMinigame`

2. **3D 오브젝트**
   - **기름** (Oil):
     - Cube, Scale: (5, 3, 5)
     - Material: 반투명 노란색
     - Position: (0, 0, 0)
   
   - **튀김 바구니** (Basket):
     - Cube 또는 3D 모델
     - Position: (0, 4, 0) - 기름 위
     - 스크립트에서 위아래 이동

3. **파티클 시스템**
   - 이름: `BubbleParticles`
   - Shape: Box (기름 영역)
   - Start Size: 0.1~0.3
   - Start Color: 흰색 반투명
   - Gravity: 위로 (Velocity over Lifetime)

4. **UI**
   - **Timer Text**: 큰 폰트 크기로 시간 표시

5. **스크립트 연결**
   - `DeepFryMinigame` 스크립트 추가
   - 참조 연결

6. **프리팹 저장**

---

### 3단계: MiniGameManager 설정

1. **새 씬 또는 메인 씬에서**
   - Hierarchy > Create Empty
   - 이름: `MiniGameManager`

2. **스크립트 추가**
   - Add Component > `MiniGameManager`

3. **프리팹 매핑**
   - Inspector > Minigame Prefabs
   - Size: 4 설정
   - Element 0:
     - Id: `CatchPigeon`
     - Prefab: `CatchPigeonMinigame` 프리팹
   - Element 1:
     - Id: `PourFlour`
     - Prefab: `PourFlourMinigame` 프리팹
   - Element 2:
     - Id: `MixDough`
     - Prefab: `MixDoughMinigame` 프리팹
   - Element 3:
     - Id: `DeepFry`
     - Prefab: `DeepFryMinigame` 프리팹

4. **Dimmer 설정**
   - Dimmer Canvas와 Dimmer Canvas Group은 자동 생성됨
   - 수동으로 만들려면:
     - Hierarchy > UI > Canvas
     - Render Mode: Screen Space - Overlay
     - Sorting Order: 9999
     - Canvas Group 컴포넌트 추가

---

### 4단계: 레이어 설정

1. **Layers 추가**
   - Edit > Project Settings > Tags and Layers
   - Layers에 `Minigame` 추가 (빈 슬롯에)

2. **미니게임 프리팹 레이어 적용**
   - 각 미니게임 프리팹의 모든 자식 오브젝트를 `Minigame` 레이어로 설정
   - Canvas는 제외 (UI는 기본 레이어 유지)

---

### 5단계: 테스트 설정

1. **테스트 트리거 생성**
   - 메인 씬에 빈 GameObject 생성
   - 이름: `MinigameTestTrigger`
   - Add Component > `MinigameTestTrigger` 스크립트

2. **설정**
   - Test Minigame Id: 테스트할 미니게임 선택
   - Trigger Key: `T` (또는 원하는 키)

3. **플레이 모드에서 테스트**
   - Play 버튼 클릭
   - `T` 키 눌러서 미니게임 실행
   - 각 미니게임 테스트 후 결과 확인

---

## 🎮 사용 방법 (코드)

### 기본 사용법
```csharp
using Minigame;

// 미니게임 시작
MiniGameManager.Instance.StartMinigame(MinigameId.CatchPigeon, OnMinigameFinished);

// 콜백 함수
void OnMinigameFinished(MiniGameResult result)
{
    Debug.Log($"점수: {result.score}, 등급: {result.rank}");
    
    // 요리 시스템과 연동
    if (result.rank >= 'B')
    {
        // 성공 처리
    }
}

// 중단 (옵션)
MiniGameManager.Instance.AbortCurrentMinigame();
```

### 요리 시스템 연동 예시 (추후 구현)
```csharp
public class CookingSystem : MonoBehaviour
{
    public void StartCooking(Recipe recipe)
    {
        // 레시피 단계별로 미니게임 실행
        MinigameId gameId = GetMinigameForStep(recipe.currentStep);
        MiniGameManager.Instance.StartMinigame(gameId, OnCookingStepComplete);
    }
    
    private void OnCookingStepComplete(MiniGameResult result)
    {
        // 점수에 따라 요리 품질 결정
        float quality = result.score / 100f;
        currentDish.quality = quality;
        
        // 다음 단계로
        NextCookingStep();
    }
}
```

---

## 🔧 커스터마이징

### 파라미터 조정
- ScriptableObject 파일을 수정하면 즉시 반영됩니다.
- AnimationCurve를 조정하여 난이도 세밀하게 조정 가능

### 새 미니게임 추가
1. `MinigameId` enum에 새 ID 추가
2. 새 Parameters 클래스 생성 (MinigameParametersBase 상속)
3. 새 Minigame 클래스 생성 (MinigameBase 상속)
4. 프리팹 제작 및 MiniGameManager에 등록

---

## 📌 주의사항

1. **TextMeshPro 설치 필요**
   - Window > TextMeshPro > Import TMP Essential Resources

2. **Input System**
   - 프로젝트에 New Input System 사용 시 PlayerInput 컴포넌트 필요
   - Old Input System 사용 중이면 MiniGameManager의 Input 관련 코드 주석 처리

3. **Camera 충돌 방지**
   - 미니게임 카메라는 Minigame 레이어만 렌더링
   - 메인 카메라는 Minigame 레이어 제외

4. **성능 최적화**
   - 미니게임 프리팹은 가볍게 유지
   - 파티클은 적절한 Max Particles 설정

---

## 🐛 트러블슈팅

### 미니게임이 실행되지 않음
- MiniGameManager가 씬에 있는지 확인
- 프리팹 매핑이 올바른지 확인
- Console에서 에러 메시지 확인

### UI가 보이지 않음
- Canvas의 Render Mode 확인
- Camera의 Culling Mask 확인
- EventSystem이 씬에 있는지 확인

### 입력이 안 됨
- Time.timeScale이 0이 아닌지 확인
- Dimmer가 입력을 막고 있지 않은지 확인

---

## 📝 체크리스트

### 필수 작업
- [ ] ScriptableObject 파라미터 4개 생성
- [ ] 미니게임 프리팹 4개 제작
- [ ] MiniGameManager 설정 완료
- [ ] Minigame 레이어 생성 및 적용
- [ ] TextMeshPro 설치
- [ ] 각 미니게임 테스트 완료

### 선택 작업
- [ ] 3D 모델 교체 (현재는 Primitive 사용)
- [ ] 사운드 효과 추가
- [ ] 파티클 효과 개선
- [ ] UI 디자인 커스터마이징
- [ ] 애니메이션 추가

---

이제 요리 시스템과 연동할 준비가 되었습니다!


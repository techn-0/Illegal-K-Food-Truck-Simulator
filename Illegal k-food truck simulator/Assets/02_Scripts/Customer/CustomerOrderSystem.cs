using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 손님의 주문 시스템을 관리하는 클래스
/// 단일 책임: 손님의 이동 및 주문 상태 관리
/// </summary>
public class CustomerOrderSystem : MonoBehaviour
{
    [Header("Order Settings")]
    [Range(1, 5)] public int orderQuantity = 1; // 주문 수량 (인스펙터에서 설정)
    
    [Header("Timer Settings")]
    [Range(10f, 120f)] public float queueWaitTimeLimit = 30f; // 대기열에서 기다리는 시간 (인스펙터에서 설정)
    [Range(10f, 120f)] public float foodWaitTimeLimit = 30f; // 음식을 기다리는 시간 (인스펙터에서 설정)

    [Header("UI Settings")]
    public GameObject orderUI; // 주문 UI 프리팹 (인스펙터에서 설정)

    [Header("Movement Settings")]
    [SerializeField] private float arrivalDistance = 0.5f; // 도착 판정 거리

    [Header("Animation Settings")]
    [SerializeField] private Animator animator; // 애니메이터 컴포넌트

    private NavMeshAgent _agent;
    private GameObject _instantiatedUI;
    private CustomerOrder _currentOrder;
    private RecipeDefinition _orderedRecipe; // 주문한 레시피
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Vector3 _targetPosition;
    private bool _isInQueue;
    private bool _hasPlacedOrder;
    private Vector3 _lastPosition;
    
    // 대기열 타이머 관련
    private float _queueWaitTimer; // 대기열에서 기다린 시간
    private bool _isWaitingInQueue; // 대기열 대기 중인지 여부

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        
        // 애니메이터가 설정되지 않았다면 자동으로 찾기
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // 원래 위치와 회전 저장
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _lastPosition = transform.position;
        
        // 해금된 레시피 중 랜덤으로 하나 선택
        SelectRandomRecipe();
        
        // 주문 데이터 초기화
        if (_orderedRecipe != null)
        {
            _currentOrder = new CustomerOrder(_orderedRecipe.ResultDish, orderQuantity, foodWaitTimeLimit);
            _currentOrder.OnOrderExpired += HandleOrderExpired;
        }
        
        // 비즈니스 상태 이벤트 구독
        BusinessManager.OnBusinessStateChanged += HandleBusinessStateChanged;
        
        // 영업 중이면 대기열에 참가
        if (BusinessManager.IsBusinessActive)
        {
            JoinQueue();
        }
    }

    /// <summary>해금된 레시피 중 랜덤으로 선택</summary>
    private void SelectRandomRecipe()
    {
        if (CookingManager.Instance == null) return;

        var unlockedRecipes = CookingManager.Instance.GetAvailableRecipes();
        if (unlockedRecipes != null && unlockedRecipes.Length > 0)
        {
            _orderedRecipe = unlockedRecipes[Random.Range(0, unlockedRecipes.Length)];
        }
    }

    private void Update()
    {
        // 대기열 타이머 업데이트
        if (_isWaitingInQueue && !_hasPlacedOrder)
        {
            _queueWaitTimer += Time.deltaTime;
            
            // 대기열 타임아웃 체크
            if (_queueWaitTimer >= queueWaitTimeLimit)
            {
                Debug.Log($"대기열 타임아웃: 주문 차례가 오지 않아 떠남");
                HandleQueueTimeout();
                return;
            }
        }
        
        // 주문 타이머 업데이트 (주문을 한 후에만)
        if (_currentOrder != null && _hasPlacedOrder)
        {
            _currentOrder.UpdateTimer(Time.deltaTime);
        }

        // 움직임 감지 및 애니메이션 제어
        UpdateWalkingAnimation();
        
        // 목표 위치에 도달했는지 확인
        CheckArrival();
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        BusinessManager.OnBusinessStateChanged -= HandleBusinessStateChanged;
        if (_currentOrder != null)
        {
            _currentOrder.OnOrderExpired -= HandleOrderExpired;
        }
    }

    /// <summary>
    /// 비즈니스 상태 변경 처리
    /// </summary>
    private void HandleBusinessStateChanged(bool isActive)
    {
        if (isActive)
        {
            JoinQueue();
        }
        else
        {
            LeaveQueue();
            ReturnToOriginalPosition();
        }
    }

    /// <summary>
    /// 대기열에 참가
    /// </summary>
    private void JoinQueue()
    {
        if (OrderManager.Instance != null && !_isInQueue)
        {
            OrderManager.Instance.EnqueueCustomer(this);
            _isInQueue = true;
            _isWaitingInQueue = true;
            _queueWaitTimer = 0f; // 대기열 타이머 시작
            
            // 대기열에 들어갈 때 UI 생성 (주문 아이템 미리 표시)
            CreateQueueUI();
        }
    }
    
    /// <summary>
    /// 대기열 UI 생성
    /// </summary>
    private void CreateQueueUI()
    {
        if (orderUI != null && _instantiatedUI == null && _orderedRecipe != null)
        {
            _instantiatedUI = Instantiate(orderUI, transform);
            
            var orderUIComponent = _instantiatedUI.GetComponent<OrderUI>();
            if (orderUIComponent != null)
            {
                // 기존 Setup 메서드 사용 (ItemDefinition)
                orderUIComponent.Setup(_orderedRecipe.ResultDish, orderQuantity);
            }
        }
    }

    /// <summary>
    /// 대기열에서 나가기
    /// </summary>
    private void LeaveQueue()
    {
        if (OrderManager.Instance != null && _isInQueue)
        {
            OrderManager.Instance.DequeueCustomer(this);
            _isInQueue = false;
        }
        
        _isWaitingInQueue = false;
        _queueWaitTimer = 0f;
        
        // 주문 UI 제거
        RemoveOrderUI();
        
        // 주문 비활성화
        if (_currentOrder != null)
        {
            _currentOrder.DeactivateOrder();
        }
        
        _hasPlacedOrder = false;
    }

    /// <summary>
    /// 대기열 타임아웃 처리
    /// </summary>
    private void HandleQueueTimeout()
    {
        LeaveQueue();
        ReturnToOriginalPosition();
    }

    /// <summary>
    /// OrderManager에서 호출되는 목표 위치 설정
    /// </summary>
    public void SetTargetPosition(Vector3 position)
    {
        _targetPosition = position;
        if (_agent != null && _agent.isActiveAndEnabled)
        {
            _agent.SetDestination(_targetPosition);
        }
    }

    /// <summary>
    /// 대기열에서 제거될 때 호출
    /// </summary>
    public void OnRemovedFromQueue()
    {
        _isInQueue = false;
        _isWaitingInQueue = false;
        _queueWaitTimer = 0f;
        RemoveOrderUI();
        if (_currentOrder != null)
        {
            _currentOrder.DeactivateOrder();
        }
        _hasPlacedOrder = false;
    }

    /// <summary>
    /// 목표 위치 도달 확인
    /// </summary>
    private void CheckArrival()
    {
        if (Vector3.Distance(transform.position, _targetPosition) <= arrivalDistance)
        {
            // 첫 번째 손님이고 아직 주문하지 않았다면 주문 생성
            if (OrderManager.Instance != null && 
                OrderManager.Instance.IsFirstInQueue(this) && 
                !_hasPlacedOrder &&
                BusinessManager.IsBusinessActive)
            {
                PlaceOrder();
            }
        }
    }

    /// <summary>
    /// 주문 생성 (대기열 타이머 중지, 음식 대기 타이머 시작)
    /// </summary>
    private void PlaceOrder()
    {
        // UI가 이미 생성되어 있으므로 중복 생성하지 않음
        // 주문 타이머 시작 (음식 대기 타이머)
        if (_currentOrder != null)
        {
            _currentOrder.ActivateOrder();
            _hasPlacedOrder = true;
            _isWaitingInQueue = false; // 대기열 타이머 중지
            
            Debug.Log($"주문 완료: {_orderedRecipe.RecipeName}, 대기 시간: {_queueWaitTimer:F1}초");
        }
    }

    /// <summary>
    /// 주문 UI 제거
    /// </summary>
    private void RemoveOrderUI()
    {
        if (_instantiatedUI != null)
        {
            Destroy(_instantiatedUI);
            _instantiatedUI = null;
        }
    }

    /// <summary>
    /// 주문 시간 초과 처리 (음식 대기 시간 초과)
    /// </summary>
    private void HandleOrderExpired(CustomerOrder expiredOrder)
    {
        Debug.Log($"음식 대기 시간 초과: {expiredOrder.orderItem.DisplayName}");
        LeaveQueue();
        ReturnToOriginalPosition();
    }

    /// <summary>
    /// 주문 완료 처리 (OrderUI에서 호출)
    /// </summary>
    public void OnOrderCompleted()
    {
        if (_currentOrder != null)
        {
            _currentOrder.CompleteOrder();
        }
        
        LeaveQueue();
        ReturnToOriginalPosition();
    }

    /// <summary>
    /// 원래 위치로 복귀
    /// </summary>
    private void ReturnToOriginalPosition()
    {
        if (_agent != null && _agent.isActiveAndEnabled)
        {
            _agent.SetDestination(_originalPosition);
        }
    }

    /// <summary>
    /// 현재 주문 정보 반환
    /// </summary>
    public CustomerOrder GetCurrentOrder()
    {
        return _currentOrder;
    }
    
    /// <summary>
    /// 대기열 타이머 정보 반환 (UI 표시용)
    /// </summary>
    public float GetQueueWaitTime()
    {
        return _queueWaitTimer;
    }
    
    /// <summary>
    /// 대기열 대기 중인지 반환
    /// </summary>
    public bool IsWaitingInQueue()
    {
        return _isWaitingInQueue;
    }
    
    /// <summary>
    /// 남은 대기열 시간 반환
    /// </summary>
    public float GetRemainingQueueTime()
    {
        if (!_isWaitingInQueue) return 0f;
        return Mathf.Max(0f, queueWaitTimeLimit - _queueWaitTimer);
    }

    /// <summary>
    /// 움직임을 감지하여 걷는 애니메이션을 제어
    /// </summary>
    private void UpdateWalkingAnimation()
    {
        if (animator != null)
        {
            // 현재 위치와 이전 위치를 비교하여 움직임 감지
            float movementDistance = Vector3.Distance(transform.position, _lastPosition);
            bool isWalking = movementDistance > 0.01f; // 아주 작은 움직임도 감지
            
            animator.SetBool("isWalkF", isWalking);
            
            // 이전 위치 업데이트
            _lastPosition = transform.position;
        }
    }

    /// <summary>
    /// 주문한 레시피 정보 반환 (OrderUI에서 사용)
    /// </summary>
    public RecipeDefinition GetOrderedRecipe()
    {
        return _orderedRecipe;
    }
}

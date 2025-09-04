using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전체 주문을 관리하는 매니저
/// 단일 책임: 주문 대기열 관리 및 줄 서기 위치 계산
/// </summary>
public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Queue Settings")]
    [SerializeField] private Transform truckOrderPoint; // 트럭의 주문 접수 지점
    [SerializeField] private float customerSpacing = 2f; // 손님 간 간격
    [SerializeField] private Vector3 queueDirection = Vector3.forward; // 줄 서는 방향 (트럭 기준)

    private Queue<CustomerOrderSystem> _orderQueue = new Queue<CustomerOrderSystem>();
    private List<CustomerOrderSystem> _queuedCustomers = new List<CustomerOrderSystem>();

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
    /// 손님을 대기열에 추가
    /// </summary>
    public void EnqueueCustomer(CustomerOrderSystem customer)
    {
        if (customer == null || _queuedCustomers.Contains(customer)) return;

        _orderQueue.Enqueue(customer);
        _queuedCustomers.Add(customer);
        UpdateQueuePositions();
    }

    /// <summary>
    /// 주문이 완료되거나 취소된 손님을 대기열에서 제거
    /// </summary>
    public void DequeueCustomer(CustomerOrderSystem customer)
    {
        if (customer == null) return;

        // Queue에서는 직접 제거할 수 없으므로 리스트로 재구성
        var tempList = new List<CustomerOrderSystem>();
        while (_orderQueue.Count > 0)
        {
            var c = _orderQueue.Dequeue();
            if (c != customer)
                tempList.Add(c);
        }

        // Queue 재구성
        foreach (var c in tempList)
            _orderQueue.Enqueue(c);

        _queuedCustomers.Remove(customer);
        UpdateQueuePositions();
    }

    /// <summary>
    /// 첫 번째 손님이 있는지 확인
    /// </summary>
    public bool HasCustomersInQueue()
    {
        return _queuedCustomers.Count > 0;
    }

    /// <summary>
    /// 첫 번째 손님인지 확인
    /// </summary>
    public bool IsFirstInQueue(CustomerOrderSystem customer)
    {
        return _queuedCustomers.Count > 0 && _queuedCustomers[0] == customer;
    }

    /// <summary>
    /// 대기열의 모든 손님 위치 업데이트
    /// </summary>
    private void UpdateQueuePositions()
    {
        if (truckOrderPoint == null) return;

        for (int i = 0; i < _queuedCustomers.Count; i++)
        {
            Vector3 queuePosition = CalculateQueuePosition(i);
            _queuedCustomers[i].SetTargetPosition(queuePosition);
        }
    }

    /// <summary>
    /// 대기열에서의 위치 계산 (트럭 기준 방향)
    /// </summary>
    private Vector3 CalculateQueuePosition(int queueIndex)
    {
        if (truckOrderPoint == null) return Vector3.zero;

        // 트럭의 방향을 기준으로 줄 서는 방향 계산
        Vector3 truckForward = truckOrderPoint.forward;
        Vector3 actualQueueDirection = truckForward * queueDirection.z + 
                                     truckOrderPoint.right * queueDirection.x + 
                                     truckOrderPoint.up * queueDirection.y;

        // 첫 번째는 주문 지점, 나머지는 간격을 두고 배치
        Vector3 position = truckOrderPoint.position;
        if (queueIndex > 0)
        {
            position -= actualQueueDirection.normalized * (queueIndex * customerSpacing);
        }

        return position;
    }

    /// <summary>
    /// 영업 종료 시 모든 대기열 초기화
    /// </summary>
    public void ClearQueue()
    {
        // 모든 손님에게 대기열 해제 알림
        foreach (var customer in _queuedCustomers)
        {
            customer.OnRemovedFromQueue();
        }

        _orderQueue.Clear();
        _queuedCustomers.Clear();
    }

    private void Start()
    {
        BusinessManager.OnBusinessStateChanged += HandleBusinessStateChanged;
    }

    private void OnDestroy()
    {
        BusinessManager.OnBusinessStateChanged -= HandleBusinessStateChanged;
    }

    private void HandleBusinessStateChanged(bool isActive)
    {
        if (!isActive)
        {
            ClearQueue();
        }
    }
}

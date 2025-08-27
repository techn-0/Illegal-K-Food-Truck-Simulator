using UnityEngine;
using UnityEngine.AI;

public class CustomerOrderSystem : MonoBehaviour
{
    [Header("Order Settings")]
    public Transform orderCounter; // 주문 창구 위치
    public ItemDefinition orderItem; // 주문 아이템
    [Range(1, 5)] public int orderQuantity = 1; // 주문 수량

    [Header("UI Settings")]
    public GameObject orderUI; // 주문 UI 프리팹

    private NavMeshAgent agent;
    private GameObject instantiatedUI;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToOrderCounter();
    }

    private void MoveToOrderCounter()
    {
        if (orderCounter != null)
        {
            agent.SetDestination(orderCounter.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == orderCounter)
        {
            PlaceOrder();
        }
    }

    private void PlaceOrder()
    {
        if (orderUI != null && instantiatedUI == null)
        {
            instantiatedUI = Instantiate(orderUI, transform);
            instantiatedUI.GetComponent<OrderUI>().Setup(orderItem, orderQuantity);
        }
    }
}

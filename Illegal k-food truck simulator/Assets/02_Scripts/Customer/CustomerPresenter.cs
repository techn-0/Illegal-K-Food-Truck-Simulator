using UnityEngine;

public class CustomerPresenter : MonoBehaviour
{
    [SerializeField] private CustomerView customerView;
    [SerializeField] private Transform truckTransform; // 트럭 위치를 인스펙터에서 지정
    private CustomerModel customerModel;

    public void Initialize(string customerId, string initialOrder)
    {
        customerModel = new CustomerModel(customerId, initialOrder);
        customerView.ShowOrderUI(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CustomerTrigger")) // 손님이 트리거에 들어왔을 때
        {
            customerView.MoveToTruck(truckTransform.position);
            customerView.ShowOrderUI(true);
        }
    }

    public void ServeOrder()
    {
        if (!customerModel.IsServed)
        {
            customerModel.ServeOrder();
            customerView.ShowOrderUI(false);
            Debug.Log($"Order served for customer {customerModel.CustomerId}");
        }
    }
}

using UnityEngine;

public class CustomerView : MonoBehaviour
{
    [SerializeField] private Animator animator; // 손님 애니메이션
    [SerializeField] private GameObject orderUI; // 주문 UI

    public void MoveToTruck(Vector3 truckPosition)
    {
        // 손님이 푸드트럭으로 이동하는 애니메이션 트리거
        animator.SetTrigger("MoveToTruck");
        transform.position = Vector3.MoveTowards(transform.position, truckPosition, Time.deltaTime);
    }

    public void ShowOrderUI(bool show)
    {
        // 주문 UI 표시/숨김
        orderUI.SetActive(show);
    }
}

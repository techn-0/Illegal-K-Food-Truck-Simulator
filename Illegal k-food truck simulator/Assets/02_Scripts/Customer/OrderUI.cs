using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemQuantity;
    public Button sellButton;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // UI가 항상 카메라를 바라보도록 설정
        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0; // 수직 축 회전을 방지
        transform.rotation = Quaternion.LookRotation(-direction);
    }

    public void Setup(ItemDefinition item, int quantity)
    {
        if (item != null)
        {
            itemIcon.sprite = item.Icon;
            itemName.text = item.DisplayName;
            itemQuantity.text = quantity.ToString();
        }

        // 판매 버튼은 현재 더미로 설정
        sellButton.onClick.AddListener(() => Debug.Log("Sell button clicked"));
    }
}

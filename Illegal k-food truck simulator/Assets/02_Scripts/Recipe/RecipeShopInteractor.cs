using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 레시피 상점과의 상호작용을 처리하는 컴포넌트
/// 단일 책임: 플레이어와 상점 간의 상호작용 감지 및 UI 표시
/// </summary>
public class RecipeShopInteractor : MonoBehaviour
{
    [Header("Shop Reference")]
    [SerializeField] private RecipeShop recipeShop;
    
    [Header("UI Reference")]
    [SerializeField] private GameObject shopUI; // 상점 UI 프리팹 또는 오브젝트
    
    private InputAction interactAction;
    private bool playerInRange = false;
    
    private void OnEnable()
    {
        // E키 상호작용 설정
        interactAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
        interactAction.AddBinding("<Gamepad>/buttonSouth");
        interactAction.performed += OnInteractPerformed;
        interactAction.Enable();
    }
    
    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
            interactAction.Disable();
            interactAction.Dispose();
        }
    }
    
    private void Start()
    {
        // 상점이 설정되지 않았다면 같은 오브젝트에서 찾기
        if (recipeShop == null)
        {
            recipeShop = GetComponent<RecipeShop>();
        }
        
        // 초기에는 상점 UI 비활성화
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 상점 범위에 들어왔을 때
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("상점 범위에 들어왔습니다. E키를 눌러 상점을 열어보세요.");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 상점 범위에서 나갔을 때
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            CloseShop();
        }
    }
    
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!playerInRange || recipeShop == null) return;
        
        // 상점 UI 토글
        if (shopUI != null)
        {
            bool isActive = shopUI.activeSelf;
            shopUI.SetActive(!isActive);
            
            if (!isActive)
            {
                OpenShop();
            }
            else
            {
                CloseShop();
            }
        }
    }
    
    private void OpenShop()
    {
        Debug.Log("레시피 상점이 열렸습니다.");
        // 상점 UI가 열릴 때 필요한 추가 로직
        
        // 커서 표시 (선택사항)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void CloseShop()
    {
        if (shopUI != null && shopUI.activeSelf)
        {
            shopUI.SetActive(false);
            Debug.Log("레시피 상점이 닫혔습니다.");
            
            // 커서 숨기기 (선택사항)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    /// <summary>
    /// 상점 참조 반환 (UI에서 사용)
    /// </summary>
    public RecipeShop GetRecipeShop()
    {
        return recipeShop;
    }
}

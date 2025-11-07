using UnityEngine;

/// <summary>
/// 요리 가능 영역(푸드트럭 등)과의 상호작용을 처리
/// </summary>
public class CookingInteractor : MonoBehaviour
{
    public static CookingInteractor Instance { get; private set; }

    public event System.Action<bool> OnPlayerRangeChanged;
    
    private bool playerInRange = false;
    
    void Awake()
    {
        // 씬마다 새로운 CookingInteractor가 필요하므로 기존 인스턴스를 교체
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            OnPlayerRangeChanged?.Invoke(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            OnPlayerRangeChanged?.Invoke(false);
        }
    }
    
    public bool IsPlayerInCookingRange()
    {
        return playerInRange;
    }
}

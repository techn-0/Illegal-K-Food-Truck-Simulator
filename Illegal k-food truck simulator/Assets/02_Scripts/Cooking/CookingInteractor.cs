using UnityEngine;

/// <summary>
/// 요리 가능 영역(푸드트럭 등)과의 상호작용을 처리
/// </summary>
public class CookingInteractor : MonoBehaviour
{
    public static CookingInteractor Instance { get; private set; }
    
    private bool playerInRange = false;
    
    void Awake()
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
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
    public bool IsPlayerInCookingRange()
    {
        return playerInRange;
    }
}


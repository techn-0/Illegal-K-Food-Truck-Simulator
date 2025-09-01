using UnityEngine;
using System;

/// <summary>
/// 플레이어의 돈을 관리하는 시스템
/// 단일 책임: 플레이어의 돈 관련 로직만 담당
/// </summary>
public class PlayerMoneyManager : MonoBehaviour
{
    [Header("Money Settings")]
    [SerializeField] private int currentMoney = 0;
    
    /// <summary>
    /// 돈이 변경될 때 호출되는 이벤트 (현재 돈)
    /// </summary>
    public static event Action<int> OnMoneyChanged;
    
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    public static PlayerMoneyManager Instance { get; private set; }
    
    /// <summary>
    /// 현재 소지 금액 (읽기 전용)
    /// </summary>
    public int CurrentMoney => currentMoney;
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 초기 돈 이벤트 호출
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    /// <summary>
    /// 돈을 추가합니다
    /// </summary>
    /// <param name="amount">추가할 금액</param>
    /// <returns>성공 여부</returns>
    public bool AddMoney(int amount)
    {
        if (amount <= 0) return false;
        
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        
        Debug.Log($"돈 추가: +{amount}원 (현재: {currentMoney}원)");
        return true;
    }
    
    /// <summary>
    /// 돈을 차감합니다
    /// </summary>
    /// <param name="amount">차감할 금액</param>
    /// <returns>성공 여부 (잔액 부족시 false)</returns>
    public bool SpendMoney(int amount)
    {
        if (amount <= 0 || currentMoney < amount) return false;
        
        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);
        
        Debug.Log($"돈 차감: -{amount}원 (현재: {currentMoney}원)");
        return true;
    }
    
    /// <summary>
    /// 특정 금액을 지불할 수 있는지 확인
    /// </summary>
    /// <param name="amount">필요한 금액</param>
    /// <returns>지불 가능 여부</returns>
    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }
}

using UnityEngine;

/// <summary>
/// 플레이어가 침대와 상호작용할 때 하루 종료 UI를 띄움
/// </summary>
public class BedInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EndDayUI endDayUI;

    /// <summary>
    /// 침대와 상호작용 (E키 또는 상호작용 버튼)
    /// </summary>
    public void Interact()
    {
        if (endDayUI != null)
        {
            endDayUI.ShowConfirm();
        }
        else
        {
            Debug.LogWarning("EndDayUI가 연결되지 않았습니다. Inspector에서 EndDayUI를 할당하세요.");
        }
    }

    // 트리거 방식으로도 사용 가능하도록 예시 메서드 추가
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 입력 처리는 PlayerController 등에서 호출하도록 설계
            // 여기서는 상호작용 가능 표시만 할 수 있음
        }
    }
}


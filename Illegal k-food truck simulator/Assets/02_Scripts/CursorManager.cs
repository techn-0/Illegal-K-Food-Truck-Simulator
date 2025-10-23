using UnityEngine;

/// <summary>
/// 마우스 커서의 표시/숨김 및 잠금 상태를 관리하는 클래스
/// Alt 키를 눌러 커서 모드를 토글할 수 있습니다.
/// </summary>
public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private bool startWithCursorLocked = true; // 시작 시 커서 잠금 여부

    [Header("Dependencies")]
    [SerializeField] private MonoBehaviour cinemachineInputAxisController; // Cinemachine Input Axis Controller 참조

    private bool isCursorLocked = true; // 현재 커서 잠금 상태
    private int activeUIWindows = 0; // 활성화된 UI 창의 개수

    void Start()
    {
        // 게임 시작 시 커서 상태 설정
        SetCursorState(startWithCursorLocked);
    }

    void Update()
    {
        // Alt 키 입력 감지
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            ToggleCursorState();
        }
    }

    /// <summary>
    /// 커서 상태를 토글합니다
    /// </summary>
    private void ToggleCursorState()
    {
        // UI 창이 열려 있으면 Alt 키로 커서 상태를 변경하지 않음
        if (activeUIWindows > 0) return;

        SetCursorState(!isCursorLocked);

        // Cinemachine Input Axis Controller 활성화/비활성화
        if (cinemachineInputAxisController != null)
        {
            cinemachineInputAxisController.enabled = isCursorLocked;
        }
    }

    /// <summary>
    /// 커서 상태를 설정합니다
    /// </summary>
    /// <param name="locked">true: 커서 숨김 및 잠금, false: 커서 표시 및 해제</param>
    private void SetCursorState(bool locked)
    {
        isCursorLocked = locked;

        if (locked)
        {
            // 커서 숨기고 화면 중앙에 잠금
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // 커서 표시하고 자유롭게 이동 가능
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Debug.Log($"커서 상태 변경: {(locked ? "잠금" : "해제")}");

        // Cinemachine Input Axis Controller 활성화/비활성화
        if (cinemachineInputAxisController != null)
        {
            cinemachineInputAxisController.enabled = locked;
        }
    }

    /// <summary>
    /// 현재 커서가 잠겨있는지 확인
    /// </summary>
    public bool IsCursorLocked => isCursorLocked;

    /// <summary>
    /// 외부에서 커서 상태를 강제로 설정할 때 사용
    /// </summary>
    /// <param name="locked">설정할 커서 잠금 상태</param>
    public void ForceCursorState(bool locked)
    {
        SetCursorState(locked);
    }

    /// <summary>
    /// UI 창이 열릴 때 호출
    /// </summary>
    public void OnUIWindowOpened()
    {
        activeUIWindows++;
        SetCursorState(false); // 커서 활성화
    }

    /// <summary>
    /// UI 창이 닫힐 때 호출
    /// </summary>
    public void OnUIWindowClosed()
    {
        if (activeUIWindows > 0)
        {
            activeUIWindows--;
        }

        // 모든 UI 창이 닫혔을 때만 커서 상태를 원래대로 복원
        if (activeUIWindows == 0)
        {
            SetCursorState(startWithCursorLocked);
        }
    }
}

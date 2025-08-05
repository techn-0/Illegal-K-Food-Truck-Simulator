using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineInputAxisController))]
public class CameraDragToggle : MonoBehaviour
{
    [SerializeField] private InputActionReference camDrag;
    private CinemachineInputAxisController axisCtrl;

    void Awake()
    {
        axisCtrl = GetComponent<CinemachineInputAxisController>();

        camDrag.action.started  += _ => EnableDrag(true);
        camDrag.action.canceled += _ => EnableDrag(false);
        camDrag.action.Enable();

        EnableDrag(false);   // 처음엔 꺼둔다
    }

    void EnableDrag(bool on)
    {
        axisCtrl.enabled   = on;                       // ← ★ 핵심
        Cursor.lockState   = on ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible     = !on;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] Transform cameraTransform; // 카메라 Transform 참조
    
    CharacterController _cc;
    Vector2 _moveInput;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        
        // 카메라 Transform이 설정되지 않았다면 Main Camera 사용
        if (cameraTransform == null)
            cameraTransform = Camera.main?.transform;
    }

    void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();

    void Update()
    {
        // CharacterController가 활성화되어 있는지 확인
        if (_cc == null || !_cc.enabled || !_cc.gameObject.activeInHierarchy) return;
        if (cameraTransform == null) return;
        
        // 카메라 기준 방향 벡터 계산
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        // Y축 제거 (수평면에서만 움직임)
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // 입력에 따른 움직임 방향 계산
        Vector3 dir = cameraRight * _moveInput.x + cameraForward * _moveInput.y;
        
        // 대각선 이동 속도 보정: 입력값의 크기를 1로 제한
        if (dir.sqrMagnitude > 1f)
            dir = dir.normalized;
            
        if (dir.sqrMagnitude > 0)
            _cc.Move(dir * (speed * Time.deltaTime));

        // 임시 중력
        if (!_cc.isGrounded) _cc.Move(Physics.gravity * Time.deltaTime);
    }
}

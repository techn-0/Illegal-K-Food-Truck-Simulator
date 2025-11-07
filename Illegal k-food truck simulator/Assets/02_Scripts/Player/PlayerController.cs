using UnityEngine;
using UnityEngine.InputSystem;
using _02_Scripts.Player;

namespace _02_Scripts
{
    /// <summary>
    /// 플레이어 캐릭터의 이동과 기본 UI 인터랙션을 처리하는 컨트롤러
    /// 카메라 기준 3D 이동, 인벤토리 UI 토글 기능 제공
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float speed = 5f;
        [SerializeField] Transform cameraTransform; // 카메라 Transform 참조
        [SerializeField] InventoryView inventoryView; // 인벤토리 UI 참조
        [SerializeField] CookingUI cookingUI; // 쿠킹 UI 참조
    
        CharacterController _cc;
        Vector2 _moveInput; // Input System에서 받은 이동 입력값
        PlayerAnimationPresenter _animationPresenter;

        // 침대 상호작용을 위한 변수
        private BedInteractor _nearbyBed;

        /// <summary>
        /// 초기화: 필수 컴포넌트 연결 및 카메라 자동 할당
        /// </summary>
        void Awake()
        {
            _cc = GetComponent<CharacterController>();
        
            // 카메라 Transform이 설정되지 않았다면 Main Camera 자동 할당
            if (cameraTransform == null)
                cameraTransform = Camera.main?.transform;

            var animationModel = new PlayerAnimationModel();
            var animationView = GetComponentInChildren<PlayerAnimationView>();
            _animationPresenter = new PlayerAnimationPresenter(animationModel, animationView);
        }

        /// <summary>
        /// Input System 콜백: 이동 입력 처리 (WASD, 조이스틱)
        /// </summary>
        void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();

        void Update()
        {
            // 인벤토리 UI 토글 처리 (I키)
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inventoryView != null)
                {
                    bool isActive = !inventoryView.gameObject.activeSelf;
                    inventoryView.gameObject.SetActive(isActive);

                    // CursorManager 호출
                    CursorManager cursorManager = FindObjectOfType<CursorManager>();
                    if (cursorManager != null)
                    {
                        if (isActive)
                        {
                            cursorManager.OnUIWindowOpened();
                        }
                        else
                        {
                            cursorManager.OnUIWindowClosed();
                        }
                    }
                    
                    Debug.Log($"인벤토리 UI: {(isActive ? "열림" : "닫힘")}");
                }
                else
                {
                    Debug.LogWarning("InventoryView가 할당되지 않았습니다!");
                }
            }

            // 쿠킹 UI 토글 처리 (C키)
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (cookingUI != null)
                {
                    bool isActive = !cookingUI.gameObject.activeSelf;
                    cookingUI.gameObject.SetActive(isActive);

                    // CursorManager 호출
                    CursorManager cursorManager = FindObjectOfType<CursorManager>();
                    if (cursorManager != null)
                    {
                        if (isActive)
                        {
                            cursorManager.OnUIWindowOpened();
                        }
                        else
                        {
                            cursorManager.OnUIWindowClosed();
                        }
                    }
                    
                    Debug.Log($"쿠킹 UI: {(isActive ? "열림" : "닫힘")}");
                }
                else
                {
                    Debug.LogWarning("CookingUI가 할당되지 않았습니다!");
                }
            }

            // E키로 침대 상호작용
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_nearbyBed != null)
                {
                    _nearbyBed.Interact();
                }
            }

            // 이동 처리를 위한 기본 검증
            if (_cc == null || !_cc.enabled || !_cc.gameObject.activeInHierarchy) return;
            if (cameraTransform == null) return;
        
            // 카메라 기준 방향 벡터 계산 (월드 좌표계)
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
        
            // Y축 제거하여 수평면에서만 이동하도록 제한
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
        
            // 입력값을 카메라 기준 방향으로 변환
            Vector3 dir = cameraRight * _moveInput.x + cameraForward * _moveInput.y;
        
            // 대각선 이동 시 속도 보정 (벡터 크기를 1로 제한)
            if (dir.sqrMagnitude > 1f)
                dir = dir.normalized;
            
            // 실제 이동 처리
            if (dir.sqrMagnitude > 0)
            {
                _cc.Move(dir * (speed * Time.deltaTime));

                // 캐릭터가 이동 방향을 바라보도록 회전
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // 기본 중력 처리 (땅에 붙어있지 않을 때)
            if (!_cc.isGrounded) _cc.Move(Physics.gravity * Time.deltaTime);

            // 이동 상태에 따라 애니메이션 업데이트
            bool isWalking = _moveInput.sqrMagnitude > 0;
            _animationPresenter.UpdateWalkingState(isWalking);
        }

        // 침대 트리거 진입
        private void OnTriggerEnter(Collider other)
        {
            BedInteractor bed = other.GetComponent<BedInteractor>();
            if (bed != null)
            {
                _nearbyBed = bed;
                Debug.Log("침대 근처입니다. E키를 눌러 상호작용하세요.");
            }
        }

        // 침대 트리거 벗어남
        private void OnTriggerExit(Collider other)
        {
            BedInteractor bed = other.GetComponent<BedInteractor>();
            if (bed != null && _nearbyBed == bed)
            {
                _nearbyBed = null;
                Debug.Log("침대에서 멀어졌습니다.");
            }
        }
    }
}

using UnityEngine;

namespace _02_Scripts
{
    public class BirdMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f; // m/s
        [SerializeField] private float wanderRadius = 5f;
        [SerializeField] private float rotationSpeed = 180f; // degrees/s
        
        [Header("Idle Settings")]
        [SerializeField] private Vector2 idleTimeRange = new Vector2(1f, 3f);
        
        [Header("Home Position")]
        [SerializeField] private Transform homeOverride; // 인스펙터에서 지정 가능한 홈 위치
        
        // Private variables
        private Vector3 _homePosition;
        private Vector3 _targetPosition;
        private bool _isMoving;
        private float _idleTimer;
        private float _currentIdleTime;
        
        void Start()
        {
            // 홈 위치 설정 - homeOverride가 있으면 그 위치를, 없으면 현재 위치를 홈으로 사용
            _homePosition = homeOverride != null ? homeOverride.position : transform.position;
            
            // 첫 번째 목표 지점 설정
            SetNewTarget();
            
            // 첫 번째 정지 시간 설정
            SetNewIdleTime();
        }
        
        void Update()
        {
            if (_isMoving)
            {
                MoveToTarget();
            }
            else
            {
                HandleIdleState();
            }
        }
        
        private void MoveToTarget()
        {
            Vector3 direction = (_targetPosition - transform.position).normalized;
            
            // Y축 이동을 방지하기 위해 direction의 Y 성분을 0으로 설정
            direction.y = 0;
            direction = direction.normalized;
            
            Vector3 moveVector = direction * (moveSpeed * Time.deltaTime);
            
            // 이동 (Y축 변화 방지)
            Vector3 newPosition = transform.position + moveVector;
            newPosition.y = transform.position.y; // Y 위치 고정
            transform.position = newPosition;
            
            // 회전 - 이동 방향을 향하도록 (Y축 회전만)
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            
            // 목표 지점에 도착했는지 확인 (0.1m 이내, Y축 제외)
            Vector3 currentPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetPosFlat = new Vector3(_targetPosition.x, 0, _targetPosition.z);
            if (Vector3.Distance(currentPosFlat, targetPosFlat) < 0.1f)
            {
                _isMoving = false;
                _idleTimer = 0f;
                SetNewIdleTime();
            }
        }
        
        private void HandleIdleState()
        {
            _idleTimer += Time.deltaTime;
            
            if (_idleTimer >= _currentIdleTime)
            {
                SetNewTarget();
                _isMoving = true;
            }
        }
        
        private void SetNewTarget()
        {
            // 홈 위치를 중심으로 wanderRadius 내에서 무작위 지점 선택
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            _targetPosition = _homePosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
            
            // Y축은 현재 새의 Y 위치로 설정하여 수평 이동만 하도록 함
            _targetPosition.y = transform.position.y;
        }
        
        private void SetNewIdleTime()
        {
            _currentIdleTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
        }
        
        // 디버그용 - 홈 위치와 배회 반경을 시각화
        void OnDrawGizmosSelected()
        {
            Vector3 home = homeOverride != null ? homeOverride.position : transform.position;
            
            // 홈 위치 표시
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(home, 0.2f);
            
            // 배회 반경 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(home, wanderRadius);
            
            // 현재 목표 지점 표시 (게임 실행 중일 때만)
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_targetPosition, 0.1f);
                
                // 현재 위치에서 목표까지의 선
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, _targetPosition);
            }
        }
    }
}

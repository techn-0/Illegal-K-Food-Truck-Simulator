using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    CharacterController cc;
    Vector2 moveInput;

    void Awake() => cc = GetComponent<CharacterController>();

    void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    void Update()
    {
        // 쿼터뷰 기준 -Z가 화면 위쪽
        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        if (dir.sqrMagnitude > 0)
            cc.Move(dir * speed * Time.deltaTime);

        // 임시 중력
        if (!cc.isGrounded) cc.Move(Physics.gravity * Time.deltaTime);
    }
}

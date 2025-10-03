using UnityEngine;

namespace _02_Scripts.Player
{
    /// <summary>
    /// 애니메이션 컨트롤러와 상호작용하는 뷰 클래스
    /// </summary>
    public class PlayerAnimationView : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private const string IsWalkFParam = "isWalkF";

        public void SetWalkingAnimation(bool isWalking)
        {
            if (animator != null)
            {
                animator.SetBool(IsWalkFParam, isWalking);
            }
        }
    }
}

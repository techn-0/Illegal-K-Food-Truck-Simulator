using UnityEngine;

namespace _02_Scripts.Player
{
    /// <summary>
    /// 애니메이션 상태를 관리하는 모델 클래스
    /// </summary>
    public class PlayerAnimationModel
    {
        public bool IsWalking { get; private set; }

        public void SetWalking(bool isWalking)
        {
            IsWalking = isWalking;
        }
    }
}

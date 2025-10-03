namespace _02_Scripts.Player
{
    /// <summary>
    /// 모델과 뷰를 연결하는 프레젠터 클래스
    /// </summary>
    public class PlayerAnimationPresenter
    {
        private readonly PlayerAnimationModel _model;
        private readonly PlayerAnimationView _view;

        public PlayerAnimationPresenter(PlayerAnimationModel model, PlayerAnimationView view)
        {
            _model = model;
            _view = view;
        }

        public void UpdateWalkingState(bool isWalking)
        {
            _model.SetWalking(isWalking);
            _view.SetWalkingAnimation(_model.IsWalking);
        }
    }
}

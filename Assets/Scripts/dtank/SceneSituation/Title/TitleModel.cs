using UniRx;

namespace dtank
{
    public class TitleModel
    {
        private readonly ReactiveProperty<TitleState> _currentState = new ReactiveProperty<TitleState>();
        public IReadOnlyReactiveProperty<TitleState> CurrentState => _currentState;

        private readonly ReactiveProperty<bool> _endFlag = new ReactiveProperty<bool>(false);
        public IReadOnlyReactiveProperty<bool> EndFlag => _endFlag;

        public void PushStart()
        {
            _currentState.Value = TitleState.Start;
        }

        public void EndScene()
        {
            _endFlag.Value = true;
        }
    }
}
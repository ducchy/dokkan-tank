using GameFramework.ModelSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class TitleModel : SingleModel<TitleModel>
    {
        private readonly ReactiveProperty<TitleState> _currentState = new ReactiveProperty<TitleState>();
        public IReadOnlyReactiveProperty<TitleState> CurrentState => _currentState;

        private TitleModel(object empty) : base(empty)
        {
        }

        protected override void OnDeletedInternal()
        {
            base.OnDeletedInternal();
            
            _currentState.Dispose();
        }

        public void PushStart()
        {
            SetState(TitleState.Start);
        }

        public void EndScene()
        {
            SetState(TitleState.End);
        }

        private void SetState(TitleState state)
        {
            _currentState.Value = state;
        }

        public void OnChangedState(TitleState prev, TitleState current)
        {
            Debug.LogFormat("TitleModel.OnChangedState: current={0}", current);

            SetState(current);
        }
    }
}
using GameFramework.ModelSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class TitleModel : SingleModel<TitleModel>
    {
        private readonly ReactiveProperty<TitleState> _currentState = new();
        public IReadOnlyReactiveProperty<TitleState> CurrentState => _currentState;

        private TitleModel(object empty) : base(empty)
        {
        }

        protected override void OnDeletedInternal()
        {
            base.OnDeletedInternal();

            _currentState.Dispose();
        }

        public void ChangeState(TitleState next)
        {
            var current = _currentState.Value;
            if (current == next)
                return;
            
            Debug.Log($"[TitleModel] ChangeState(): {current} -> {next}");

            _currentState.Value = next;
        }
    }
}
using System;
using System.Linq;
using UnityEngine;

namespace dtank
{
    public class BattleTankActor : TankActorBase
    {
        [SerializeField] private AnimatorBody _animator;

        public Action<BattleTankAnimatorState> OnStateExitListener;
        
        public void Construct()
        {
            _animator.Construct();
            
            _animator.OnStateEnterAction = OnStateEnter;
            _animator.OnStateExitAction = OnStateExit;
        }

        public void SetTransform(TransformData data)
        {
            transform.Set(data);
        }

        public void Play(BattleTankAnimatorState state)
        {
            _animator.Play(state.ToStateName());
        }

        private void OnStateEnter(AnimatorStateInfo info)
        {
        }

        private void OnStateExit(AnimatorStateInfo info)
        {
            var state = GetStateFromInfo(info);
            if (state == BattleTankAnimatorState.Invalid)
                return;
            
            OnStateExitListener?.Invoke(state);
        }

        private BattleTankAnimatorState GetStateFromInfo(AnimatorStateInfo info)
        {
            return Enum.GetValues(typeof(BattleTankAnimatorState))
                .Cast<BattleTankAnimatorState>()
                .FirstOrDefault(value => info.shortNameHash == value.ToStateHash());
        }
    }
}
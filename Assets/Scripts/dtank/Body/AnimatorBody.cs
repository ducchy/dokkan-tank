using System;
using UnityEngine;

namespace dtank
{
    public class AnimatorBody : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private StateMachineCallbackReceiver _callbackReceiver;
        
        public Action<AnimatorStateInfo> OnStateEnterAction = null;
        public Action<AnimatorStateInfo> OnStateExitAction = null;
        public Action<string> OnAnimationEventAction = null;

        public void Construct()
        {
            _callbackReceiver = _animator.GetBehaviour<StateMachineCallbackReceiver>();

            if (_callbackReceiver != null)
            {
                _callbackReceiver.OnStateEnterAction = OnStateEnter;
                _callbackReceiver.OnStateExitAction = OnStateExit;
            }
        }

        private void OnDestroy()
        {
            if (_callbackReceiver != null)
            {
                _callbackReceiver.OnStateEnterAction = null;
                _callbackReceiver.OnStateExitAction = null;
            }
        }

        public void Play(string stateName)
        {
            _animator.Play(stateName);
        }

        private void OnStateEnter(AnimatorStateInfo info)
        {
            OnStateEnterAction?.Invoke(info);
        }
        
        private void OnStateExit(AnimatorStateInfo info)
        {
            OnStateExitAction?.Invoke(info);
        }

        private void OnAnimationEvent(string id)
        {
            OnAnimationEventAction?.Invoke(id);
        }
    }
}
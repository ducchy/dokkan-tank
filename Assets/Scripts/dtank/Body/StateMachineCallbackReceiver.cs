using System;
using UnityEngine;

namespace dtank
{
    public class StateMachineCallbackReceiver : StateMachineBehaviour
    {
        public Action<AnimatorStateInfo> OnStateEnterAction = null;
        public Action<AnimatorStateInfo> OnStateExitAction = null;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            OnStateEnterAction?.Invoke(stateInfo);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            OnStateExitAction?.Invoke(stateInfo);
        }
    }
}
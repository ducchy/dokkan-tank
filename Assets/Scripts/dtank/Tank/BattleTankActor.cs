using System;
using System.Linq;
using UnityEngine;

namespace dtank
{
    public class BattleTankActor : TankActorBase
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AnimatorBody _animator;
        [SerializeField] private ShellActor _shellForShotCurve;
        [SerializeField] private ShellActor _shellForShotStraight;
        [SerializeField] private Transform _muzzle;

        [SerializeField] private float _moveSpeed = 12f;
        [SerializeField] private float _turnSpeed = 180f;

        [SerializeField] private float _moveAmount;
        [SerializeField] private float _turnAmount;

        public Action<BattleTankAnimatorState> OnStateExitListener;
        public Action<string> OnAnimationEventListener;

        public void Construct()
        {
            _animator.Construct();

            _animator.OnStateEnterAction = OnStateEnter;
            _animator.OnStateExitAction = OnStateExit;
            _animator.OnAnimationEventAction = OnAnimationEvent;
        }

        public void SetTransform(TransformData data)
        {
            transform.Set(data);
        }

        public void Play(BattleTankAnimatorState state)
        {
            _animator.Play(state.ToStateName());
        }

        public void ShotCurve()
        {
            ShotShell(_shellForShotCurve);
        }

        public void ShotStraight()
        {
            ShotShell(_shellForShotStraight);
        }

        private void ShotShell(ShellActor prefab)
        {
            var position = _muzzle.position;
            var instance = Instantiate(prefab, position, _muzzle.rotation);
            instance.Shot(_muzzle.forward);
        }

        public void SetMoveAmount(float moveAmount)
        {
            _moveAmount = moveAmount;
        }

        public void SetTurnAmount(float turnAmount)
        {
            _turnAmount = turnAmount;
        }

        public void Move(float deltaTime)
        {
            var movement = transform.forward * _moveAmount * _moveSpeed * deltaTime;
            _rigidbody.MovePosition(_rigidbody.position + movement);
        }

        public void Turn(float deltaTime)
        {
            var turn = _turnAmount * _turnSpeed * deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
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

        private void OnAnimationEvent(string id)
        {
            OnAnimationEventListener?.Invoke(id);
        }

        private BattleTankAnimatorState GetStateFromInfo(AnimatorStateInfo info)
        {
            return Enum.GetValues(typeof(BattleTankAnimatorState))
                .Cast<BattleTankAnimatorState>()
                .FirstOrDefault(value => info.shortNameHash == value.ToStateHash());
        }
    }
}
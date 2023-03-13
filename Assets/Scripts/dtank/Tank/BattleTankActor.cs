using System;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace dtank
{
    public interface IAttacker
    {
    }

    public interface IDamageReceiver
    {
        bool ReceiveDamage(IAttacker attacker);
    }

    public class BattleTankActor : TankActorBase, IDamageReceiver, IAttacker
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AnimatorBody _animator;
        [SerializeField] private ShellActor _shellForShotCurve;
        [SerializeField] private ShellActor _shellForShotStraight;
        [SerializeField] private MeshRenderer[] _renderers;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _muzzle;

        [SerializeField] private float _moveSpeed = 12f;
        [SerializeField] private float _turnSpeed = 180f;

        [SerializeField] private float _moveAmount;
        [SerializeField] private float _turnAmount;

        public Action<BattleTankAnimatorState> OnStateExitListener;
        public Action<string> OnAnimationEventListener;
        public Action OnDamageReceivedListener;
        public Action<Vector3> OnPositionChangedListener;
        public Action<Vector3> OnForwardChangedListener;

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
            instance.Shot(this, _muzzle.forward);
        }

        public void SetMoveAmount(float moveAmount)
        {
            _moveAmount = moveAmount;
        }

        public void SetTurnAmount(float turnAmount)
        {
            _turnAmount = turnAmount;
        }

        public void Dead()
        {
            SetActive(false);
        }

        private void SetActive(bool active)
        {
            _collider.enabled = active;
            foreach (var renderer in _renderers)
                renderer.enabled = active;
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            Move(deltaTime);
            Turn(deltaTime);
        }

        private void Move(float deltaTime)
        {
            var movement = transform.forward * _moveAmount * _moveSpeed * deltaTime;
            _rigidbody.velocity = movement;
            
            OnPositionChangedListener?.Invoke(_rigidbody.position);
        }

        private void Turn(float deltaTime)
        {
            var turn = _turnAmount * _turnSpeed * deltaTime;
            _rigidbody.angularVelocity = new Vector3(0f, turn, 0f);
            
            OnForwardChangedListener?.Invoke(_rigidbody.rotation * Vector3.forward);
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

        public bool ReceiveDamage(IAttacker attacker)
        {
            if (attacker == this)
                return false;

            OnDamageReceivedListener?.Invoke();
            return true;
        }
    }
}
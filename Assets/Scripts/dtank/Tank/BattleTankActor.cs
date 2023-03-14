using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace dtank
{
    public interface IAttacker
    {
        void DealDamage();
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
        public Action<IAttacker> OnDamageReceivedListener;
        public Action<Vector3> OnPositionChangedListener;
        public Action<Vector3> OnForwardChangedListener;
        public Action OnDealDamageListener;

        private Sequence _invincibleSeq;
        
        public int OwnerId { get; private set; }

        public void Construct(int ownerId)
        {
            OwnerId = ownerId;
            
            _animator.Construct();

            _animator.OnStateEnterAction = OnStateEnter;
            _animator.OnStateExitAction = OnStateExit;
            _animator.OnAnimationEventAction = OnAnimationEvent;
            
            SetActive(false);
        }

        public void SetTransform(TransformData data)
        {
            transform.Set(data);
            _rigidbody.position = data.Position;
            _rigidbody.rotation = Quaternion.Euler(data.Angle);
            
            OnPositionChangedListener?.Invoke(_rigidbody.position);
            OnForwardChangedListener?.Invoke(_rigidbody.rotation * Vector3.forward);
        }

        public void Play(BattleTankAnimatorState state)
        {
            _animator.Play(state.ToStateName());
        }

        public void Ready()
        {
            SetActive(true);
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
            _invincibleSeq?.Kill();
            
            SetActive(false);
        }

        private void SetActive(bool active)
        {
            _collider.enabled = active;
            SetVisible(active);
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            Move(deltaTime);
            Turn(deltaTime);
        }

        private void Move(float deltaTime)
        {
            var movement = transform.forward * (_moveAmount * _moveSpeed * deltaTime);
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
            if (attacker == (IAttacker)this)
                return false;

            OnDamageReceivedListener?.Invoke(attacker);
            return true;
        }
        
        public void SetInvincible(bool flag)
        {
            _invincibleSeq?.Kill();

            if (!flag) {
                SetVisible(true);
                return;
            }

            SetVisible(false);

            _invincibleSeq = DOTween.Sequence()
                .AppendInterval(0.05f)
                .AppendCallback(() => SetVisible(true))
                .AppendInterval(0.05f)
                .AppendCallback(() => SetVisible(false))
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject)
                .Play();
        }

        private void SetVisible(bool flag)
        {
            foreach (var rend in _renderers)
                rend.enabled = flag;
        }

        public void DealDamage()
        {
            OnDealDamageListener?.Invoke();
        }
    }
}
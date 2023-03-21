using System;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleTankActor : TankActorBase, IDamageReceiver, IAttacker, IDisposable
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AnimatorBody _animator;
        [SerializeField] private ShellActor _shellForShotCurve;
        [SerializeField] private ShellActor _shellForShotStraight;
        [SerializeField] private MeshRenderer[] _renderers;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private GameObject _deadEffectPrefab;
        [SerializeField] private GameObject _fireEffectPrefab;

        [SerializeField] private float _moveSpeed = 12f;
        [SerializeField] private float _turnSpeed = 180f;

        [SerializeField] private float _moveAmount;
        [SerializeField] private float _turnAmount;

        private readonly Subject<BattleTankAnimatorState> _onStateExitSubject = new Subject<BattleTankAnimatorState>();
        public IObservable<BattleTankAnimatorState> OnStateExitAsObservable => _onStateExitSubject;

        private readonly Subject<string> _onAnimationEventSubject = new Subject<string>();
        public IObservable<string> OnAnimationEventAsObservable => _onAnimationEventSubject;

        private readonly Subject<IAttacker> _onDamageReceivedSubject = new Subject<IAttacker>();
        public IObservable<IAttacker> OnDamageReceivedAsObservable => _onDamageReceivedSubject;

        private readonly Subject<Vector3> _onPositionChangedSubject = new Subject<Vector3>();
        public IObservable<Vector3> OnPositionChangedAsObservable => _onPositionChangedSubject;

        private readonly Subject<Vector3> _onForwardChangedSubject = new Subject<Vector3>();
        public IObservable<Vector3> OnForwardChangedAsObservable => _onForwardChangedSubject;

        private readonly Subject<Unit> _onDealDamageSubject = new Subject<Unit>();
        public IObservable<Unit> OnDealDamageAsObservable => _onDealDamageSubject;

        private Sequence _invincibleSeq;
        
        public int OwnerId { get; private set; }

        public void Setup(int ownerId)
        {
            OwnerId = ownerId;
            
            _animator.Construct();

            _animator.OnStateEnterAction = OnStateEnter;
            _animator.OnStateExitAction = OnStateExit;
            _animator.OnAnimationEventAction = OnAnimationEvent;
            
            SetActive(false);
        }

        public void Dispose()
        {
            _invincibleSeq.Kill();
            
            _onStateExitSubject.Dispose();
            _onAnimationEventSubject.Dispose();
            _onDamageReceivedSubject.Dispose();
            _onPositionChangedSubject.Dispose();
            _onForwardChangedSubject.Dispose();
            _onDealDamageSubject.Dispose();
        }

        public void SetTransform(TransformData data)
        {
            transform.Set(data);
            _rigidbody.position = data.Position;
            _rigidbody.rotation = Quaternion.Euler(data.Angle);
            
            _onPositionChangedSubject.OnNext(_rigidbody.position);
            _onForwardChangedSubject.OnNext(_rigidbody.rotation * Vector3.forward);
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
            var forward = _muzzle.forward;
            instance.Shot(this, forward);

            var fireEffect = Instantiate(_fireEffectPrefab);
            fireEffect.transform.position = position + forward;
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

            var deadEffect = Instantiate(_deadEffectPrefab);
            deadEffect.transform.position = transform.position;
            
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
            
            _onPositionChangedSubject.OnNext(_rigidbody.position);
        }

        private void Turn(float deltaTime)
        {
            var turn = _turnAmount * _turnSpeed * deltaTime;
            _rigidbody.angularVelocity = new Vector3(0f, turn, 0f);
            
            _onForwardChangedSubject.OnNext(_rigidbody.rotation * Vector3.forward);
        }

        private void OnStateEnter(AnimatorStateInfo info)
        {
        }

        private void OnStateExit(AnimatorStateInfo info)
        {
            var state = GetStateFromInfo(info);
            if (state == BattleTankAnimatorState.Invalid)
                return;

            _onStateExitSubject.OnNext(state);
        }

        private void OnAnimationEvent(string id)
        {
            _onAnimationEventSubject.OnNext(id);
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

            _onDamageReceivedSubject.OnNext(attacker);
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
            _onDealDamageSubject.OnNext(Unit.Default); 
        }
    }
}
using System;
using System.Linq;
using DG.Tweening;
using GameFramework.BodySystems;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.EntitySystems;
using GameFramework.PlayableSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleTankActor : Actor, IDamageReceiver, IAttacker
    {
        public IBattleTankActorSetupData Data { get; private set; }
        public TransformData StartPointData { get; private set; }

        // 行動キャンセル通知用
        private DisposableScope _actionScope = new DisposableScope();

        // コルーチン実行用
        private readonly CoroutineRunner _coroutineRunner;

        private ShellActor _shellForShotCurve;
        private ShellActor _shellForShotStraight;
        private MeshRenderer[] _renderers;
        private Collider _collider;
        private Transform _muzzle;
        private GameObject _deadEffectPrefab;
        private GameObject _fireEffectPrefab;

        private MotionController _motionController;
        private readonly MoveController _moveController;
        private AnimatorControllerPlayableProvider _playableProvider;

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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleTankActor(Body body, IBattleTankActorSetupData setupData, TransformData startPointData)
            : base(body)
        {
            Data = setupData;
            StartPointData = startPointData;
            
            // _statusEventListener = body.GetComponent<StatusEventListener>();
            _motionController = body.GetController<MotionController>();
            _coroutineRunner = new CoroutineRunner();
            _moveController = new MoveController(body.GetComponent<Rigidbody>(), Data.MoveMaxSpeed, Data.TurnMaxSpeed,
                pos => _onPositionChangedSubject.OnNext(pos),
                fwd => _onForwardChangedSubject.OnNext(fwd));

            _renderers = body.GetComponents<MeshRenderer>();
            _collider = body.GetComponent<Collider>();
            
            _moveController.SetTransform(startPointData);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            
            _invincibleSeq.Kill();
            
            _coroutineRunner.Dispose();
            _moveController.Dispose();

            _onStateExitSubject.Dispose();
            _onAnimationEventSubject.Dispose();
            _onDamageReceivedSubject.Dispose();
            _onPositionChangedSubject.Dispose();
            _onForwardChangedSubject.Dispose();
            _onDealDamageSubject.Dispose();
        }

        protected override void ActivateInternal(IScope scope)
        {
            base.ActivateInternal(scope);
            // 基本モーションの設定
            _playableProvider = _motionController.Player.Change(Data.Controller, 0.0f, false);
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();
            
            _moveController.Update(Time.deltaTime);
        }

        public void SetTransform(TransformData data)
        {
            _moveController.SetTransform(data);
        }

        public void Play(BattleTankAnimatorState state)
        {
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
            // var instance = Instantiate(prefab, position, _muzzle.rotation);
            var forward = _muzzle.forward;
            // instance.Shot(this, forward);

            // var fireEffect = Instantiate(_fireEffectPrefab);
            // fireEffect.transform.position = position + forward;
        }

        public void SetMoveAmount(float moveAmount)
        {
            _moveController.SetMoveAmount(moveAmount);
        }

        public void SetTurnAmount(float turnAmount)
        {
            _moveController.SetTurnAmount(turnAmount);
        }

        public void Dead()
        {
            _invincibleSeq?.Kill();

            // var deadEffect = Instantiate(_deadEffectPrefab);
            // deadEffect.transform.position = transform.position;

            SetActive(false);
        }

        private void SetActive(bool active)
        {
            _collider.enabled = active;
            SetVisible(active);
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

            if (!flag)
            {
                SetVisible(true);
                return;
            }

            SetVisible(false);

            /*
            _invincibleSeq = DOTween.Sequence()
                .AppendInterval(0.05f)
                .AppendCallback(() => SetVisible(true))
                .AppendInterval(0.05f)
                .AppendCallback(() => SetVisible(false))
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject)
                .Play();
            */
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
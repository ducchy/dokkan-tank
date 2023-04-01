using System;
using ActionSequencer;
using DG.Tweening;
using GameFramework.BodySystems;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.EntitySystems;
using GameFramework.PlayableSystems;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace dtank
{
    public class BattleTankActor : Actor, IAttacker
    {
        private readonly IBattleTankActorSetupData _setupData;
        private readonly TransformData _startPointData;

        private readonly MotionController _motionController;
        private readonly LocatorParts _locatorParts;
        private readonly StatusEventListener _statusEventListener;
        private readonly DamageReceiveListener _damageReceiveListener;
        private readonly MeshRenderer[] _renderers;
        private readonly Collider _collider;

        private readonly MoveController _moveController;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly SequenceClipContainer _sequenceClipContainer;
        private readonly SequenceController _sequenceController;
        
        private SequenceHandle _actionSequenceHandle;
        private AnimatorControllerPlayableProvider _playableProvider;

        private BattleTankAnimatorState _currentState = BattleTankAnimatorState.Invalid;

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
        public BattleTankActor(Body body, IBattleTankActorSetupData setupSetupData, TransformData startPointData)
            : base(body)
        {
            _setupData = setupSetupData;
            _startPointData = startPointData;

            _motionController = body.GetController<MotionController>();
            _locatorParts = body.GetComponent<LocatorParts>();
            _statusEventListener = body.GetComponent<StatusEventListener>();
            _damageReceiveListener = body.GetComponent<DamageReceiveListener>();
            _renderers = body.GetComponentsInChildren<MeshRenderer>();
            _collider = body.GetComponent<Collider>();

            _coroutineRunner = new CoroutineRunner();
            _moveController = new MoveController(body.GetComponent<Rigidbody>(), _setupData.MoveMaxSpeed, _setupData.TurnMaxSpeed,
                pos => _onPositionChangedSubject.OnNext(pos),
                fwd => _onForwardChangedSubject.OnNext(fwd));
            _sequenceClipContainer = SequenceClipContainer.Create(setupSetupData.ActionInfos);
            _sequenceController = new SequenceController();
            
            _damageReceiveListener.SetCondition(attacker =>  attacker != this);
            _sequenceController.BindSignalEventHandler<CreateBulletSignalSequenceEvent, CreateBulletSignalSequenceEventHandler>(
                handler => { handler.Setup(this, _locatorParts["Muzzle"]); });

            SetStartPoint();
        }

        protected override void DisposeInternal()
        {
            _invincibleSeq.Kill();

            _coroutineRunner.Dispose();
            _moveController.Dispose();
            _sequenceController.ResetEventHandlers();
            _sequenceController.Dispose();

            _onStateExitSubject.Dispose();
            _onAnimationEventSubject.Dispose();
            _onDamageReceivedSubject.Dispose();
            _onPositionChangedSubject.Dispose();
            _onForwardChangedSubject.Dispose();
            _onDealDamageSubject.Dispose();
        }

        protected override void ActivateInternal(IScope scope)
        {
            _playableProvider = _motionController.Player.Change(_setupData.Controller, 0.0f, false);

            _statusEventListener.EnterSubject
                .TakeUntil(scope)
                .Subscribe(OnChangeState)
                .ScopeTo(scope);

            _damageReceiveListener.ReceiveDamageObservable
                .TakeUntil(scope)
                .Subscribe(ReceiveDamage)
                .ScopeTo(scope);
        }

        protected override void UpdateInternal()
        {
            var deltaTime = Time.deltaTime;
            _moveController.Update(deltaTime);
            _sequenceController.Update(deltaTime);
        }

        private void SetStartPoint()
        {
            _moveController.SetTransform(_startPointData);
        }

        private void SetTrigger(string triggerName) {
            _sequenceController.Stop(_actionSequenceHandle);

            var sequence = _sequenceClipContainer[triggerName];
            if (sequence != null)
                _actionSequenceHandle = _sequenceController.Play(sequence);
            
            _playableProvider.GetPlayable().SetTrigger(triggerName);
        }

        private void OnChangeState(string stateName)
        {
            foreach (BattleTankAnimatorState value in Enum.GetValues(typeof(BattleTankAnimatorState)))
            {
                if (value.ToStateName() != stateName)
                    continue;
                
                _onStateExitSubject.OnNext(_currentState);
                _currentState = value;
                break;
            }
        }

        public void Ready()
        {
            SetActive(true);
        }

        public void Damage()
        {
            if (_currentState == BattleTankAnimatorState.Damage)
                return;

            SetTrigger("onDamage");
        }

        public void ShotCurve()
        {
            if (_currentState != BattleTankAnimatorState.Idle)
                return;

            SetTrigger("onShotCurve");
        }

        public void ShotStraight()
        {
            if (_currentState != BattleTankAnimatorState.Idle)
                return;

            SetTrigger("onShotStraight");
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

            var center = _locatorParts["Center"];
            var deadEffect = Object.Instantiate(_setupData.DeadEffectPrefab);
            deadEffect.transform.position = center.position;

            SetActive(false);
        }

        private void SetActive(bool active)
        {
            _collider.enabled = active;
            SetVisible(active);
        }

        private void OnAnimationEvent(string id)
        {
            _onAnimationEventSubject.OnNext(id);
        }

        public void ReceiveDamage(IAttacker attacker)
        {
            _onDamageReceivedSubject.OnNext(attacker);
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

            _invincibleSeq = DOTween.Sequence()
                .AppendInterval(0.05f)
                .AppendCallback(() => SetVisible(true))
                .AppendInterval(0.05f)
                .AppendCallback(() => SetVisible(false))
                .SetLoops(-1, LoopType.Restart)
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
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
using UnityEngine.Playables;

namespace dtank
{
    public class BattleTankActor : Actor, IAttacker
    {
        #region Variable

        private readonly IBattleTankActorSetupData _setupData;
        private readonly TransformData _startPointData;

        private readonly MotionController _motionController;
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
        private Sequence _invincibleSeq;

        private BattleTankAnimatorState _currentState = BattleTankAnimatorState.Invalid;

        private readonly Subject<BattleTankAnimatorState> _onStateExitSubject = new();
        public IObservable<BattleTankAnimatorState> OnStateExitAsObservable => _onStateExitSubject;

        private readonly Subject<IAttacker> _onDamageReceivedSubject = new();
        public IObservable<IAttacker> OnDamageReceivedAsObservable => _onDamageReceivedSubject;

        private readonly Subject<Vector3> _onPositionChangedSubject = new();
        public IObservable<Vector3> OnPositionChangedAsObservable => _onPositionChangedSubject;

        private readonly Subject<Vector3> _onForwardChangedSubject = new();
        public IObservable<Vector3> OnForwardChangedAsObservable => _onForwardChangedSubject;

        private readonly Subject<Unit> _onDealDamageSubject = new();
        public IObservable<Unit> OnDealDamageAsObservable => _onDealDamageSubject;

        int IAttacker.Id => (int)Body.UserId;

        #endregion Variable

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleTankActor(Body body, IBattleTankActorSetupData setupData, TransformData startPointData)
            : base(body)
        {
            _setupData = setupData;
            _startPointData = startPointData;

            _motionController = body.GetController<MotionController>();
            _statusEventListener = body.GetComponent<StatusEventListener>();
            _damageReceiveListener = body.GetComponent<DamageReceiveListener>();
            _renderers = body.GetComponentsInChildren<MeshRenderer>();
            _collider = body.GetComponent<Collider>();
            var materialOverwriter = body.GetComponent<MultipleMeshMaterialOverwriter>();
            var locatorParts = body.GetComponent<LocatorParts>();

            _coroutineRunner = new CoroutineRunner();
            _moveController = new MoveController(body.GetComponent<Rigidbody>(), _setupData.MoveMaxSpeed,
                _setupData.TurnMaxSpeed,
                pos => _onPositionChangedSubject.OnNext(pos),
                fwd => _onForwardChangedSubject.OnNext(fwd));
            _sequenceClipContainer = SequenceClipContainer.Create(setupData.ActionInfos);
            _sequenceController = new SequenceController();

            materialOverwriter.Setup(setupData.Color);
            _damageReceiveListener.SetCondition(attacker => attacker != this);
            _sequenceController
                .BindSignalEventHandler<CreateBulletSignalSequenceEvent, CreateBulletSignalSequenceEventHandler>(
                    handler => { handler.Setup(this, locatorParts["Muzzle"]); });
            _sequenceController
                .BindSignalEventHandler<PlayEffectSignalSequenceEvent, PlayEffectSignalSequenceEventHandler>(
                    handler => { handler.Setup(locatorParts["Center"]); });

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
                .Subscribe(OnEnterState)
                .ScopeTo(scope);

            _statusEventListener.ExitSubject
                .TakeUntil(scope)
                .Subscribe(OnExitState)
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

        private void SetActive(bool active)
        {
            SetVisible(active);

            if (active)
                _playableProvider.GetPlayable().Play();
            else
                _playableProvider.GetPlayable().Pause();

            _collider.enabled = active;
        }

        private void SetVisible(bool flag)
        {
            foreach (var rend in _renderers)
                rend.enabled = flag;
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

        #region AnimatorState

        private void SetTrigger(string triggerName)
        {
            _sequenceController.Stop(_actionSequenceHandle);

            var sequence = _sequenceClipContainer[triggerName];
            if (sequence != null)
                _actionSequenceHandle = _sequenceController.Play(sequence);

            _playableProvider.GetPlayable().SetTrigger(triggerName);
        }

        public void Damage()
        {
            if (_currentState != BattleTankAnimatorState.Idle)
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

        public void Dead()
        {
            SetTrigger("onDead");
        }

        private void OnEnterState(string stateName)
        {
            _onStateExitSubject.OnNext(_currentState);
            _currentState = GetStateFromStateName(stateName);
        }

        private void OnExitState(string stateName)
        {
            var state = GetStateFromStateName(stateName);
            if (state == BattleTankAnimatorState.Dead)
                SetActive(false);
        }

        private BattleTankAnimatorState GetStateFromStateName(string stateName)
        {
            foreach (BattleTankAnimatorState value in Enum.GetValues(typeof(BattleTankAnimatorState)))
                if (value.ToStateName() == stateName)
                    return value;
            
            return BattleTankAnimatorState.Invalid;
        }

        #endregion AnimatorState

        #region MoveController

        private void SetStartPoint()
        {
            _moveController.SetTransform(_startPointData);
        }

        public void SetMoveAmount(float moveAmount)
        {
            _moveController.SetMoveAmount(moveAmount);
        }

        public void SetTurnAmount(float turnAmount)
        {
            _moveController.SetTurnAmount(turnAmount);
        }

        #endregion MoveController

        #region Damage

        void IAttacker.DealDamage()
        {
            _onDealDamageSubject.OnNext(Unit.Default);
        }

        private void ReceiveDamage(IAttacker attacker)
        {
            _onDamageReceivedSubject.OnNext(attacker);
        }

        #endregion Damage
    }
}
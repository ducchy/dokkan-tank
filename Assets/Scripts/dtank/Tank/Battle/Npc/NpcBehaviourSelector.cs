using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace dtank
{
    public class NpcBehaviourSelector : IBehaviourSelector
    {
        private const float ActInterval = 0.5f;
        private const float IdleDurationAfterShot = 1f;
        private const float ShotRange = 10f;
        
        private readonly Subject<Unit> _onShotCurveSubject = new();
        public IObservable<Unit> OnShotCurveAsObservable => _onShotCurveSubject;

        private readonly Subject<Unit> _onShotStraightSubject = new();
        public IObservable<Unit> OnShotStraightAsObservable => _onShotStraightSubject;

        private readonly Subject<float> _onTurnValueChangedSubject = new();
        public IObservable<float> OnTurnValueChangedAsObservable => _onTurnValueChangedSubject;

        private readonly Subject<float> _onMoveValueChangedSubject = new();
        public IObservable<float> OnMoveValueChangedAsObservable => _onMoveValueChangedSubject;

        private readonly BattleTankModel _owner;
        private readonly IReadOnlyList<BattleTankModel> _tankModels;
        private readonly NpcBehaviourObserver _behaviourObserver;

        private BattleTankModel _target;
        private NpcTankStateBase _current;
        private int _failedCount;
        private bool _activeFlag;

        public int OwnerId => _owner.Id;

        public NpcBehaviourSelector(BattleTankModel owner, IReadOnlyList<BattleTankModel> tankModels)
        {
            _owner = owner;
            _tankModels = tankModels;

            _behaviourObserver = new NpcBehaviourObserver(
                _onShotCurveSubject,
                _onShotStraightSubject,
                _onTurnValueChangedSubject,
                _onMoveValueChangedSubject);

            _failedCount = 0;
            _activeFlag = false;
            _target = FindTarget();

            SetActive(false);
        }

        public void Dispose()
        {
            _onShotCurveSubject.Dispose();
            _onShotStraightSubject.Dispose();
            _onTurnValueChangedSubject.Dispose();
            _onMoveValueChangedSubject.Dispose();
        }

        public void Reset()
        {
            _onMoveValueChangedSubject.OnNext(0f);
            _onTurnValueChangedSubject.OnNext(0f);

            _failedCount = 0;
            _target = null;
        }

        public void Update()
        {
            if (_current == null || _current.State == NpcTankState.None)
                return;

            _current.OnUpdate(Time.deltaTime);

            if (_current.Result != NpcTankStateResult.None)
                ToNextState(_current.Result);
        }

        void IBehaviourSelector.BeginDamage()
        {
            ChangeState(new NpcTankStateDamage());
        }

        void IBehaviourSelector.EndDamage()
        {
            ToNextState(NpcTankStateResult.Cancel);
        }

        void IBehaviourSelector.EndShotStraight()
        {
            _target = FindTarget();
            ChangeState(new NpcTankStateIdle(IdleDurationAfterShot));
        }

        public void SetActive(bool active)
        {
            Debug.Log($"[NpcBehaviourSelector] SetActive(): ownerId={_owner.Id} active={active}");

            if (_activeFlag == active)
                return;

            _activeFlag = active;

            if (active)
                _target = FindTarget();
            else
                Reset();
            
            ChangeState(active ? new NpcTankStateIdle(ActInterval) : new NpcTankStateNone(), force: true);
        }

        private void ToNextState(NpcTankStateResult result)
        {
            ChangeState(GetNextState(result));
        }

        private NpcTankStateBase GetNextState(NpcTankStateResult result)
        {
            if (_target == null)
                return null;

            if (_target.DeadFlag.Value)
            {
                _target = FindTarget();

                if (_target == null)
                {
                    SetActive(false);
                    return null;
                }
            }

            if (result == NpcTankStateResult.Failed)
            {
                _failedCount++;
                if (_failedCount >= 3)
                {
                    _target = FindTarget();
                    _failedCount = 0;
                    return new NpcTankStateTurn(_behaviourObserver, _owner, _target, ActInterval);
                }
            }
            else
                _failedCount = 0;

            switch (_current.State)
            {
                case NpcTankState.Turn:
                    if (result == NpcTankStateResult.Failed)
                        return new NpcTankStateTurn(_behaviourObserver, _owner, _target, ActInterval);

                    var distance = Vector3.Distance(_owner.Position, _target.Position);
                    if (distance < ShotRange)
                        return new NpcTankStateShotStraight(_behaviourObserver);

                    return new NpcTankStateMove(_behaviourObserver, _owner, ActInterval);
                case NpcTankState.Move:
                    if (result == NpcTankStateResult.Failed)
                    {
                        _target = FindTarget();
                        return new NpcTankStateTurn(_behaviourObserver, _owner, _target, ActInterval);
                    }

                    return new NpcTankStateTurn(_behaviourObserver, _owner, _target, ActInterval);
                default:
                    return new NpcTankStateTurn(_behaviourObserver, _owner, _target, ActInterval);
            }
        }

        private BattleTankModel FindTarget()
        {
            var targets = _tankModels.Where(o => o != _owner && o != _target && !o.DeadFlag.Value).ToArray();
            return targets.Length == 0
                ? (_target != null && !_target.DeadFlag.Value ? _target : null)
                : targets[Random.Range(0, targets.Length)];
        }

        private void ChangeState(NpcTankStateBase state, bool force = false)
        {
            if (state == null)
                return;

            if (!force && !_activeFlag)
                return;

            _current?.OnExit();
            _current = state;
            _current.OnEnter();
        }
    }
}
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace dtank
{
    public class NpcBehaviourSelector : IBehaviourSelector, IDisposable
    {
        private readonly Subject<IAttacker> _onDamageSubject = new Subject<IAttacker>();
        public IObservable<IAttacker> OnDamageAsObservable => _onDamageSubject;

        private readonly Subject<Unit> _onShotCurveSubject = new Subject<Unit>();
        public IObservable<Unit> OnShotCurveAsObservable => _onShotCurveSubject;

        private readonly Subject<Unit> _onShotStraightSubject = new Subject<Unit>();
        public IObservable<Unit> OnShotStraightAsObservable => _onShotStraightSubject;

        private readonly Subject<float> _onTurnValueChangedSubject = new Subject<float>();
        public IObservable<float> OnTurnValueChangedAsObservable => _onTurnValueChangedSubject;

        private readonly Subject<float> _onMoveValueChangedSubject = new Subject<float>();
        public IObservable<float> OnMoveValueChangedAsObservable => _onMoveValueChangedSubject;

        private readonly BattleTankModel _owner;
        private readonly BattleTankModel[] _others;
        private readonly NpcBehaviourObserver _behaviourObserver;

        private BattleTankModel _target;
        private NpcTankStateBase _current;
        private int _failedCount;
        private bool _activeFlag;

        public NpcBehaviourSelector(BattleTankModel owner, BattleTankModel[] others)
        {
            _owner = owner;
            _others = others;

            _behaviourObserver = new NpcBehaviourObserver(
                _onDamageSubject,
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
            _onDamageSubject.Dispose();
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
            ChangeState(new NpcTankStateIdle(2f));
        }

        public void SetActive(bool active)
        {
            Debug.LogFormat("NpcBehaviourSelector.SetActive(): active={0}", active);

            if (_activeFlag == active)
                return;

            _activeFlag = active;

            if (active)
                _target = FindTarget();
            else
                Reset();
            
            ChangeState(active ? new NpcTankStateIdle(0.5f) : new NpcTankStateNone(), force: true);
        }

        private void ToNextState(NpcTankStateResult result)
        {
            ChangeState(GetNextState(result));
        }

        private NpcTankStateBase GetNextState(NpcTankStateResult result)
        {
            if (_target == null)
                return null;

            if (_target.DeadFlag)
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
                    return new NpcTankStateTurn(_behaviourObserver, _owner, _target, 2f);
                }
            }
            else
                _failedCount = 0;

            switch (_current.State)
            {
                case NpcTankState.Turn:
                    if (result == NpcTankStateResult.Failed)
                        return new NpcTankStateTurn(_behaviourObserver, _owner, _target, 2f);

                    var distance = Vector3.Distance(_owner.Position, _target.Position);
                    if (distance < 10f)
                        return new NpcTankStateShotStraight(_behaviourObserver);

                    return new NpcTankStateMove(_behaviourObserver, _owner, 2f);
                case NpcTankState.Move:
                    if (result == NpcTankStateResult.Failed)
                    {
                        _target = FindTarget();
                        return new NpcTankStateTurn(_behaviourObserver, _owner, _target, 2f);
                    }

                    return new NpcTankStateTurn(_behaviourObserver, _owner, _target, 2f);
                default:
                    return new NpcTankStateTurn(_behaviourObserver, _owner, _target, 2f);
            }
        }

        private BattleTankModel FindTarget()
        {
            var targets = _others.Where(o => o != _target && !o.DeadFlag).ToArray();
            return targets.Length == 0
                ? (_target is { DeadFlag: false } ? _target : null)
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
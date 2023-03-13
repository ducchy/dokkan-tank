using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace dtank
{
    public class NpcBehaviourSelector : IBehaviourSelector
    {
        public Action OnDamageListener { get; set; }
        public Action OnShotCurveListener { get; set; }
        public Action OnShotStraightListener { get; set; }
        public Action<float> OnTurnValueChangedListener { get; set; }
        public Action<float> OnMoveValueChangedListener { get; set; }

        private readonly BattleTankModel _owner;
        private readonly BattleTankModel[] _others;

        private BattleTankModel _target;
        private NpcTankStateBase _current;
        private int _failedCount;

        public NpcBehaviourSelector(BattleTankModel owner, BattleTankModel[] others)
        {
            _owner = owner;
            _others = others;

            _failedCount = 0;
            _target = FindTarget();

            SetActive(false);
        }

        public void Update(float deltaTime)
        {
            if (_current == null || _current.State == NpcTankState.None)
                return;

            _current.OnUpdate(deltaTime);

            if (_current.Result != NpcTankStateResult.None)
                ToNextState(_current.Result);
        }

        private void ToNextState(NpcTankStateResult result)
        {
            ChangeState(GetNextState(result));
        }

        private NpcTankStateBase GetNextState(NpcTankStateResult result)
        {
            if (_target == null)
                return null;

            if (_target.Hp.Value <= 0)
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
                    return new NpcTankStateTurn(this, _owner, _target, 2f);
                }
            }
            else
                _failedCount = 0;

            switch (_current.State)
            {
                case NpcTankState.Turn:
                    if (result == NpcTankStateResult.Failed)
                        return new NpcTankStateTurn(this, _owner, _target, 2f);

                    var distance = Vector3.Distance(_owner.Position, _target.Position);
                    if (distance < 10f)
                        return new NpcTankStateShotStraight(this);

                    return new NpcTankStateMove(this, _owner, 2f);
                case NpcTankState.Move:
                    if (result == NpcTankStateResult.Failed) {
                        _target = FindTarget();
                        return new NpcTankStateTurn(this, _owner, _target, 2f);
                    }
                    return new NpcTankStateTurn(this, _owner, _target, 2f);
                default:
                    return new NpcTankStateTurn(this, _owner, _target, 2f);
            }
        }

        private BattleTankModel FindTarget()
        {
            var targets = _others.Where(o => o != _target && o.Hp.Value > 0).ToArray();
            return targets.Length == 0 ? (_target.Hp.Value > 0 ? _target : null) : targets[Random.Range(0, targets.Length)];
        }

        private void ChangeState(NpcTankStateBase state)
        {
            if (state == null)
                return;

            _current?.OnExit();
            _current = state;
            _current.OnEnter();
        }

        public void SetActive(bool active)
        {
            ChangeState(active ? new NpcTankStateIdle(0.5f) : new NpcTankStateNone());
        }

        public void BeginDamage()
        {
            ChangeState(new NpcTankStateDamage());
        }

        public void EndDamage()
        {
            ToNextState(NpcTankStateResult.Cancel);
        }

        public void EndShotStraight()
        {
            _target = FindTarget();
            ChangeState(new NpcTankStateIdle(2f));
        }
    }
}
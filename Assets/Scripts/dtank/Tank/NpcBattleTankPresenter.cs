using System;
using UniRx;

namespace dtank
{
    public class NpcBattleTankPresenter : IDisposable
    {
        private readonly BattleTankModel _model;
        private readonly BattleTankActor _actor;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public NpcBattleTankPresenter(
            BattleTankModel model,
            BattleTankActor actor)
        {
            _model = model;
            _actor = actor;

            Bind();
            SetEvents();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void Bind()
        {
            _model.BattleState
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case BattleTankState.Damage:
                            _actor.Play(BattleTankAnimatorState.Damage);
                            break;
                        case BattleTankState.ShotCurve:
                            _actor.Play(BattleTankAnimatorState.ShotCurve);
                            break;
                        case BattleTankState.ShotStraight:
                            _actor.Play(BattleTankAnimatorState.ShotStraight);
                            break;
                    }
                })
                .AddTo(_disposable);

            _model.Hp
                .Subscribe(hp => { }).AddTo(_disposable);
        }

        private void SetEvents()
        {
            _actor.OnStateExitListener = animState =>
            {
                switch (animState)
                {
                    case BattleTankAnimatorState.Damage:
                        _model.EndDamage();
                        if (_model.Hp.Value <= 0)
                            _actor.Dead();
                        break;
                    case BattleTankAnimatorState.ShotCurve:
                        _model.EndShotCurve();
                        break;
                    case BattleTankAnimatorState.ShotStraight:
                        _model.EndShotStraight();
                        break;
                }
            };
            _actor.OnAnimationEventListener = id =>
            {
                switch (id)
                {
                    case "ShotCurve":
                        _actor.ShotCurve();
                        break;
                    case "ShotStraight":
                        _actor.ShotStraight();
                        break;
                }
            };
            _actor.OnDamageReceivedListener = () => { _model.Damage(); };
        }
    }
}
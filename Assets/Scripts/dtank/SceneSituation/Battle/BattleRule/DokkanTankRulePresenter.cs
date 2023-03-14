using UniRx;

namespace dtank
{
    public class DokkanTankRulePresenter : BattleRulePresenterBase
    {
        private readonly BattleRuleModel _ruleModel;
        private readonly BattleTankActor[] _tankActors;
        private readonly BattleTankModel[] _tankModels;
        private readonly BattlePlayingUiView _playingUiView;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public DokkanTankRulePresenter(
            BattleRuleModel ruleModel,
            BattleTankActor[] tankActors,
            BattleTankModel[] tankModels,
            BattlePlayingUiView playingUiView)
        {
            _ruleModel = ruleModel;
            _tankActors = tankActors;
            _tankModels = tankModels;
            _playingUiView = playingUiView;

            Bind();
            SetEvent();
        }

        public override void Dispose()
        {
            _disposable.Dispose();
        }

        private void Bind()
        {
            foreach (var tankModel in _tankModels)
            {
                tankModel.BattleState.Subscribe(state =>
                {
                    if (state == BattleTankState.Dead)
                        _ruleModel.Dead(tankModel.PlayerId);
                }).AddTo(_disposable);
            }

            _ruleModel.RemainTime.Subscribe(_playingUiView.SetTime).AddTo(_disposable);
        }

        private void SetEvent()
        {
            foreach (var tankActor in _tankActors)
                tankActor.OnDealDamageListener = () => _ruleModel.IncrementScore(tankActor.OwnerId);
        }

        public override void OnChangedState(BattleState prev, BattleState current)
        {
            switch (current)
            {
                case BattleState.Ready:
                    _ruleModel.Ready();
                    break;
                case BattleState.Playing:
                    _ruleModel.Start();
                    break;
            }
        }

        public void Update(float deltaTime)
        {
            _ruleModel.Update(deltaTime);
        }
    }
}
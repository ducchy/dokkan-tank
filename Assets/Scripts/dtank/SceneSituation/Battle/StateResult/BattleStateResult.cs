using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStateResult : BattleStateBase
    {
        public override BattleState Key => BattleState.Result;

        private readonly BattleResultPresenter _presenter;

        public BattleStateResult()
        {
            var uiView = Services.Get<BattleResultUiView>();
            uiView.Construct();

            var controller = new BattleResultController();
            
            _presenter = new BattleResultPresenter(controller, uiView);
            _presenter.OnQuit = () => SceneSituationContainer?.Transition(new TitleSceneSituation());
            _presenter.OnRetry = () => StateContainer.Change(BattleState.Ready);
        }
        
        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStateResult.OnEnter()");

            _presenter.Activate(true);
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStateResult.OnExit()");
            
            _presenter.Deactivate();
        }
    }
}
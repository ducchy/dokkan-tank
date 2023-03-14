using System;

namespace dtank
{
    public class BattleResultController : IDisposable
    {
        private readonly BattleRuleModel _ruleModel;
        private readonly BattleResultUiView _uiView;
        
        public BattleResultController(
            BattleRuleModel ruleModel,
            BattleResultUiView uiView)
        {
            _ruleModel = ruleModel;
            _uiView = uiView;
        }

        public void Dispose()
        {
        }

        public void PlayResult()
        {
            _uiView.PlayResult(_ruleModel.ResultType.Value == BattleResultType.Win);
        }
    }
}
using System;
using System.Collections.Generic;

namespace dtank
{
    public class BattleResultController : IDisposable
    {
        private readonly BattleRuleModel _ruleModel;
        private readonly IReadOnlyList<BattleTankModel> _tankModels;
        private readonly BattleResultUiView _uiView;
        
        public BattleResultController(
            BattleRuleModel ruleModel,
            IReadOnlyList<BattleTankModel> tankModels,
            BattleResultUiView uiView)
        {
            _ruleModel = ruleModel;
            _tankModels = tankModels;
            _uiView = uiView;
        }

        public void Dispose()
        {
        }

        public void PlayResult()
        {
            _uiView.PlayResult(_ruleModel.ResultType.Value == BattleResultType.Win, _tankModels);
        }
    }
}
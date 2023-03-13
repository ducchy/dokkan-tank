using System;

namespace dtank
{
    public class BattleResultController : IDisposable
    {
        private readonly BattleResultData _resultData;
        private readonly BattleResultUiView _uiView;
        
        public BattleResultController(
            BattleResultData resultData,
            BattleResultUiView uiView)
        {
            _resultData = resultData;
            _uiView = uiView;
        }

        public void Dispose()
        {
        }

        public void PlayResult()
        {
            _uiView.PlayResult(_resultData.ResultType == BattleResultType.Win);
        }
    }
}
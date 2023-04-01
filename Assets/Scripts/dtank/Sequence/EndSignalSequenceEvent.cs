using System;
using ActionSequencer;

namespace dtank
{
    /// <summary>
    /// 終了用イベント
    /// </summary>
    public class EndSignalSequenceEvent : SignalSequenceEvent
    {
    }

    /// <summary>
    /// エフェクト再生用イベントのハンドラ
    /// </summary>
    public class EndSignalSequenceEventHandler : SignalSequenceEventHandler<EndSignalSequenceEvent>
    {
        private Action _endAction;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="endAction">終了処理</param>
        public void Setup(Action endAction)
        {
            _endAction = endAction;
        }

        /// <summary>
        /// イベント発火時処理
        /// </summary>
        protected override void OnInvoke(EndSignalSequenceEvent sequenceEvent)
        {
            _endAction?.Invoke();
        }
    }
}
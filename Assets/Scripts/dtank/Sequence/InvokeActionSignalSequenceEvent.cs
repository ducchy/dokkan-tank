using System;
using ActionSequencer;

namespace dtank
{
    /// <summary>
    /// エフェクト再生用イベント
    /// </summary>
    public class InvokeActionSignalSequenceEvent : SignalSequenceEvent
    {
    }

    /// <summary>
    /// アクション実行用イベントのハンドラ
    /// </summary>
    public class InvokeActionSignalSequenceEventHandler : SignalSequenceEventHandler<InvokeActionSignalSequenceEvent>
    {
        private Action _action;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="action">アクション</param>
        public void Setup(Action action)
        {
            _action = action;
        }

        /// <summary>
        /// イベント発火時処理
        /// </summary>
        protected override void OnInvoke(InvokeActionSignalSequenceEvent sequenceEvent)
        {
            _action?.Invoke();
        }
    }
}
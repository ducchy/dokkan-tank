using System;
using UniRx;
using UnityEngine;

namespace dtank {
    /// <summary>
    /// ステータスイベント監視用リスナー
    /// </summary>
    public class StatusEventListener : MonoBehaviour, IStatusEventListener {
        private readonly Subject<string> _enterSubject = new();
        private readonly Subject<Tuple<string, int>> _cycleSubject = new();
        private readonly Subject<string> _exitSubject = new();

        public IObservable<string> EnterSubject => _enterSubject;
        public IObservable<Tuple<string, int>> CycleSubject => _cycleSubject;
        public IObservable<string> ExitSubject => _exitSubject;

        /// <summary>
        /// ステータスに入った時
        /// </summary>
        void IStatusEventListener.OnStatusEnter(string statusName) {
            _enterSubject.OnNext(statusName);
        }

        /// <summary>
        /// ステータスのループ回数変化時
        /// </summary>
        void IStatusEventListener.OnStatusCycle(string statusName, int cycle) {
            _cycleSubject.OnNext(new Tuple<string, int>(statusName, cycle));
        }

        /// <summary>
        /// ステータスを抜けた時
        /// </summary>
        void IStatusEventListener.OnStatusExit(string statusName) {
            _exitSubject.OnNext(statusName);
        }

        /// <summary>
        /// 廃棄時処理
        /// </summary>
        private void OnDestroy() {
            _enterSubject.OnCompleted();
            _cycleSubject.OnCompleted();
            _exitSubject.OnCompleted();
        }
    }
}
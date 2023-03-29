using System.Collections;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.SituationSystems;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// フェードを使った遷移エフェクト
    /// </summary>
    public class FadeTransitionEffect : ITransitionEffect
    {
        private Color _color;
        private float _enterDuration;
        private float _exitDuration;
        private DisposableScope _scope;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FadeTransitionEffect(Color color, float enterDuration, float exitDuration)
        {
            _color = color;
            _enterDuration = enterDuration;
            _exitDuration = exitDuration;
        }

        /// <summary>
        /// 演出開始
        /// </summary>
        IEnumerator ITransitionEffect.EnterRoutine()
        {
            _scope = new DisposableScope();
            yield return Services.Get<FadeController>()
                .FadeOutAsync(_color, _enterDuration)
                .StartAsEnumerator(_scope);
        }

        /// <summary>
        /// 演出中
        /// </summary>
        void ITransitionEffect.Update()
        {
        }

        /// <summary>
        /// 演出終了
        /// </summary>
        IEnumerator ITransitionEffect.ExitRoutine()
        {
            _scope.Dispose();
            yield return Services.Get<FadeController>()
                .FadeInAsync(_exitDuration)
                .StartAsEnumerator(_scope);
        }
    }
}
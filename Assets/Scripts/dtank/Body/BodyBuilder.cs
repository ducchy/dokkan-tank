using GameFramework.BodySystems;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// Body生成クラス
    /// </summary>
    public class BodyBuilder : IBodyBuilder
    {
        /// <summary>
        /// 構築処理
        /// </summary>
        public void Build(IBody body, GameObject gameObject)
        {
        }

        private void RequireComponent<T>(GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                gameObject.AddComponent<T>();
            }
        }
    }
}
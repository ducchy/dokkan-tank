using ActionSequencer;
using UnityEngine;


namespace dtank
{
    /// <summary>
    /// エフェクト再生用イベント
    /// </summary>
    public class PlayEffectSignalSequenceEvent : SignalSequenceEvent
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Vector3 _offsetPosition;
        [SerializeField] private Vector3 _offsetAngles;

        public GameObject Prefab => _prefab;
        public Vector3 OffsetPosition => _offsetPosition;
        public Vector3 OffsetAngles => _offsetAngles;
    }

    /// <summary>
    /// エフェクト再生用イベントのハンドラ
    /// </summary>
    public class PlayEffectSignalSequenceEventHandler : SignalSequenceEventHandler<PlayEffectSignalSequenceEvent>
    {
        private Transform _origin;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="origin">原点</param>
        public void Setup(Transform origin)
        {
            _origin = origin;
        }

        /// <summary>
        /// イベント発火時処理
        /// </summary>
        protected override void OnInvoke(PlayEffectSignalSequenceEvent sequenceEvent)
        {
            if (_origin == null)
                return;

            if (sequenceEvent.Prefab == null)
                return;

            var position = _origin.TransformPoint(sequenceEvent.OffsetPosition);
            var rotation = Quaternion.Euler(sequenceEvent.OffsetAngles) * _origin.rotation;
            var instance = Object.Instantiate(sequenceEvent.Prefab);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            var particleSystem = instance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                // ※削除はPrefabの指定任せ
                particleSystem.Play();
            }
        }
    }
}
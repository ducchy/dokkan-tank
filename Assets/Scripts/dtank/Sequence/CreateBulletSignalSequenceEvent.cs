using ActionSequencer;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// 弾生成用イベント
    /// </summary>
    public class CreateBulletSignalSequenceEvent : SignalSequenceEvent
    {
        [SerializeField] private Bullet _prefab;
        [SerializeField] private Vector3 _offsetPosition;
        [SerializeField] private Vector3 _offsetAngles;
        [SerializeField] private float _speed;
        [SerializeField] private bool _useGravity;
        [SerializeField] private ParticleSystem _fireEffect;
        [SerializeField] private ParticleSystem _explosionEffect;

        public Bullet Prefab => _prefab;
        public Vector3 OffsetPosition => _offsetPosition;
        public Vector3 OffsetAngles => _offsetAngles;
        public float Speed => _speed;
        public bool UseGravity => _useGravity;
        public ParticleSystem FireEffect => _fireEffect;
        public ParticleSystem ExplosionEffect => _explosionEffect;
    }

    /// <summary>
    /// 弾生成用イベントのハンドラ
    /// </summary>
    public class CreateBulletSignalSequenceEventHandler : SignalSequenceEventHandler<CreateBulletSignalSequenceEvent>
    {
        private IAttacker _owner;
        private Transform _muzzle;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="owner">所有者</param>
        /// <param name="muzzle">マズル</param>
        public void Setup(IAttacker owner, Transform muzzle)
        {
            _owner = owner;
            _muzzle = muzzle;
        }

        /// <summary>
        /// イベント発火時処理
        /// </summary>
        protected override void OnInvoke(CreateBulletSignalSequenceEvent sequenceEvent)
        {
            if (_muzzle == null)
                return;

            if (sequenceEvent.Prefab == null)
                return;

            var instance = Object.Instantiate(sequenceEvent.Prefab);
            var fireEffectPrefab = sequenceEvent.FireEffect;
            var explosionEffectPrefab = sequenceEvent.ExplosionEffect;
            var useGravity = sequenceEvent.UseGravity;
            instance.Setup(_owner, fireEffectPrefab, explosionEffectPrefab, useGravity);

            var position = _muzzle.TransformPoint(sequenceEvent.OffsetPosition);
            var rotation = Quaternion.Euler(sequenceEvent.OffsetAngles) * _muzzle.rotation;
            var velocity = _muzzle.forward * sequenceEvent.Speed;
            instance.Shot(position, rotation, velocity);
        }
    }
}
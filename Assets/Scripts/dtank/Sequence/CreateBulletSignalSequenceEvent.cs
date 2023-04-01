using ActionSequencer;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// エフェクト再生用イベント
    /// </summary>
    public class CreateBulletSignalSequenceEvent : SignalSequenceEvent
    {
        public Bullet prefab;
        public Vector3 offsetPosition;
        public Vector3 offsetAngles;
        public float speed;
        public bool useGravity;
        public ParticleSystem fireEffect;
        public ParticleSystem explosionEffect;
    }
    
    /// <summary>
    /// エフェクト再生用イベントのハンドラ
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
            {
                return;
            }

            if (sequenceEvent.prefab == null)
            {
                return;
            }

            var instance = Object.Instantiate(sequenceEvent.prefab);
            var fireEffectPrefab = sequenceEvent.fireEffect;
            var explosionEffectPrefab = sequenceEvent.explosionEffect;
            var useGravity = sequenceEvent.useGravity;
            instance.Setup(_owner, fireEffectPrefab, explosionEffectPrefab, useGravity);
            
            var position = _muzzle.TransformPoint(sequenceEvent.offsetPosition);
            var rotation = Quaternion.Euler(sequenceEvent.offsetAngles) * _muzzle.rotation;
            var velocity = _muzzle.forward * sequenceEvent.speed;
            instance.Shot(position,rotation, velocity);
        }
    }
}
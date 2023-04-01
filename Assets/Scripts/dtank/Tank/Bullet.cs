using Unity.Mathematics;
using UnityEngine;

namespace dtank
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private IAttacker _owner;
        private ParticleSystem _fireEffectPrefab;
        private ParticleSystem _explosionEffectPrefab;


        public void Setup(IAttacker owner, ParticleSystem fireEffectPrefab, ParticleSystem explosionEffectPrefab,
            bool useGravity)
        {
            _owner = owner;
            _fireEffectPrefab = fireEffectPrefab;
            _explosionEffectPrefab = explosionEffectPrefab;

            _rigidbody.useGravity = useGravity;
        }

        public void Shot(Vector3 position, quaternion rotation, Vector3 velocity)
        {
            _rigidbody.position = position;
            _rigidbody.rotation = rotation;
            _rigidbody.velocity = velocity;

            var fire = Instantiate(_fireEffectPrefab);
            fire.transform.position = position;
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.LogFormat("OnCollisionEnter: name={0}", other.gameObject.name);

            var damageReceiver = other.gameObject.GetComponent<IDamageReceiver>();
            if (damageReceiver != null)
            {
                if (!damageReceiver.ReceiveDamage(_owner))
                    return;

                Debug.LogFormat("ダメージ！");
            }

            Explode();
        }

        private void Explode()
        {
            Debug.LogFormat("Explode");

            var explosion = Instantiate(_explosionEffectPrefab);
            explosion.transform.position = transform.position;

            Destroy(gameObject);
        }
    }
}
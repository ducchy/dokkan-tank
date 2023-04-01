using Unity.Mathematics;
using UnityEngine;

namespace dtank
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private static int _counter;
        
        private IAttacker _owner;
        private ParticleSystem _fireEffectPrefab;
        private ParticleSystem _explosionEffectPrefab;
        private int _id;

        public void Setup(IAttacker owner, ParticleSystem fireEffectPrefab, ParticleSystem explosionEffectPrefab,
            bool useGravity)
        {
            _owner = owner;
            _fireEffectPrefab = fireEffectPrefab;
            _explosionEffectPrefab = explosionEffectPrefab;

            _id = ++_counter;

            _rigidbody.useGravity = useGravity;
        }

        public void Shot(Vector3 position, quaternion rotation, Vector3 velocity)
        {
            Debug.Log($"[Bullet] Shot: id={_id}, ownerId={_owner.Id}");

            _rigidbody.position = position;
            _rigidbody.rotation = rotation;
            _rigidbody.velocity = velocity;

            var fire = Instantiate(_fireEffectPrefab);
            fire.transform.position = position;
        }

        private void OnCollisionEnter(Collision other)
        {
            var damageReceiver = other.gameObject.GetComponent<IDamageReceiver>();
            if (damageReceiver != null)
            {
                DealDamage(damageReceiver);
                return;
            }

            Explode();
        }

        private void DealDamage(IDamageReceiver damageReceiver)
        {
            if (!damageReceiver.ReceiveDamage(_owner))
                return;

            Debug.Log($"[Bullet] DealDamage: id={_id}, ownerId={_owner.Id}, damageReceiver={damageReceiver.Name}");

            Explode();
        }

        private void Explode()
        {
            Debug.Log($"[Bullet] Shot: id={_id}, ownerId={_owner.Id}");
            
            var explosion = Instantiate(_explosionEffectPrefab);
            explosion.transform.position = transform.position;

            Destroy(gameObject);
        }
    }
}
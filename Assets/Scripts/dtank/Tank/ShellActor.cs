using UnityEngine;

namespace dtank
{
    public class ShellActor : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private float _velocity;
        [SerializeField] private GameObject _explosionEffectPrefab;

        private IAttacker _owner;

        public void Shot(IAttacker owner, Vector3 forward)
        {
            _owner = owner;

            _rigidbody.velocity = forward * _velocity;
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
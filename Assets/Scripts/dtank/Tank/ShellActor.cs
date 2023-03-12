using System;
using UnityEngine;

namespace dtank
{
    public class ShellActor : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _velocity;

        public void Shot(Vector3 forward)
        {
            _rigidbody.velocity = forward * _velocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.LogFormat("OnCollisionEnter: name={0}", other.gameObject.name);

            Explode();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogFormat("OnTriggerEnter: name={0}", other.gameObject.name);

            var tank = other.gameObject.GetComponent<BattleTankActor>();
            if (tank != null)
            {
                Debug.LogFormat("ヒット！");
            }
            
            Explode();
        }

        private void Explode()
        {
            Debug.LogFormat("Explode");

            Destroy(gameObject);
        }
    }
}
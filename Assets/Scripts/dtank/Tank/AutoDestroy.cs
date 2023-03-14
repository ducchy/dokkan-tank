using DG.Tweening;
using UnityEngine;

namespace dtank
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float _lifetime;

        private void Awake()
        {
            DOVirtual.DelayedCall(_lifetime, () => Destroy(gameObject)).SetLink(gameObject).Play();
        }
    }
}
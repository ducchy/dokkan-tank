using UnityEngine;

namespace dtank
{
    public class FollowTargetCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 _offsetPos = new Vector3(0f, 1.5f, -3.5f);
        [SerializeField] private Vector3 _offsetAngle = new Vector3(15f, 0f, 0f);

        private Transform _transform;
        private Transform _target;

        public void Construct(Transform target)
        {
            _target = target;

            _transform = transform;
            _transform.SetParent(target);
            SetOffsetPos(_offsetPos);
            SetOffsetAngle(_offsetAngle);
        }

        public void OnUpdate(float deltaTime)
        {
        }

        private void SetOffsetPos(Vector3 offset)
        {
            _transform.localPosition = offset;
        }

        private void SetOffsetAngle(Vector3 offset)
        {
            _transform.localEulerAngles = offset;
        }
    }
}
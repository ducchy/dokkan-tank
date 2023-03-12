using UnityEngine;

namespace dtank {
    public class FollowTargetCamera : MonoBehaviour
    {
        private readonly Vector3 _offsetPos = new Vector3(0f, 1.5f, -3.5f);
        private readonly Vector3 _offsetAngle = new Vector3(15f, 0f, 0f);
        
        private Transform _transform;
        private Transform _target;
        
        public void Construct(Transform target)
        {
            _target = target;
            
            _transform = transform;
        }

        public void Update()
        {
            if (_target == null)
                return;
            
            var targetPos = _target.position;
            var targetAngle = _target.eulerAngles;
            
            _transform.position = targetPos + _transform.rotation * _offsetPos;
            _transform.eulerAngles = targetAngle + _offsetAngle;
        }
    }
}
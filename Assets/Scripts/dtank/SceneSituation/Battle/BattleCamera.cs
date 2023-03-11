using UnityEngine;

namespace dtank {
    public class BattleCamera : MonoBehaviour
    {
        public void Construct()
        {
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetEulerAngle(Vector3 angles)
        {
            transform.eulerAngles = angles;
        }
    }
}
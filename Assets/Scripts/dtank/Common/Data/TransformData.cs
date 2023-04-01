using UnityEngine;

namespace dtank
{
    public class TransformData
    {
        public readonly Vector3 Position;
        public readonly Vector3 Angle;
        public readonly Vector3 LocalScale;

        public TransformData(Transform t)
        {
            Position = t.position;
            Angle = t.eulerAngles;
            LocalScale = t.localScale;
        }

        public override string ToString()
        {
            return $"Position={Position}, Angle={Angle}, LocalScale={LocalScale}";
        }
    }
}
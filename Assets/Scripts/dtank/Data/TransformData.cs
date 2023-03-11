using UnityEngine;

namespace dtank
{
    public class TransformData
    {
        public Vector3 Position { get; set; }
        public Vector3 Angle { get; set; }
        public Vector3 LocalScale { get; set; }

        public TransformData()
        {
            Position = Vector3.zero;
            Angle = Vector3.zero;
            LocalScale = Vector3.one;
        }

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
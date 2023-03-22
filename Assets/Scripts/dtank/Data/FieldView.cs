using System.Linq;
using UnityEngine;

namespace dtank
{
    public class FieldViewData
    {
        public readonly TransformData[] StartPointDataArray;
        public readonly TransformData[] GimmickPointArray;

        public FieldViewData(TransformData[] startPointDataArray, TransformData[] gimmickPointArray)
        {
            StartPointDataArray = startPointDataArray;
            GimmickPointArray = gimmickPointArray;
        }
    }
    
    public class FieldView : MonoBehaviour
    {
        [SerializeField] private Transform[] _startPointArray;
        [SerializeField] private Transform[] _gimmickPointArray;

        public FieldViewData CreateData()
        {
            var startPointDataArray = _startPointArray.Select(t => new TransformData(t)).ToArray();
            var gimmickPointDataArray = _gimmickPointArray.Select(t => new TransformData(t)).ToArray();
            return new FieldViewData(startPointDataArray, gimmickPointDataArray);
        }
    }
}
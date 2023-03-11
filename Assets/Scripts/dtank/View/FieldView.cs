using System.Linq;
using UnityEngine;

namespace dtank
{
    public class FieldView : MonoBehaviour
    {
        [SerializeField] private Transform[] _startPointArray;
        [SerializeField] private Transform[] _gimmickPointArray;

        public TransformData[] StartPointDataArray
        {
            get
            {
                if (_startPointDataArray == null && _startPointArray.Length > 0)
                    _startPointDataArray = _startPointArray.Select(t => new TransformData(t)).ToArray();
                return _startPointDataArray;
            }
        }
        private TransformData[] _startPointDataArray = null;
        
        public TransformData[] GimmickPointDataArray
        {
            get
            {
                if (_gimmickPointDataArray == null && _gimmickPointArray.Length > 0)
                    _gimmickPointDataArray = _gimmickPointArray.Select(t => new TransformData(t)).ToArray();
                return _gimmickPointDataArray;
            }
        }
        private TransformData[] _gimmickPointDataArray = null;
    }
}
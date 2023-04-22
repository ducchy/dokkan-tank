using System.Linq;
using UnityEngine;

namespace dtank
{   
    public class FieldView : MonoBehaviour
    {
        [SerializeField] private Transform[] _startPointArray;
        [SerializeField] private Transform[] _gimmickPointArray;

        public FieldData CreateData()
        {
            var startPointDataArray = _startPointArray.Select(t => new TransformData(t)).ToArray();
            var gimmickPointDataArray = _gimmickPointArray.Select(t => new TransformData(t)).ToArray();
            return new FieldData(startPointDataArray, gimmickPointDataArray);
        }
    }
}
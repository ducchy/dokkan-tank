namespace dtank
{
    public class FieldData
    {
        public readonly TransformData[] StartPointDataArray;
        public readonly TransformData[] GimmickPointArray;

        public FieldData(TransformData[] startPointDataArray, TransformData[] gimmickPointArray)
        {
            StartPointDataArray = startPointDataArray;
            GimmickPointArray = gimmickPointArray;
        }
    }
}
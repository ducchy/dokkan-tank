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
}
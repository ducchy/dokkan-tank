namespace dtank
{
    public class FieldScene : AdditiveSceneBase
    {
        private readonly int _fieldId;
        
        protected override string SceneAssetPath => $"field{_fieldId:d3}";

        public FieldScene(int fieldId)
        {
            _fieldId = fieldId;
        }
    }
}

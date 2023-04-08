#if DEVELOPMENT_BUILD || UNITY_EDITOR

namespace dtank
{
    public class DebugManager
    {
        private readonly DebugSheetController _debugSheetController;
        
        public DebugManager()
        {
            _debugSheetController = new DebugSheetController();
        }
    }
}
#endif // DEVELOPMENT_BUILD || UNITY_EDITOR
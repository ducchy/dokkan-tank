using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace dtank
{
    public class DebugSheetController
    {
        private const string Path = "Debug/DebugSheetCanvas";

        private readonly DebugSheet _debugSheet;
        private readonly DebugPage _rootPage;
        private readonly DtankDebugPage _dtankDebugPage;

        public DebugSheetController()
        {
            DebugSheet CreateDebugSheet()
            {
                var prefab = Resources.Load<DebugSheet>(Path);
                if (prefab != null)
                    return Object.Instantiate(prefab);

                Debug.LogError($"[DebugSheetController] CreateDebugSheet: prefab取得失敗(path={Path})");
                return null;
            }

            DebugPage CreateRootPage() => DebugSheet.Instance.GetOrCreateInitialPage();

            var dtankDebugPage = default(DtankDebugPage);
            void OnLoadDtankDebugPage((string, DtankDebugPage) tuple) => dtankDebugPage = tuple.Item2;

            _debugSheet = CreateDebugSheet();
            _rootPage = CreateRootPage();
            _rootPage.AddPageLinkButton<DtankDebugPage>(nameof(DtankDebugPage), onLoad: OnLoadDtankDebugPage);
            _dtankDebugPage = dtankDebugPage;
        }
    }
}
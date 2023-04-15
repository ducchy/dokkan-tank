#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace dtank
{
    public class DtankDebugSheet : IDisposable
    {
        private const string Path = "Debug/DebugSheetCanvas";

        public DtankDebugSheet()
        {
            void CreateDebugSheet()
            {
                var prefab = Resources.Load<DebugSheet>(Path);
                if (prefab != null)
                {
                    Object.Instantiate(prefab);
                    return;
                }

                Debug.LogError($"[DebugSheetController] CreateDebugSheet: prefab取得失敗(path={Path})");
            }

            DebugPage CreateRootPage() => DebugSheet.Instance.GetOrCreateInitialPage();

            CreateDebugSheet();

            var rootPage = CreateRootPage();
            rootPage.AddPageLinkButton<DtankDebugPage>("dtank");
            rootPage.AddPageLinkButton<SystemInfoDebugPage>("System Info");
            rootPage.AddPageLinkButton<ApplicationDebugPage>("Application");
            rootPage.AddPageLinkButton<TimeDebugPage>("Time");
            rootPage.AddPageLinkButton<QualitySettingsDebugPage>("Quality Setting");
            rootPage.AddPageLinkButton<ScreenDebugPage>("Screen");
            rootPage.AddPageLinkButton<InputDebugPage>("Input");
            rootPage.AddPageLinkButton<GraphicsDebugPage>("Graphics");
            rootPage.AddPageLinkButton<PhysicsDebugPage>("Physics");

            DebugSheet.Instance.PushPage<DtankDebugPage>(false);
        }

        public void Dispose()
        {
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR
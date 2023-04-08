#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System;
using UnityDebugSheet.Runtime.Core.Scripts;
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

            void OnLoadDtankPage((string, DtankDebugPage) tuple)
            {
            }

            CreateDebugSheet();

            var rootPage = CreateRootPage();
            rootPage.AddPageLinkButton<DtankDebugPage>("dtank Debug", onLoad: OnLoadDtankPage);
        }

        public void Dispose()
        {
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR
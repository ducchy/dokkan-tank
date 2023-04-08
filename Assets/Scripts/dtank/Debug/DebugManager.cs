#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System;
using GameFramework.Core;

namespace dtank
{
    public class DebugManager : IDisposable
    {
        private readonly DisposableScope _scope = new();

        private static readonly Services Services = new();
        public static IServiceContainer ServiceContainer => Services;

        public DebugManager()
        {
            var dtankDebugSheet = new DtankDebugSheet();
            dtankDebugSheet.ScopeTo(_scope);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
#endif // DEVELOPMENT_BUILD || UNITY_EDITOR
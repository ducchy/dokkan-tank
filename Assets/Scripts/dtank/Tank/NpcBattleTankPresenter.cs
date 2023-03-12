using System;
using UniRx;

namespace dtank
{
    public class NpcBattleTankPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public NpcBattleTankPresenter(
            BattleTankModel model,
            BattleTankActor actor)
        {
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
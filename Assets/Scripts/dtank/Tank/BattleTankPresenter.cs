using System;
using UniRx;

namespace dtank
{
    public class BattleTankPresenter : IDisposable
    {
        private readonly BattleTankModel[] _models;
        private readonly BattleTankActor[] _actors;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public BattleTankPresenter(
            BattleTankModel[] models,
            BattleTankActor[] actors)
        {
            _models = models;
            _actors = actors;

            for (var i = 0; i < models.Length; i++)
            {
                var model = models[i];
                var actor = actors[i];
                model.TransformData
                    .Subscribe(data => actor.SetTransform(data))
                    .AddTo(_disposable);
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
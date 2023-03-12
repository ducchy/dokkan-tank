using System;
using UniRx;

namespace dtank
{
    public class BattleTankPresenter : IDisposable
    {
        private readonly BattleTankModel[] _models;
        private readonly BattleTankView[] _views;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public BattleTankPresenter(
            BattleTankModel[] models,
            BattleTankView[] views)
        {
            _models = models;
            _views = views;

            for (var i = 0; i < models.Length; i++)
            {
                var model = models[i];
                var view = views[i];
                model.TransformData
                    .Subscribe(data => view.SetTransform(data))
                    .AddTo(_disposable);
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
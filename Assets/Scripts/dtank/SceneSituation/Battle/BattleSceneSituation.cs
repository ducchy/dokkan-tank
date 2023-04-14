using System.Collections;
using System.Collections.Generic;
using GameFramework.BodySystems;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleSceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "Battle";

        private StateContainer<BattleStateBase, BattleState> _stateContainer;

        private readonly BattleEntryData _battleEntryData;

        public BattleSceneSituation()
        {
            _battleEntryData = BattleEntryData.CreateDefaultData();
        }

        public BattleSceneSituation(BattleEntryData battleEntryData)
        {
            _battleEntryData = battleEntryData;
        }

        protected override IEnumerator SetupRoutineInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("[BattleSceneSituation] Begin SetupRoutineInternal");

            yield return SetupAllRoutine(scope);

            Bind(scope);

            BattleModel.Get().ChangeState(BattleState.Ready);

            Debug.Log("[BattleSceneSituation] End SetupRoutineInternal");
        }

        protected override void UpdateInternal()
        {
            _stateContainer.Update(Time.deltaTime);
        }

        protected override void UnloadInternal(TransitionHandle handle)
        {
            Debug.Log("[BattleSceneSituation] UnloadInternal");

            UnloadAll();
        }

        private void Bind(IScope scope)
        {
            var model = BattleModel.Get();
            model.CurrentState
                .TakeUntil(scope)
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case BattleState.Quit:
                            ParentContainer.Transition(new TitleSceneSituation(),
                                new CommonFadeTransitionEffect(0.5f, 0.5f));
                            break;
                        case BattleState.Retry:
                            ParentContainer.Transition(new BattleReadySceneSituation(_battleEntryData),
                                new CommonFadeTransitionEffect(0.5f, 0f));
                            break;
                        default:
                            _stateContainer.Change(state);
                            break;
                    }
                })
                .ScopeTo(scope);
        }

        private void RegisterTask(ITask task, TaskOrder order)
        {
            var taskRunner = Services.Get<TaskRunner>();
            taskRunner.Register(task, order);
        }

        #region Setup

        private IEnumerator SetupAllRoutine(IScope scope)
        {
            yield return SetupManagerRoutine();
            yield return SetupModelRoutine(scope);
            yield return SetupPresenterRoutine(scope);
            SetupStateContainer(scope);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var model = BattleModel.Get();
            DebugManager.BattleDebugModel.Setup(model);
            DebugManager.BattleDebugModel.ScopeTo(scope);
#endif
        }

        private IEnumerator SetupManagerRoutine()
        {
            var bodyManager = new BodyManager(new BodyBuilder());
            ServiceContainer.Set(bodyManager);
            RegisterTask(bodyManager, TaskOrder.Body);

            var fieldManager = Services.Get<FieldManager>();
            yield return fieldManager.LoadRoutine(1);
        }

        private IEnumerator SetupModelRoutine(IScope scope)
        {
            var fieldViewData = Services.Get<FieldViewData>();

            var battleModel = BattleModel.Create();
            RegisterTask(battleModel, TaskOrder.Logic);
            yield return battleModel.SetupAsync(_battleEntryData, fieldViewData)
                .StartAsEnumerator(scope);
            battleModel.ScopeTo(scope);
        }

        private IEnumerator SetupPresenterRoutine(IScope scope)
        {
            var model = BattleModel.Get();

            var uiView = Services.Get<BattleUiView>();
            uiView.Setup();
            uiView.ScopeTo(scope);

            uiView.PlayerStatusUiView.Setup(model);

            var tankEntityContainer =
                new BattleTankEntityContainer(uiView.TankControlUiView, uiView.PlayerStatusUiView);
            tankEntityContainer.ScopeTo(scope);
            yield return tankEntityContainer.SetupRoutine(model.TankModels, model.NpcBehaviourSelectors, scope);

            var cameraController = Services.Get<BattleCameraController>();
            cameraController.Setup(model, tankEntityContainer);
            cameraController.ScopeTo(scope);

            var presenter = new BattlePresenter(model, uiView, cameraController);
            presenter.ScopeTo(scope);
        }

        private void SetupStateContainer(IScope scope)
        {
            _stateContainer = new StateContainer<BattleStateBase, BattleState>();
            _stateContainer.ScopeTo(scope);

            var states = new List<BattleStateBase>()
            {
                new BattleStateReady(),
                new BattleStatePlaying(),
                new BattleStateResult(),
            };
            _stateContainer.Setup(BattleState.Invalid, states.ToArray());
        }

        #endregion Load

        #region Unload

        private void UnloadAll()
        {
            var fieldManager = Services.Get<FieldManager>();
            fieldManager.Unload();

            BattleModel.Delete();
        }

        #endregion Unload
    }
}
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

        private readonly List<ITask> _tasks = new();
        
        private StateContainer<BattleStateBase, BattleState> _stateContainer;

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
                            ParentContainer.Transition(new TitleSceneSituation(), new CommonFadeTransitionEffect(0.5f, 0.5f));
                            break;
                        case BattleState.Retry:
                            ParentContainer.Transition(new BattleReadySceneSituation(), new CommonFadeTransitionEffect(0.5f, 0f));
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
            _tasks.Add(task);
        }

        #region Setup

        private IEnumerator SetupAllRoutine(IScope scope)
        {
            yield return SetupManagerRoutine();
            SetupBattleEntryData();
            yield return SetupModelRoutine(scope);
            yield return SetupPresenterRoutine(scope);
            SetupStateContainer(scope);
        }

        private IEnumerator SetupManagerRoutine()
        {
            var bodyManager = new BodyManager(new BodyBuilder());
            ServiceContainer.Set(bodyManager);
            RegisterTask(bodyManager, TaskOrder.Body);

            var fieldManager = Services.Get<FieldManager>();
            yield return fieldManager.LoadRoutine(1);
        }

        private void SetupBattleEntryData()
        {
            // TODO: 選んだルールに応じて設定
            var mainPlayerData = new BattlePlayerEntryData(1, "プレイヤー1", 1, 0, CharacterType.Player, 1);
            var battleEntryData = Services.Get<BattleEntryData>();
            battleEntryData.Set(1, mainPlayerData, new List<BattlePlayerEntryData>()
            {
                mainPlayerData,
                new(2, "プレイヤー2", 1, 1, CharacterType.NonPlayer, 2),
                new(3, "プレイヤー3", 1, 2, CharacterType.NonPlayer, 3),
                new(4, "プレイヤー4", 1, 3, CharacterType.NonPlayer, 4),
            });
        }

        private IEnumerator SetupModelRoutine(IScope scope)
        {
            var battleEntryData = Services.Get<BattleEntryData>();
            var fieldViewData = Services.Get<FieldViewData>();

            var battleModel = BattleModel.Create();
            RegisterTask(battleModel, TaskOrder.Logic);
            yield return battleModel.SetupAsync(battleEntryData, fieldViewData)
                .StartAsEnumerator(scope);
            battleModel.ScopeTo(scope);
        }

        private IEnumerator SetupPresenterRoutine(IScope scope)
        {
            var model = BattleModel.Get();

            var fadeController = Services.Get<FadeController>();
            var uiView = Services.Get<BattleUiView>();
            uiView.Setup(fadeController);
            uiView.ScopeTo(scope);
            
            uiView.PlayerStatusUiView.Setup(model);

            var tankEntityContainer = new BattleTankEntityContainer(uiView.TankControlUiView, uiView.PlayerStatusUiView);
            tankEntityContainer.ScopeTo(scope);
            yield return tankEntityContainer.SetupRoutine(model.TankModels, scope);

            var cameraController = Services.Get<BattleCameraController>();
            cameraController.Setup(model, tankEntityContainer);
            cameraController.ScopeTo(scope);

            var presenter = new BattlePresenter(model, uiView, tankEntityContainer, cameraController);
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

            var taskRunner = Services.Get<TaskRunner>();
            foreach (var task in _tasks)
                taskRunner.Unregister(task);

            BattleModel.Delete();
        }

        #endregion Unload
    }
}
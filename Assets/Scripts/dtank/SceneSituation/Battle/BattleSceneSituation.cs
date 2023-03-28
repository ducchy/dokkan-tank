using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.BodySystems;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.EntitySystems;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleSceneSituation : SceneSituation
    {
        /// <summary>
        /// Body生成クラス
        /// </summary>
        private class BodyBuilder : IBodyBuilder
        {
            /// <summary>
            /// 構築処理
            /// </summary>
            public void Build(IBody body, GameObject gameObject)
            {
                // RequireComponent<StatusEventListener>(gameObject);
            }

            private void RequireComponent<T>(GameObject gameObject)
                where T : Component
            {
                var component = gameObject.GetComponent<T>();
                if (component == null)
                {
                    gameObject.AddComponent<T>();
                }
            }
        }

        protected override string SceneAssetPath => "Battle";

        private StateContainer<BattleStateBase, BattleState> _stateContainer;
        private BattlePresenter _presenter;
        private readonly List<ITask> _tasks = new List<ITask>();

        protected override IEnumerator SetupRoutineInternal(TransitionHandle handle, IScope scope)
        {
            yield return base.SetupRoutineInternal(handle, scope);

            yield return SetupAllRoutine(scope);

            Bind(scope);

            BattleModel.Get().ChangeState(BattleState.Ready);
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();

            _stateContainer.Update(Time.deltaTime);
        }

        protected override void UnloadInternal(TransitionHandle handle)
        {
            base.UnloadInternal(handle);

            UnloadAll();
        }

        #region Load

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
            var mainPlayerData = new BattlePlayerEntryData(1, "一郎", 1, 0, CharacterType.Player, 1);
            var battleEntryData = Services.Get<BattleEntryData>();
            battleEntryData.Set(1, mainPlayerData, new List<BattlePlayerEntryData>()
            {
                mainPlayerData,
                new(2, "二郎", 2, 1, CharacterType.NonPlayer, 1),
                new(3, "三郎", 3, 2, CharacterType.NonPlayer, 1),
                new(4, "四郎", 4, 3, CharacterType.NonPlayer, 1),
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
            RegisterTask(uiView, TaskOrder.UI);

            var tankEntityContainer = new BattleTankEntityContainer();
            tankEntityContainer.ScopeTo(scope);
            foreach (var tankModel in model.TankModels)
                yield return tankEntityContainer.AddRoutine(tankModel, scope);
            foreach (var tankModel in model.TankModels)
            {
                var entity = tankEntityContainer.Get(tankModel.Id);
                var logic = entity.GetLogic<EntityLogic>() as BattleTankLogic;
                if (logic == null)
                {
                    Debug.LogError($"logic取得失敗; id={tankModel.Id}");
                    continue;
                }
                
                if (tankModel.Id == model.MainPlayerTankModel.Id)
                    logic.SetBehaviourSelector(uiView.TankControlUiView);
                else 
                    logic.SetBehaviourSelector(new NpcBehaviourSelector(tankModel, model.TankModels.Where(m => m != tankModel).ToArray()));
            }

            var cameraController = Services.Get<BattleCameraController>();
            cameraController.Setup(model, tankEntityContainer);
            cameraController.ScopeTo(scope);

            _presenter = new BattlePresenter(model, uiView, tankEntityContainer, cameraController);
            _presenter.ScopeTo(scope);
        }

        private void SetupStateContainer(IScope scope)
        {
            Debug.Log("BattleSceneSituation.SetupStateContainer");

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

        private void Bind(IScope scope)
        {
            var battleModel = BattleModel.Get();
            battleModel.CurrentState
                .TakeUntil(scope)
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case BattleState.Quit:
                            ParentContainer.Transition(new TitleSceneSituation(), new CommonFadeTransitionEffect());
                            break;
                        case BattleState.Retry:
                            Debug.LogError("BattleState.Retryは未実装");
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
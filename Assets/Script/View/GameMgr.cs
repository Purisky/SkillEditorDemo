using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkillEditorDemo.View
{
    public class GameMgr : MonoBehaviour
    {
        public EcsSystems Systems;
        public UIDocument UIDocument;
        public MainUI MainUI;
        public static GameMgr Inst;
        void Start()
        {
            Inst = this;
            TreeNode.Utility.Debug.Init(Debug.Log, Debug.LogError, (start, end, color, time) =>
             {
                 Debug.DrawLine(new(start.X, 0, start.Y), new(end.X, 0, end.Y), new Color(color.R / 256f, color.G / 256f, color.B / 256f), time);
             });
            DataMgr.Inst.Init();
            MainUI = UIDocument.rootVisualElement.Q<MainUI>();



            //获取所有VisualUnitSpawner组件并调用SpawnUnitAndDestroy方法
            VisualUnitSpawner[] visualUnitSpawners = FindObjectsByType<VisualUnitSpawner>(FindObjectsSortMode.None);
            for (int i = 0; i < visualUnitSpawners.Length; i++)
            {
                visualUnitSpawners[i].SpawnUnitAndDestroy();
            }
            Model.GameCore.Inst.Init();
            Systems = new EcsSystems(Model.GameCore.Inst.World);
            Systems.Add(new InputSystem());
            Systems.Add(new RenderSystem());
            Systems.Init();

        }

        void Update()
        {
            Systems.Run();
        }
        private void FixedUpdate()
        {
            Model.GameCore.Inst.Update();
        }

    }
}

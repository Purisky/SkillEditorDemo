using Leopotam.EcsLite;
using UnityEngine;

namespace SkillEditorDemo.View
{
    public class GameMgr : MonoBehaviour
    {
        public EcsSystems Systems;
        void Start()
        {
            Utility.Debug.Init(Debug.Log, Debug.LogError);
            DataMgr.Inst.Init();
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

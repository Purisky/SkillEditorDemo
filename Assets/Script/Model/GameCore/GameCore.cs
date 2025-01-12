using Leopotam.EcsLite;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public class GameCore : Singleton<GameCore>
    {
        public EcsWorld World;
        public EcsSystems Systems;
        public override void Init()
        {
            World = new();
            Systems = new(World);
            Systems.Add(new GameTickStartSystem());
            Systems.Add(new InputSystem());
            Systems.Add(new AISystem());
            //Systems.Add(new PhysicSystem());
            Systems.Add(new CollisionHandlerSystem());
            Systems.Add(new UnitCombatResSystem());
            Systems.Add(new BuffPeriodicSystem());
            Systems.Add(new BuffTimeOutSystem());
            Systems.Add(new UnitDeadSystem());
            Systems.Add(new GameTickEndSystem());
            Systems.Init();
        }



        public void Update()
        {
            if (Time.Pause) { return; }
            Systems.Run();
        }







    }
}

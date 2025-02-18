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
            Systems.Add(new CreateSystem());

            Systems.Add(new AISystem());

            Systems.Add(new VelocitySystem());
            Systems.Add(new CollisionDetectionSystem());
            Systems.Add(new CollisionHandlerSystem());
            Systems.Add(new AfterCollisionSystem());
            Systems.Add(new UnitCombatResSystem());
            Systems.Add(new BuffPeriodicSystem());
            Systems.Add(new BuffTimeOutSystem());

            Systems.Add(new ReleaseSystem());
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

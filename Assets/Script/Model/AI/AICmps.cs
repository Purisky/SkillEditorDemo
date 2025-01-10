namespace SkillEditorDemo.Model
{
    public struct AICmp
    { 
        public AIState State;
        public float AtkRange;
        public float SearchRange;
        public float ThinkInterval;
        public float ThinkTime;
        public int Target;
    }
    


    public enum AIState
    {
        Idle,
        IdleMove,
        Attack,
        Pursuit,
    }
}

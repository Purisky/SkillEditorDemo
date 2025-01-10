using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
namespace SkillEditorDemo.Model
{
    public struct UnitDeadComp { }

    public struct UnitCmp
    {
        public Unit Unit;
    }

    public struct InputMoveComp
    {
    }
    public struct InputLookTo
    {
        public float rot;
    }
    public struct InputLookAt
    {
        public float x;
        public float y;
    }

}

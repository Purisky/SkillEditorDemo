using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Numerics;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#BB8588")]
    public abstract class TransformNode : JsonNode
    {
        public abstract TransformCmp GetResult(TrigInfo info, CombatCache cache);
    }

    [NodeInfo(typeof(TransformNode), @"XYAngle", 180, "变换/XYAngle")]
    public class Vector2RotNode : TransformNode
    {
        [Child]
        public Vector2Node Vector2;
        [ShowInNode]
        public FuncValue Angle;
        public override TransformCmp GetResult(TrigInfo info, CombatCache cache)
        {
            return new TransformCmp()
            { 
                Pos = Vector2.GetResult(info, cache),
                Rot = Angle.GetResult(info, cache)
            };
        }
    }







    [NodeInfo(typeof(TransformNode), "获取单位变换", 180, "变换/单位变换")]
    public class GetUnitTransform : TransformNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;

        public override TransformCmp GetResult(TrigInfo info, CombatCache cache)
        {
            Unit unit = UnitNode.GetUnits(info, cache)[0];
            return unit.Entity.Get<TransformCmp>();
        }
    }
    [NodeInfo(typeof(TransformNode), "获取命中变换", 150, "变换/命中变换")]
    public class GetHitTransform : TransformNode
    {
        public override TransformCmp GetResult(TrigInfo info, CombatCache cache)
        {
            return cache.HitInfo;
        }
    }
    [NodeInfo(typeof(TransformNode), "变换相加", 150, "变换/变换相加")]
    public class AddTransform : TransformNode
    {
        [Child(true),LabelInfo(Hide =true)]
        public TransformNode TransformA;
        [Child(true), LabelInfo(Hide = true)]
        public TransformNode TransformB;
        public override TransformCmp GetResult(TrigInfo info, CombatCache cache)
        {
            return TransformA.GetResult(info, cache) + TransformB.GetResult(info, cache);
        }
    }




}

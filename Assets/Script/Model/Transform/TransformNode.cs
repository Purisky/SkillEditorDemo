using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Numerics;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#BB8588")]
    [Prompt(@"变换节点的基类,用于描述一个变换的基本信息,包括位置和旋转等")]
    public abstract class TransformNode : JsonNode
    {
        public abstract TransformCmp GetResult(TrigInfo info, CombatCache cache);
    }

    [NodeInfo(typeof(TransformNode), @"XYAngle", 180, "变换/XYAngle")]
    [Prompt(@"获取一个变换,包含位置和角度,位置为Vector2,角度为度")]
    public partial class Vector2RotNode : TransformNode
    {
        [Child]
        [Prompt(@"位置")]
        public Vector2Node Vector2;
        [ShowInNode]
        [Prompt(@"角度 in degrees (0,1)=>0, (1,0)=>90, (0,-1)=>180, (-1,0)=>270")]
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
    [Prompt(@"获取单位的变换,包含位置和旋转等信息")]
    public partial class GetUnitTransform : TransformNode
    {
        [Child(true), TitlePort]
        [Prompt(@"获取变换的单位")]
        public UnitNode UnitNode;

        public override TransformCmp GetResult(TrigInfo info, CombatCache cache)
        {
            Unit unit = UnitNode.GetUnit(info, cache);
            return unit.Entity.Get<TransformCmp>();
        }
    }
    [NodeInfo(typeof(TransformNode), "获取命中变换", 150, "变换/命中变换")]
    [Prompt(@"获取命中的变换,包含位置和旋转等信息,通常用于获取命中时投射物的变换")]
    public partial class GetHitTransform : TransformNode
    {
        public override TransformCmp GetResult(TrigInfo info, CombatCache cache)
        {
            return cache.HitInfo;
        }
    }
    [NodeInfo(typeof(TransformNode), "变换相加", 150, "变换/变换相加")]
    [Prompt(@"将两个变换相加,位置相加,角度相加")]
    public partial class AddTransform : TransformNode
    {
        [Child(true),LabelInfo(Hide =true)]
        [Prompt(@"变换A")]
        public TransformNode TransformA;
        [Child(true), LabelInfo(Hide = true)]
        [Prompt(@"变换B")]
        public TransformNode TransformB;
        public override TransformCmp GetResult(TrigInfo info, CombatCache cache)
        {
            return TransformA.GetResult(info, cache) + TransformB.GetResult(info, cache);
        }
    }




}

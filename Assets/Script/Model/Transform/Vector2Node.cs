using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Numerics;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#D6CE93")]
    public abstract class Vector2Node : JsonNode
    {
        public abstract Vector2 GetResult(TrigInfo info, CombatCache cache);
    }

    [NodeInfo(typeof(Vector2Node), "XY", 100, "向量/XY")]
    public class XYNode : Vector2Node
    {
        [ShowInNode,LabelInfo("X",Width = 20)]
        public FuncValue X;
        [ShowInNode, LabelInfo("Y", Width = 20)]
        public FuncValue Y;
        public override Vector2 GetResult(TrigInfo info, CombatCache cache)
        {
            return new Vector2(X.GetResult(info,cache), Y.GetResult(info, cache));
        }
    }
    [NodeInfo(typeof(Vector2Node), "获取单位坐标", 150, "向量/单位坐标")]
    public class GetUnitPosition : Vector2Node
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;

        public override Vector2 GetResult(TrigInfo info, CombatCache cache)
        {
            Unit unit = UnitNode.GetUnits(info, cache)[0];
            return unit.Entity.Get<TransformCmp>().Pos;
        }
    }







}

using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Numerics;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#D6CE93")]
    [Prompt(@"向量节点的基类,所有的向量节点都继承自Vector2Node,用于在游戏场景中创建向量如单位坐标/向量计算等")]
    public abstract class Vector2Node : JsonNode
    {
        public abstract Vector2 GetResult(TrigInfo info, CombatCache cache);
    }

    [NodeInfo(typeof(Vector2Node), "XY", 100, "向量/XY")]
    [Prompt(@"XY节点,用于获取X和Y的值,可以是常量/变量/函数等")]
    public class XYNode : Vector2Node
    {
        [ShowInNode,LabelInfo("X",Width = 20)]
        [Prompt(@"X轴的值")]
        public FuncValue X;
        [ShowInNode, LabelInfo("Y", Width = 20)]
        [Prompt(@"Y轴的值")]
        public FuncValue Y;

        public override string GetText(int indent = 0)
        {
            string xText = X?.GetText(indent) ?? "0";
            string yText = Y?.GetText(indent) ?? "0";
            return $"({xText},{yText})";
        }

        public override Vector2 GetResult(TrigInfo info, CombatCache cache)
        {
            return new Vector2(X.GetResult(info,cache), Y.GetResult(info, cache));
        }
    }
    [NodeInfo(typeof(Vector2Node), "获取单位坐标", 150, "向量/单位坐标")]
    [Prompt(@"获取单位坐标节点,用于获取指定单位的坐标位置")]
    public class GetUnitPosition : Vector2Node
    {
        [Child(true), TitlePort]
        [Prompt(@"获取单位的节点")]
        public UnitNode UnitNode;

        public override string GetText(int indent = 0)
        {
            string unitText = UnitNode?.GetText(indent) ?? "单位";
            return $"{unitText}的坐标";
        }

        public override Vector2 GetResult(TrigInfo info, CombatCache cache)
        {
            Unit unit = UnitNode.GetUnits(info, cache)[0];
            return unit.Entity.Get<TransformCmp>().Pos;
        }
    }







}

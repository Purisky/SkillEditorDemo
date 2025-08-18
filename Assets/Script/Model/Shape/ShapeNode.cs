using SkillEditorDemo.Utility;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#383961")]
    [Prompt(@"形状节点的基类,用于描述一个形状,如圆/矩形/扇形等,可以用于碰撞检测/范围计算等")]
    public abstract class ShapeNode : JsonNode
    {
        public abstract IAABB GetShape(TrigInfo info, CombatCache cache);
    }

    [NodeInfo(typeof(ShapeNode), "圆", 100, "形状/圆")]
    [Prompt(@"圆形形状节点")]
    public partial class CircleShape : ShapeNode
    {
        [ShowInNode, LabelInfo("半径")]
        [Prompt(@"圆形的半径")]
        public FuncValue Radius;

        public override IAABB GetShape(TrigInfo info, CombatCache cache)
        {
            float radius = Radius.GetResult(info, cache);
            return new Circle() {  Radius = radius, Dirty = true};
        }
    }
    [NodeInfo(typeof(ShapeNode), "矩形", 200, "形状/矩形")]
    [Prompt(@"矩形形状节点")]
    public partial class RectShape : ShapeNode
    {
        [ShowInNode, LabelInfo("宽度")]
        [Prompt(@"矩形的宽度")]
        public FuncValue Width;
        [ShowInNode, LabelInfo("高度")]
        [Prompt(@"矩形的高度")]
        public FuncValue Height;
        public override IAABB GetShape(TrigInfo info, CombatCache cache)
        {
            float width = Width.GetResult(info, cache);
            float height = Height.GetResult(info, cache);
            return new Rectangle() {Size = new(width,height), Dirty = true };
        }
    }
    [NodeInfo(typeof(ShapeNode), "扇形", 200, "形状/扇形")]
    [Prompt(@"扇形形状节点")]
    public partial class SectorShape : ShapeNode
    {
        [ShowInNode, LabelInfo("半径")]
        [Prompt(@"扇形的半径")]
        public FuncValue Radius;
        [ShowInNode, LabelInfo("角度")]
        [Prompt(@"扇形的角度,单位为度,扇形会以正前方为中线向两边展开")]
        public FuncValue Angle;

        public override IAABB GetShape(TrigInfo info, CombatCache cache)
        {
            float radius = Radius.GetResult(info, cache);
            float angle = Angle.GetResult(info, cache);
            return new Sector() { Radius = radius, Angle = angle, Dirty = true };
        }
    }
    //[NodeInfo(typeof(ShapeNode), "网格", 200, "形状/网格")]
    //public class MeshShape : ShapeNode
    //{
    //}
}

using SkillEditorDemo.Utility;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#383961")]
    public abstract class ShapeNode : JsonNode
    {
        public abstract IAABB GetShape(TrigInfo info, CombatCache cache);
    }

    [NodeInfo(typeof(ShapeNode), "圆", 200, "形状/圆")]
    public class CircleShape : ShapeNode
    {
        [ShowInNode, LabelInfo("半径")]
        public FuncValue Radius;

        public override IAABB GetShape(TrigInfo info, CombatCache cache)
        {
            float radius = Radius.GetResult(info, cache);
            return new Circle() {  Radius = radius, Dirty = true};
        }
    }
    [NodeInfo(typeof(ShapeNode), "矩形", 200, "形状/矩形")]
    public class RectShape : ShapeNode
    {
        [ShowInNode, LabelInfo("宽度")]
        public FuncValue Width;
        [ShowInNode, LabelInfo("高度")]
        public FuncValue Height;
        public override IAABB GetShape(TrigInfo info, CombatCache cache)
        {
            float width = Width.GetResult(info, cache);
            float height = Height.GetResult(info, cache);
            return new Rectangle() {Size = new(width,height), Dirty = true };
        }
    }
    [NodeInfo(typeof(ShapeNode), "扇形", 200, "形状/扇形")]
    public class SectorShape : ShapeNode
    {
        [ShowInNode, LabelInfo("半径")]
        public FuncValue Radius;
        [ShowInNode, LabelInfo("角度")]
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

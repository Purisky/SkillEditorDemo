using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#F0A58A")]
    public class ObjNode : JsonNode
    {
    }
    [NodeInfo(typeof(ObjNode), "投射物", 200, "对象/投射物")]
    public class NewProjectile : ObjNode
    {
        [ShowInNode,LabelInfo("持续时间")]
        public TimeValue Time;
        [ShowInNode, LabelInfo("飞行速度")]
        public NumValue Speed;
        [ShowInNode, LabelInfo("超时触发效果")]
        public bool TrigOnTimeout;
        [ShowInNode, LabelInfo("半径")]
        public NumValue Radius;
        [Child(true), LabelInfo("效果组")]
        public List<ActionNode> Actions;

        [ShowInNode,Dropdown(nameof(GetProjectiles)), LabelInfo("特效")]
        public string DisplayPath;



        static DropdownList<string> GetProjectiles()
        {
            DropdownList<string> items = new();


            return items;
        }
    }
    [NodeInfo(typeof(ObjNode), "伤害盒子", 200, "对象/伤害盒子")]
    public class NewHitBox : ObjNode
    {
        [Child(true)]
        public ShapeNode Shape;

        [Child(true), LabelInfo("效果组")]
        public List<ActionNode> Actions;

        [ShowInNode, Dropdown(nameof(GetProjectiles)), LabelInfo("特效")]
        public string DisplayPath;
        static DropdownList<string> GetProjectiles()
        {
            DropdownList<string> items = new();
            return items;
        }
    }
}

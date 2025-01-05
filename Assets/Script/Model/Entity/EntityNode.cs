using Newtonsoft.Json;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#F0A58A")]
    public class EntityNode : JsonNode
    {
    }
    [NodeInfo(typeof(EntityNode), "投射物", 200, "实体/投射物")]
    public class NewProjectile : EntityNode
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
    [NodeInfo(typeof(EntityNode), "伤害盒子", 200, "实体/伤害盒子")]
    public class NewHitBox : EntityNode
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

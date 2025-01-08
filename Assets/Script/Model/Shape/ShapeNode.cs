using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#383961")]
    public class ShapeNode : JsonNode
    {

    }

    [NodeInfo(typeof(ShapeNode), "圆", 200, "形状/圆")]
    public class CircleShape : ShapeNode
    {
        [ShowInNode, LabelInfo("半径")]
        public NumValue Radius;
    }
    [NodeInfo(typeof(ShapeNode), "矩形", 200, "形状/矩形")]
    public class RectShape : ShapeNode
    {
        [ShowInNode, LabelInfo("宽度")]
        public NumValue Width;
        [ShowInNode, LabelInfo("高度")]
        public NumValue Height;
    }
    [NodeInfo(typeof(ShapeNode), "扇形", 200, "形状/扇形")]
    public class SectorShape : ShapeNode
    {
        [ShowInNode, LabelInfo("半径")]
        public NumValue Radius;
        [ShowInNode, LabelInfo("角度")]
        public NumValue Angle;
    }
    //[NodeInfo(typeof(ShapeNode), "网格", 200, "形状/网格")]
    //public class MeshShape : ShapeNode
    //{
    //}
}

using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#D94F3D")]
    public abstract class BaseAction : JsonNode
    {

    }
    [NodeInfo(typeof(BaseAction), "分支执行", 100, "执行/分支执行")]
    public class ConditionAction : BaseAction
    {
        [Child, LabelInfo(Text = "条件"), TitlePort]
        public Condition Condition;
        [Child, LabelInfo(Text = "真", Width = 10)]
        public BaseAction True;
        [Child, LabelInfo(Text = "假", Width = 10)]
        public BaseAction False;
    }



    [NodeInfo(typeof(BaseAction), "实体属性修改", 200, "执行/实体属性修改")]
    public class EntityProperty : BaseAction
    {
        [Child, LabelInfo("执行目标", 30), TitlePort]
        public Entity Entity;






    }
    public class Damage
    {

    }


}

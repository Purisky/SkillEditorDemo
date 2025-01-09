using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public enum BuffValueType
    {
        [LabelInfo("存值")]
        RuntimeData,
        [LabelInfo("当前等级")]
        Level,
        [LabelInfo("最大等级")]
        MaxLevel,
        [LabelInfo("当前层数")]
        Degree,
        [LabelInfo("最大层数")]
        MaxDegree,
        [LabelInfo("最大持续时间")]
        TotalTime,
        [LabelInfo("剩余持续时间")]
        RestTime,
        [LabelInfo("已存在时间")]
        ExistTime,
        [LabelInfo("触发器触发次数")]
        TotalTriggerCount,
        [LabelInfo("创建参数0")]
        Param0,
        //[LabelInfo("创建参数1")]
        //Param1,
        //[LabelInfo("创建参数2")]
        //Param2,
        //[LabelInfo("创建参数3")]
        //Param3,
    }
    public enum ValueGetterType
    {
        [LabelInfo("全局")]
        Global,
        [LabelInfo("单位")]
        Unit,
        [LabelInfo("Buff")]
        Buff,
        [LabelInfo("技能")]
        Skill,
        [LabelInfo("缓存")]
        Cache,
    }
}

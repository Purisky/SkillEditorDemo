using UnityEngine;
namespace SkillEditorDemo
{
    public enum CompeteType
    {
        [InspectorName("无事发生")]
        None,
        [InspectorName("被动")]
        Eternal,
        [InspectorName("独立")]
        Indie,
        [InspectorName("刷新")]
        Refresh,
        [InspectorName("替换刷新")]
        ReplaceRefresh,
        [InspectorName("刷新至满层")]
        Refresh2Max,
        [InspectorName("叠层并刷新")]
        AddRefresh,
    }
}

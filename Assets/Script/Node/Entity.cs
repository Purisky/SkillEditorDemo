using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#E8E3B4")]
    public abstract class Entity : JsonNode
    {

    }
    [NodeInfo(typeof(Entity), "Buff实体获取", 140, "实体/Buff实体获取"), AssetFilter(true, typeof(BuffAsset))]
    public class BuffEntity : Entity
    {
        [ShowInNode, LabelInfo(Hide = true)]
        public BuffEntityType BuffEntityType;
    }
    [NodeInfo(typeof(List<Entity>), "获取所有实体", 140, "实体/获取所有")]
    public class GetAllEntity : Entity
    {
    }
    [NodeInfo(typeof(List<Entity>), "实体筛选", 140, "实体/筛选")]
    public class EntityFilter : Entity
    {
        [Child, LabelInfo("获取实体"),TitlePort]
        public List<Entity> EntityList;
        [Child, LabelInfo("条件")]
        public Condition Condition;
    }


    [NodeInfo(typeof(List<Entity>), "实体排序", 140, "实体/排序")]
    public class EntityOrder : Entity
    {
        [Child,LabelInfo("获取实体"), TitlePort]
        public List<Entity> EntityList;
        [ShowInNode, LabelInfo("比较数值", 45)]
        public NumValue Min;
        [ShowInNode, LabelInfo("反序", 30)]
        public bool ByDescending;
    }
    [NodeInfo(typeof(Entity), "取出实体", 140, "实体/取出")]
    public class TakeOutEntity : Entity
    {
        [Child, LabelInfo("获取实体"), TitlePort]
        public List<Entity> EntityList;
        [ShowInNode, LabelInfo("索引", 45)]
        public NumValue Index;
    }
    [NodeInfo(typeof(List<Entity>), "实体缓存", 140, "实体/缓存")]
    public class EntityListCache : Entity
    {
    }
    [NodeInfo(typeof(Condition), "实体存在", 140, "条件/实体存在")]
    public class EntityAny : Condition
    {
        [Child, LabelInfo("获取实体"), TitlePort]
        public List<Entity> EntityList;

        public override string GetText()
        {
            return $"?";
        }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#E8E3B4")]
    public abstract class Entity : JsonNode
    {
        public virtual string GetText()
        {
            return "实体";
        }
    }
    [NodeInfo(typeof(Entity), "当前实体", 100, "实体/当前实体")]
    public class LastEntity : Entity
    {
        public override string GetText() => "当前实体";
    }
    [NodeInfo(typeof(Entity), "Buff实体获取", 140, "实体/Buff实体获取"), AssetFilter(true, typeof(BuffAsset))]
    public class BuffEntity : Entity
    {
        [ShowInNode, LabelInfo(Hide = true)]
        public BuffEntityType BuffEntityType;
        public override string GetText() => $"Buff.{BuffEntityType.GetLabel()}";

    }
    [NodeInfo(typeof(List<Entity>), "获取所有实体", 140, "实体/获取所有")]
    public class GetAllEntity : Entity
    {
        public override string GetText() => $"所有实体";
    }
    [NodeInfo(typeof(List<Entity>), "实体筛选", 140, "实体/筛选")]
    public class EntityFilter : Entity
    {
        [Child(true), TitlePort]
        public List<Entity> EntityList;
        [Child(true), LabelInfo("条件")]
        public Condition Condition;

        public override string GetText()
        {
            string entityText = "";
            if (EntityList != null)
            {
                entityText = string.Join(",", EntityList.Select(n => n.GetText()));
            }
            return $"([{entityText}]=>{Condition.GetText()})";
        }
    }


    [NodeInfo(typeof(List<Entity>), "实体排序", 140, "实体/排序")]
    public class EntityOrder : Entity
    {
        [Child(true), TitlePort]
        public List<Entity> EntityList;
        [ShowInNode, LabelInfo("比较数值", 45)]
        public NumValue Min;
        [ShowInNode, LabelInfo("反序", 30)]
        public bool ByDescending;

        public override string GetText()
        {
            string entityText = "";
            if (EntityList != null)
            {
                entityText = string.Join(",", EntityList.Select(n => n.GetText()));
            }
            return $"([{entityText}].排序)";
        }
    }
    [NodeInfo(typeof(Entity), "取出实体", 140, "实体/取出")]
    public class TakeOutEntity : Entity
    {
        [Child(true), TitlePort]
        public List<Entity> EntityList;
        [ShowInNode, LabelInfo("索引", 45)]
        public NumValue Index;
        public override string GetText()
        {
            string entityText = "";
            if (EntityList != null)
            {
                entityText = string.Join(",", EntityList.Select(n => n.GetText()));
            }
            return $"([{entityText}].[{Index.GetText()}])";
        }
    }
    [NodeInfo(typeof(List<Entity>), "当前实体列表", 140, "实体/当前实体列表")]
    public class LastEntityList : Entity
    {
        public override string GetText()
        {
            return $"当前实体列表";
        }
    }

    [NodeInfo(typeof(Entity), "实体迭代器", 140, "实体/迭代器")]
    public class EntityEnumerator : Entity
    {
        public override string GetText() => $"实体迭代器";
    }
    [NodeInfo(typeof(Condition), "碰撞实体", 100, "实体/碰撞")]
    public class EntityCollision : Entity
    {
        public override string GetText() => $"碰撞实体";
    }
    [NodeInfo(typeof(Condition), "实体存在", 100, "条件/实体存在")]
    public class EntityAny : Condition
    {
        [Child(true), TitlePort]
        public List<Entity> EntityList;

        public override string GetText()
        {
            string entityText = "";
            if (EntityList != null)
            {
                entityText = string.Join(",", EntityList.Select(n => n.GetText()));
            }
            return $"([{entityText}].存在)";
        }
    }



}

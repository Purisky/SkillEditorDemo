using Leopotam.EcsLite;
using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#F0A58A")]
    public abstract class ObjNode : JsonNode, IGrowID<ObjNode>
    {
        [JsonIgnore]
        public int GrowID { get; set; }
        public static T Get<T>(int id) where T : ObjNode => (T)IGrowID<ObjNode>.Get(id);

        public abstract void Create(int entity, TrigInfo info, CombatCache cache);
    }
    [NodeInfo(typeof(ObjNode), "投射物", 200, "对象/投射物")]
    public class NewProjectile : ObjNode
    {
        [ShowInNode, LabelInfo("持续时间")]
        public TimeValue Time;
        [ShowInNode, LabelInfo("飞行速度")]
        public FuncValue Speed;
        [ShowInNode, LabelInfo("超时触发效果")]
        public bool TrigOnTimeout;
        [ShowInNode, LabelInfo("击中地形触发效果")]
        public bool TrigOnHitTerrain;
        [ShowInNode, LabelInfo("半径")]
        public FuncValue Radius;
        [Child(true), LabelInfo("效果组")]
        public List<ActionNode> Actions;

        [ShowInNode, Dropdown(nameof(GetProjectiles)), LabelInfo("特效")]
        public string DisplayPath;



        static DropdownList<string> GetProjectiles()
        {
            DropdownList<string> items = new();
            return items;
        }

        public override void Create(int entity, TrigInfo info, CombatCache cache)
        {
            entity.Add(new ProjectileCreateCmp()
            {
                GrowID = GrowID,
                TrigInfo = info,
                Cache = cache.Clone(),
            });
        }
    }
    [NodeInfo(typeof(ObjNode), "伤害盒子", 200, "对象/伤害盒子")]
    public class NewHitbox : ObjNode
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

        public override void Create(int entity, TrigInfo info, CombatCache cache)
        {
            entity.Add(new HitboxCreateCmp()
            {
                GrowID = GrowID,
                TrigInfo = info,
                Cache = cache.Clone(),
            });
        }
    }
}

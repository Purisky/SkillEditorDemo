using Leopotam.EcsLite;
using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#F0A58A")]
    [Prompt(@"对象节点的基类,所有的对象节点都继承自ObjNode,用于在游戏场景中创建对象如地形/单位/伤害盒子/物品等")]
    public abstract class ObjNode : JsonNode, IGrowID<ObjNode>
    {
        [JsonIgnore]
        public int GrowID { get; set; }
        public static T Get<T>(int id) where T : ObjNode => (T)IGrowID<ObjNode>.Get(id);
        public abstract void Create(int entity, TrigInfo info, CombatCache cache);
    }
    [NodeInfo(typeof(ObjNode), "投射物", 210, "对象/投射物")]
    [Prompt(@"用于创建投射物,投射物是一种运动的伤害盒子,在给定初速之后和运动方向后会一直向前运动直到产生碰撞或者持续时间耗尽,投射物在产生一次碰撞后销毁")]
    public partial class NewProjectile : ObjNode
    {
        [ShowInNode, LabelInfo("持续时间")]
        [Prompt(@"投射物的持续时间")]
        public TimeValue Time;
        [ShowInNode, LabelInfo("飞行速度")]
        [Prompt(@"投射物的飞行速度")]
        public FuncValue Speed;
        [ShowInNode, LabelInfo("半径")]
        [Prompt(@"投射物的碰撞盒半径")]
        public FuncValue Radius;
        [Child(true), LabelInfo("效果组")]
        [Prompt(@"投射物的碰撞效果,在默认情况下只有碰撞到单位才会触发ActionNode")]
        public List<ActionNode> Actions;
        [ShowInNode, LabelInfo("超时触发效果")]
        [Prompt(@"当设为true,投射物超时(飞行至最大距离)也会触发碰撞效果")]
        public bool TrigOnTimeout;
        [ShowInNode, LabelInfo("击中地形触发效果")]
        [Prompt(@"当设为true,投射物碰撞到地形时也会触发碰撞效果")]
        public bool TrigOnHitTerrain;
        [ShowInNode, Dropdown(nameof(GetProjectiles)), LabelInfo("特效")]
        public string DisplayPath; 


#if UNITY_EDITOR
        static DropdownList<string> GetProjectiles()
        {
            DropdownList<string> items = new();
            string[] names = EditorPath.GetFileNames("Resources/Prefab/Projectile", "*.prefab", SearchOption.AllDirectories);
            for (int i = 0; i < names.Length; i++)
            {
                items.Add(new DropdownItem<string>(names[i], names[i]));
            }
            return items;
        }
#endif

        public override void Create(int entity, TrigInfo info, CombatCache cache)
        {
            entity.Add(new ProjectileCreateCmp()
            {
                GrowID = GrowID,
                TrigInfo = info,
                Cache = cache.Clone(),
            });
        }

        public override string GetText(int indent = 0)
        {
            List<string> paramParts = new List<string>();
            
            // 基础参数 - 总是显示
            string speedText = Speed?.GetText(indent) ?? "0";
            paramParts.Add($"速度{speedText}");
            
            string radiusText = Radius?.GetText(indent) ?? "0";
            paramParts.Add($"半径{radiusText}");
            
            string timeText = Time.GetText(indent) ?? "0";
            paramParts.Add($"持续{timeText}");
            
            // 触发条件
            List<string> triggerConditions = new List<string>();
            triggerConditions.Add("命中单位"); // 默认总是触发
            if (TrigOnHitTerrain) triggerConditions.Add("地形");
            if (TrigOnTimeout) triggerConditions.Add("超时");
            
            // 执行效果
            string actionText = "无事发生";
            if (Actions != null && Actions.Count > 0)
            {
                if (Actions.Count == 1)
                {
                    actionText = Actions[0].GetText(indent);
                }
                else
                {
                    actionText = $"[{string.Join(",", Actions.Select(a => a.GetText(indent)))}]";
                }
            }
            
            // 组装最终描述
            string paramText = string.Join(",", paramParts);
            string triggerText = string.Join("|", triggerConditions);
            
            return $"{paramText}且({triggerText})后:{actionText}的投射物";
        }

        public void Hit(TrigInfo info, CombatCache cache)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Handle(5, info, cache);
            }
        }
    }
    [NodeInfo(typeof(ObjNode), "碰撞盒子", 210, "对象/碰撞盒子")]
    [Prompt(@"用于创建碰撞盒子,碰撞盒子是一种静态的碰撞区域,当其他对象进入时会触发预设的效果,碰撞盒子只会存在一帧,可以同时碰撞多个单位")]
    public partial class OneShotBox : ObjNode
    {
        [Child(true), LabelInfo("形狀")]
        [Prompt(@"碰撞盒子的碰撞形状,决定了检测区域的范围和类型")]
        public ShapeNode Shape;

        [Child(true), LabelInfo("效果组")]
        [Prompt(@"碰撞盒子的碰撞效果,在默认情况下只有碰撞到单位才会触发ActionNode")]
        public List<ActionNode> Actions;

        [ShowInNode, Dropdown(nameof(GetHitboxes)), LabelInfo("特效")]
        public string DisplayPath;
#if UNITY_EDITOR
        static DropdownList<string> GetHitboxes()
        {
            DropdownList<string> items = new();
            string[] names = EditorPath.GetFileNames("Resources/Prefab/Hitbox", "*.prefab", SearchOption.AllDirectories);
            for (int i = 0; i < names.Length; i++)
            {
                items.Add(new DropdownItem<string>(names[i], names[i]));
            }
            return items;
        }
#endif

        public override void Create(int entity, TrigInfo info, CombatCache cache)
        {
            entity.Add(new HitboxCreateCmp()
            {
                GrowID = GrowID,
                TrigInfo = info,
                Cache = cache.Clone(),
            });
        }

        public override string GetText(int indent = 0)
        {
            // 形状描述
            string shapeText = Shape?.GetText(indent) ?? "形状";
            
            // 执行效果
            string actionText = "无事发生";
            if (Actions != null && Actions.Count > 0)
            {
                if (Actions.Count == 1)
                {
                    actionText = Actions[0].GetText(indent);
                }
                else
                {
                    actionText = $"[{string.Join(",", Actions.Select(a => a.GetText(indent)))}]";
                }
            }
            
            return $"{shapeText}且命中后:{actionText}的碰撞盒子";
        }

        public void Hit(TrigInfo info, CombatCache cache)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Handle(5, info, cache);
            }
        }


    }
}

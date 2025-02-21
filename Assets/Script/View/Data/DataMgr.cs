using SkillEditorDemo.Model;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo.View
{
    public class DataMgr : Singleton<DataMgr>
    {
        public override void Init()
        {
            InitUnits();
            InitBuffs();
            InitSkills();






        }

        void InitUnits()
        {
            UnitData[] unitDatas = Resources.LoadAll<UnitData>("Data");
            for (int i = 0; i < unitDatas.Length; i++)
            {
                IData<Model.UnitData>.Add(unitDatas[i].name, unitDatas[i].GenData());
            }
        }
        void InitBuffs()
        {
            TextAsset[] textAssets = Resources.LoadAll<TextAsset>("Buff");
            for (int i = 0; i < textAssets.Length; i++)
            {
                JsonAsset jsonAsset =   JsonAsset.GetJsonAssetByText(textAssets[i].text);
                if (jsonAsset.Data is BuffAsset buffAsset)
                {
                    HandleBuffAsset(buffAsset);
                }
            }
        }
        void HandleBuffAsset(BuffAsset buffAsset)
        {
            for (int i = 0; i < buffAsset.Nodes.Count; i++)
            {
                if (buffAsset.Nodes[i] is BuffNode buffNode)
                {
                    if (!string.IsNullOrEmpty(buffNode.ID)&& IData<BuffNode>.Get(buffNode.ID)==null)
                    {
                        HandleBuffNode(buffNode);
                    }
                }
            }
        }

        void HandleBuffNode(BuffNode buffNode)
        {
            IData<BuffNode>.Add(buffNode.ID,buffNode);
            List<JsonNode> Nodes =  JsonNodeHelper.GetAllJsonNodes(buffNode);
            for (int i = 0; i < Nodes.Count; i++)
            {
                HandleIGrowIDNode(Nodes[i]);
            }
        }
        void HandleIGrowIDNode(JsonNode jsonNode)
        {
            if (jsonNode is ActionNode actionNode)
            {
                IGrowID<ActionNode>.Add(actionNode);
                return;
            }
            if (jsonNode is TrigNode trigNode)
            {
                IGrowID<TrigNode>.Add(trigNode);
                return;
            }
            if (jsonNode is Condition condition)
            {
                IGrowID<Condition>.Add(condition);
                return;
            }
            if (jsonNode is FuncNode  funcNode)
            {
                IGrowID<FuncNode>.Add(funcNode);
                return;
            }
            if (jsonNode is ObjNode  objNode)
            {
                IGrowID<ObjNode>.Add(objNode);
                return;
            }
        }






        void InitSkills()
        {
            TextAsset[] textAssets = Resources.LoadAll<TextAsset>("Skill");
            for (int i = 0; i < textAssets.Length; i++)
            {
                JsonAsset jsonAsset = JsonAsset.GetJsonAssetByText(textAssets[i].text);
                if (jsonAsset.Data is SkillAsset  skillAsset)
                {
                    HandleSkillAsset(skillAsset);
                }
            }



        }
        void HandleSkillAsset(SkillAsset skillAsset)
        {
            for (int i = 0; i < skillAsset.Nodes.Count; i++)
            {
                if (skillAsset.Nodes[i] is SkillNode skillNode)
                {
                    if (!string.IsNullOrEmpty(skillNode.ID) && IData<SkillNode>.Get(skillNode.ID) == null)
                    {
                        HandleSkillNode(skillNode);
                    }
                }
            }
        }

        void HandleSkillNode(SkillNode skillNode)
        {
            IData<SkillNode>.Add(skillNode.ID, skillNode);
            List<JsonNode> Nodes = JsonNodeHelper.GetAllJsonNodes(skillNode);
            for (int i = 0; i < Nodes.Count; i++)
            {
                HandleIGrowIDNode(Nodes[i]);
            }
        }

    }
}

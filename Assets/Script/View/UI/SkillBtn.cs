using SkillEditorDemo.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkillEditorDemo.View
{
    [UxmlElement]
    public partial class SkillBtn : VisualElement
    {
        public bool Empty;
        VisualElement Disable;

        Label Name;
        [UxmlAttribute]
        public string KeyText
        {
            get => keyText;
            set
            {
                keyText = value;
                this.Q<Label>("Key").text = keyText;
                MarkDirtyRepaint();
            }
        }
        string keyText;
        Label Count;
        Label TimeText;


        public SkillBtn()
        {
            Resources.Load<VisualTreeAsset>("UI/SkillBtn").CloneTree(this);
            Disable  = this.Q<VisualElement>("Disable");
            Name = this.Q<Label>("Name");
            Count = this.Q<Label>("Count");
            TimeText = this.Q<Label>("Time");
        }

        public void SetSkill(Skill skill)
        {
            if (skill == null)
            {
                SetEmpty();
                return;
            }
            SkillNode skillNode = skill.Data;
            SetName(skillNode.Name);
            SetCount(skill.Charge);
            if (skill.ColdDownTick > 0)
            {
                UpdateCD(skill.ColdDownTick * Model.Time.GameTickDuration);
            }
            else
            {
                SetDisable(false);
            }



        }

        public void SetEmpty()
        {
            Empty = true;
            this.visible = false;
        }
        public void SetDisable(bool isDisable)
        {
            Disable.style.display = isDisable ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public void UpdateCD(float cd)
        {
            TimeText.text = $"{cd:F2}s";
            SetDisable(true);
        }
        public void SetCount(int count)
        {
            if (count <= 1)
            { 
                Count.style.display = DisplayStyle.None;
            }
            else
            {
                Count.style.display = DisplayStyle.Flex;
                Count.text = count.ToString();
            }
        }
        public void SetName(string name)
        {
            Name.text = name;
        }


    }
}

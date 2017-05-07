﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework.Message;

namespace AGrail
{
    public class SkillUI : MonoBehaviour
    {
        [SerializeField]
        private RawImage skillIcon;
        [SerializeField]
        private Image selectBorder;
        [SerializeField]
        private Text skillName;
        [SerializeField]
        private Button btn;

        private Skill skill;
        public Skill Skill
        {
            set
            {
                skill = value;
                skillName.text = skill.SkillName;
                switch (skill.SkillType)
                {
                    case SkillType.启动:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/qidong");
                        break;
                    case SkillType.被动:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/beidong");
                        break;
                    case SkillType.响应:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/xiangying");
                        break;
                    case SkillType.法术:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/fashu");
                        break;
                }
                IsEnable = false;                
            }
            get { return skill; }
        }

        public bool IsEnable
        {
            set
            {
                if (!value)
                {
                    btn.interactable = false;
                    selectBorder.enabled = false;
                }
                else if(skill.SkillType == SkillType.法术)
                    btn.interactable = true;
            }
        }

        public void OnBtnClick()
        {
            selectBorder.enabled = !selectBorder.enabled;
            if (selectBorder.enabled)
            {
                BattleData.Instance.Agent.SelectSkill = skill.SkillID;
                BattleData.Instance.Agent.AgentUIState = skill.SkillID;
            }
            else
            {
                BattleData.Instance.Agent.SelectSkill = null;
                BattleData.Instance.Agent.AgentState = BattleData.Instance.Agent.AgentState;
            }                
        }
    }
}

﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Framework.Message;
using System;

namespace AGrail
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private Button btn;
        [SerializeField]
        private Text txtSkill1;
        [SerializeField]
        private Text txtSkill2;
        [SerializeField]
        private RawImage image;
        [SerializeField]
        private Image selectBorder;

        private bool isEnable = false;
        public bool IsEnable
        {
            set
            {
                isEnable = value;
                if (!isEnable)                
                    selectBorder.enabled = false;                
            }
        }

        private Card card;
        public Card Card
        {
            set
            {
                card = value;
                image.texture = Resources.Load<Texture2D>(card.AssetPath);
                if (card.SkillNum >= 1)
                {
                    txtSkill1.text = card.SkillNames[0];
                    if (card.SkillNum >= 2)
                        txtSkill2.text = card.SkillNames[1];
                }
                IsEnable = true;
            }
        }

        public void OnCardClick()
        {
            if (isEnable)
            {
                selectBorder.enabled = !selectBorder.enabled;
                MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard, card.ID);
            }    
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            var pos = canvas.transform.localPosition;
            pos.y += 10;
            canvas.transform.localPosition = pos;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10; 
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            var pos = canvas.transform.localPosition;
            pos.y = 0;
            canvas.transform.localPosition = pos;
            canvas.overrideSorting = false;
            canvas.sortingOrder = 0;
        }
    }
}


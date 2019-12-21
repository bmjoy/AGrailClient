﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AGrail;
using System;

//这个脚本用于执行调整UI的边缘
public class UIEdgeFix : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> roots = new List<RectTransform>();

    private void Start()
    {
        //StartCoroutine(DelayFix());
        FixEdge(GameManager.UIInstance.UIEdge);
    }

    //private IEnumerator DelayFix()
    //{
    //    yield return new WaitForSeconds(.0f);
    //    FixEdge(GameManager.UIInstance.UIEdge);
    //}

    public void FixEdge(float value)
    {
        foreach (var root in roots)
        {
            //判断为四周扩展类型的锚点预设
            if (root.anchorMin == Vector2.zero && root.anchorMax == Vector2.one)
            {
                if (value > .5f)
                {
                    //设置左下
                    root.offsetMin = new Vector2((value - .5f) * 200, 0);
                    root.offsetMax = new Vector2(0, 0);
                }
                else if (value < .5f)
                {
                    //设置右上
                    root.offsetMax = new Vector2(-(.5f - value) * 200, 0);
                    root.offsetMin = new Vector2(0, 0);
                }
            }
        }
    }
}

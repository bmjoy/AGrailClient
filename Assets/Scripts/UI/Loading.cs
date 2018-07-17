﻿using Framework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;

namespace AGrail
{
    public class Loading : UIBase
    {
        [SerializeField]
        private Text progress;

        public override string Type
        {
            get
            {
                return WindowType.Loading.ToString();
            }
        }

        public override IEnumerator Start()
        {
            yield return StartCoroutine(refreshRate());
            yield return base.Start();
        }

        private IEnumerator refreshRate()
        {
            var val = AssetBundleManager.Instance.Progress;
            while (val < 100)
            {
                yield return new WaitForSeconds(0.3f);
                val = AssetBundleManager.Instance.Progress;
                progress.text = "验证远端资源更新: " + val.ToString("f1") + "%";

                if (AssetBundleManager.Instance.IsError)
                {
                    progress.text = AssetBundleManager.Instance.ErrorInfo;
                    yield break;
                }
            }

            //StartCoroutine(UIFactory.Instance.PreloadAllWindow());
            //var str = "场景资源预加载.";
            //var idx = 0;
            //while (!UIFactory.Instance.allWindowReady)
            //{
            //    progress.text = str;
            //    for (int i = 0; i < idx; i++)
            //        progress.text += ".";
            //    idx = (idx + 1) % 3;
            //    yield return new WaitForSeconds(0.5f);
            //}
            //yield return new WaitForSeconds(1);
            SceneManager.LoadScene(1);
        }
    }
}


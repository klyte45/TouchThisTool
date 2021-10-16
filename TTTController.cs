﻿using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Klyte.UpgradeUntouchable
{
    public class TTTController : BaseController<TouchThisToolMod, TTTController>
    {
        public const string FOLDER_NAME = "TouchThisTool";

        internal void ToggleTool(bool newState, InfoManager.InfoMode viewMode, InfoManager.SubInfoMode submode)
        {
            if (newState)
            {
                InfoManager.instance.SetCurrentMode(viewMode, submode);
            }
            else
            {
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.None);
            }

            StartCoroutine(ToggleTool_Internal(newState));
        }
        private IEnumerator ToggleTool_Internal(bool newState)
        {
            yield return 0;
            if (TouchThisTool.instance.enabled && !newState)
            {
                TouchThisTool.instance.enabled = newState;
                ToolController tc = FindObjectOfType<ToolController>();
                tc.CurrentTool = tc.GetComponent<DefaultTool>();
            }
            else
            {
                TouchThisTool.instance.enabled = newState;
            }
        }



    }

}

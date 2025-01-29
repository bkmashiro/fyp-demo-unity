using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]  // 让这个脚本在Editor模式下运行
public class EditorModeHelper : MonoBehaviour
{
    [Tooltip("在Editor模式下需要禁用的脚本列表")]
    public List<MonoBehaviour> scriptsToDisable = new();

    void OnEnable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            DisableScriptsInEditor();
        }
#endif
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            EnableScriptsInEditor();
        }
#endif
    }

    void DisableScriptsInEditor()
    {
        if (scriptsToDisable == null || scriptsToDisable.Count == 0)
        {
            Debug.LogWarning("没有需要禁用的脚本");
            return;
        }

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script == null)
            {
                Debug.LogWarning("脚本为空");
                continue;
            }

            script.enabled = false;
            Debug.Log("禁用脚本: " + script.GetType().Name);
        }
    }

    void EnableScriptsInEditor()
    {
        if (scriptsToDisable == null || scriptsToDisable.Count == 0)
        {
            Debug.LogWarning("没有需要启用的脚本");
            return;
        }

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script == null)
            {
                Debug.LogWarning("脚本为空");
                continue;
            }

            script.enabled = true;
            Debug.Log("启用脚本: " + script.GetType().Name);
        }
    }
}

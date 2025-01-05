using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DebugLogToScreen : MonoBehaviour
{
    public Label dbgLabel; // 用于显示日志的UI文本
    public bool showDebugLog = true;

    public int maxLines = 50; // 最多显示的日志行数
    public UIDocument uiDocument;

    private Queue<string> logQueue = new Queue<string>();


    private void OnEnable()
    {
        // 获取UI Document

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument not found!");
            return;
        }

        // 获取根VisualElement
        VisualElement root = uiDocument.rootVisualElement;

        dbgLabel = root.Q<Label>("dbg");

        root.Q<UnityEngine.UIElements.Button>("enableDbgBtn").clicked += () => ToggleDebugLog(!showDebugLog);
    }

    void Awake()
    {
        // 订阅Unity的日志回调
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        // 移除订阅
        Application.logMessageReceived -= HandleLog;
    }

    // 处理日志
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"[{type}] {logString}";
        logQueue.Enqueue(logEntry);

        // 限制显示的日志行数
        if (logQueue.Count > maxLines)
        {
            logQueue.Dequeue();
        }

        // 更新UI显示
        if (dbgLabel != null)
        {
            dbgLabel.text = string.Join("\n", logQueue);
        }
    }

    public void ClearLog()
    {
        logQueue.Clear();
        dbgLabel.text = "";
    }

    public void Log(string message)
    {
        logQueue.Enqueue(message);

        // 限制显示的日志行数
        if (logQueue.Count > maxLines)
        {
            logQueue.Dequeue();
        }

        // 更新UI显示
        if (dbgLabel != null)
        {
            dbgLabel.text = string.Join("\n", logQueue);
        }
    }

    public void ToggleDebugLog(bool value)
    {
        showDebugLog = value;
        dbgLabel.style.display = showDebugLog ? DisplayStyle.Flex : DisplayStyle.None;
    }
}

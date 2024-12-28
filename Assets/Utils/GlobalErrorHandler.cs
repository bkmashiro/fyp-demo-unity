using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalErrorHandler : MonoBehaviour
{

    public DebugLogToScreen logger;

    private void Awake()
    {
        // 捕获未处理的异常
        AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

        // 捕获Unity日志消息（包括错误和警告）
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        // 移除事件订阅，防止内存泄漏
        AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
        Application.logMessageReceived -= HandleLog;
    }

    // 处理未捕获的异常
    private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        string exceptionMessage = $"[Unhandled Exception]: {e.ExceptionObject.ToString()}";
        DisplayError(exceptionMessage);
    }

    // 处理Unity日志消息
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            string exceptionMessage = $"[Error]: {logString}\n{stackTrace}";
            DisplayError(exceptionMessage);
        }
    }

    // 在UI上显示错误信息
    private void DisplayError(string message)
    {
        if (logger != null)
        {
            logger.Log(message);
        }
        else
        {
            Debug.LogWarning("错误信息无法显示：未设置UI文本组件！");
        }
    }
}

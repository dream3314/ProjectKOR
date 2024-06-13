using System;
using UnityEngine;
using System.Collections.Generic;

public class CustomLogger : MonoBehaviour
{
    private List<LogMessage> logMessages = new List<LogMessage>();
    private Vector2 scrollPosition;
    private bool showInfo = true;
    private bool showWarning = true;
    private bool showError = true;

    private GUIStyle infoStyle;
    private GUIStyle warningStyle;
    private GUIStyle errorStyle;
    private GUIStyle headerStyle;
    private bool stylesInitialized = false;

    [SerializeField] private int fontSize = 32; // 기본 폰트 크기
    [SerializeField] private float windowWidth = 800f; // 창 너비 (픽셀 단위)
    [SerializeField] private float windowHeight = 400f; // 창 높이 (픽셀 단위)
    [SerializeField] private float margin = 10f; // 여백 (픽셀 단위)

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void InitializeStyles()
    {
        infoStyle = new GUIStyle();
        infoStyle.normal.textColor = Color.white;
        infoStyle.wordWrap = true;
        infoStyle.fontSize = fontSize;

        warningStyle = new GUIStyle();
        warningStyle.normal.textColor = Color.yellow;
        warningStyle.wordWrap = true;
        warningStyle.fontSize = fontSize;

        errorStyle = new GUIStyle();
        errorStyle.normal.textColor = Color.red;
        errorStyle.wordWrap = true;
        errorStyle.fontSize = fontSize;

        headerStyle = new GUIStyle(GUI.skin.box);
        headerStyle.fontSize = fontSize;
        headerStyle.alignment = TextAnchor.MiddleCenter;

        stylesInitialized = true;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logMessages.Add(new LogMessage(logString, stackTrace, type));
        if (logMessages.Count > 100)
        {
            logMessages.RemoveAt(0);
        }
    }

    void OnGUI()
    {
        if (!stylesInitialized)
        {
            InitializeStyles();
        }

        GUILayout.BeginArea(new Rect(margin, margin, windowWidth, windowHeight), GUI.skin.box);
        GUILayout.Label("Logs", headerStyle);

        GUILayout.BeginHorizontal();
        showInfo = GUILayout.Toggle(showInfo, "Info");
        showWarning = GUILayout.Toggle(showWarning, "Warning");
        showError = GUILayout.Toggle(showError, "Error");
        if (GUILayout.Button("Clear"))
        {
            logMessages.Clear();
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowWidth - 20), GUILayout.Height(windowHeight - 80));
        foreach (LogMessage log in logMessages)
        {
            if ((log.Type == LogType.Log && showInfo) ||
                (log.Type == LogType.Warning && showWarning) ||
                (log.Type == LogType.Error && showError))
            {
                GUIStyle style = infoStyle;
                if (log.Type == LogType.Warning) style = warningStyle;
                else if (log.Type == LogType.Error) style = errorStyle;

                GUILayout.Label(log.Message, style);
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private class LogMessage
    {
        public string Message { get; }
        public string StackTrace { get; }
        public LogType Type { get; }

        public LogMessage(string message, string stackTrace, LogType type)
        {
            Message = message;
            StackTrace = stackTrace;
            Type = type;
        }
    }
}

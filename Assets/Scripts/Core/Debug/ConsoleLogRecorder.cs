using System;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Core.Debugging
{
    // プレイ開始ごとに Logs/latest.log を作り直し、コンソール出力を収集
    public static class ConsoleLogRecorder
    {
        private static readonly object LockObj = new object();
        private static string s_LogsDir;
        private static string s_LatestPath;
        private static StreamWriter s_Writer;
        private static bool s_Initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (s_Initialized) return;
            s_Initialized = true;
            try
            {
                // プロジェクト直下に Logs フォルダを作成
                var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                s_LogsDir = Path.Combine(projectRoot, "Logs");
                Directory.CreateDirectory(s_LogsDir);
                s_LatestPath = Path.Combine(s_LogsDir, "latest.log");

                // 前回分を上書き
                s_Writer = new StreamWriter(s_LatestPath, append: false, Encoding.UTF8) { AutoFlush = true };
                WriteHeader();
                TryAppendSetupSummary();

                Application.logMessageReceivedThreaded += OnLog;
                Application.quitting += OnQuit;
                // ドメインリロード無効（Enter Play Mode Options）でも確実にヘッダを書けるように、Play開始印もメインスレッドで記録
                Debug.Log("[ConsoleLogRecorder] Session started");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ConsoleLogRecorder] Init failed: {e.Message}");
            }
        }

        private static void TryAppendSetupSummary()
        {
            try
            {
                var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                var logsDir = Path.Combine(projectRoot, "Logs");
                var summaryPath = Path.Combine(logsDir, "setup_summary.txt");
                if (File.Exists(summaryPath))
                {
                    var text = File.ReadAllText(summaryPath, Encoding.UTF8);
                    lock (LockObj)
                    {
                        if (s_Writer != null)
                        {
                            s_Writer.WriteLine(text);
                            s_Writer.WriteLine();
                        }
                    }
                }

#if UNITY_EDITOR
                // Editor限定で Active Input Handling を出力（確認用）
                var settings = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
                s_Writer?.WriteLine($"[Editor] ActiveBuildTarget={settings}");
                try
                {
                    // 表示用にProjectSettings/ProjectSettings.assetのActive Input Handling値を推定して出力
                    var playerSettingsPath = Path.Combine(projectRoot, "ProjectSettings", "ProjectSettings.asset");
                    if (File.Exists(playerSettingsPath))
                    {
                        var content = File.ReadAllText(playerSettingsPath);
                        // 簡易表記（正確な列挙は未公開APIのためログのみ）
                        if (content.Contains("activeInputHandler: 0")) s_Writer?.WriteLine("[Editor] Active Input Handling=Both/Old");
                        if (content.Contains("activeInputHandler: 1")) s_Writer?.WriteLine("[Editor] Active Input Handling=Input Manager");
                        if (content.Contains("activeInputHandler: 2")) s_Writer?.WriteLine("[Editor] Active Input Handling=Input System Package");
                    }
                }
                catch { }
#endif
            }
            catch { }
        }

        private static void WriteHeader()
        {
            lock (LockObj)
            {
                if (s_Writer == null) return;
                s_Writer.WriteLine($"=== Play Session Start {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                s_Writer.WriteLine($"Unity {Application.unityVersion} | {Application.platform} | Product: {Application.productName}");
                s_Writer.WriteLine();
            }
        }

        private static void OnLog(string condition, string stackTrace, LogType type)
        {
            try
            {
                lock (LockObj)
                {
                    if (s_Writer == null) return;
                    var time = DateTime.Now.ToString("HH:mm:ss.fff");
                    s_Writer.Write('[');
                    s_Writer.Write(time);
                    s_Writer.Write("] ");
                    s_Writer.Write(type);
                    s_Writer.Write(": ");
                    s_Writer.WriteLine(condition);
                    // 簡易タグ: ワーニング以上を視認しやすく
                    if (type == LogType.Warning)
                    {
                        s_Writer.WriteLine("[!] WARNING ABOVE");
                    }
                    if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                    {
                        if (!string.IsNullOrEmpty(stackTrace))
                        {
                            s_Writer.WriteLine(stackTrace);
                        }
                        s_Writer.WriteLine("[!] ERROR ABOVE");
                    }
                }
            }
            catch
            {
                // 簡易耐障害
            }
        }

        private static void OnQuit()
        {
            try
            {
                Application.logMessageReceivedThreaded -= OnLog;
                lock (LockObj)
                {
                    if (s_Writer != null)
                    {
                        s_Writer.Flush();
                        s_Writer.Dispose();
                        s_Writer = null;
                    }
                }
            }
            catch { }
        }
    }
}



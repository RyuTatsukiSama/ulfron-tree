using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RLog
{
    public enum LogLevel
    {
        DEBUG = 0x1,
        VERBOSE = 0x2,
        INFO = 0x4,
        WARN = 0x8,
        ERROR = 0x10,
        FATAL = 0x20,
        ALL = 0x3f,
        NONE = 0xff
    }

    public interface ILoggerFormatter
    {
        string Format(LogLevel level, DateTime time, string msg, params object[] list);
    }

    public class DefaultFormatter : ILoggerFormatter
    {
        public string Format(LogLevel level, DateTime time, string msg, params object[] list)
        {
            string str = "{0} {1}\n{2}\n[{3}]\n";
            string userFormattedMessage = string.Format(msg, list);
            return string.Format(str, time.ToString("yyyy-MM-dd HH:mm:ss"), level, userFormattedMessage, 0 /*CallStack.GetCallStack()*/);
        }
    }

    static class CallStack
    {
        public static string GetCallStack()
        {
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(3);
            string str = $"{sf.GetFileName().Split("\\")[sf.GetFileName().Split("\\").Length - 1]} -> {sf.GetMethod()} : Line {sf.GetFileLineNumber()}";
            for (int i = 4; i < st.FrameCount; i++)
            {
                sf = st.GetFrame(i);
                str += $" | {sf.GetFileName().Split("\\")[sf.GetFileName().Split("\\").Length - 1]} -> {sf.GetMethod()} : Line {sf.GetFileLineNumber()}";
            }
            return str;
        }
    }

    public static class RLogger
    {
        private static ILoggerFormatter defaultFormatter;
        private static ILoggerFormatter currentFormatter;

        public static LogLevel minimumLevel = LogLevel.DEBUG;
        public static LogLevel levelFilters = LogLevel.ALL;

        private static DateTime fileName = default;

        private static bool IsLevelValid(LogLevel level)
        {
            return (level != LogLevel.NONE && level != LogLevel.ALL && level >= minimumLevel
            && (level & levelFilters) == level);
        }

        public static ILoggerFormatter Formatter
        {
            get { return defaultFormatter; }
            set
            {
                if (value == null)
                    currentFormatter = defaultFormatter;
                else
                    defaultFormatter = value;
            }
        }

        public static void Log(LogLevel level, string msg, params object[] list)
        {
            defaultFormatter = new DefaultFormatter();
            if (IsLevelValid(level))
            {
                string str = Formatter.Format(level, DateTime.Now, msg, list);
                WriteInFile(str);
#if UNITY_EDITOR
                WriteConsole(level, msg);
#endif
            }
        }

        static void WriteInFile(string msg)
        {
            string projectFolder;
#if UNITY_EDITOR
            projectFolder = Regex.Replace(Application.dataPath, "/Assets$", "");
#else
            projectFolder = Regex.Replace(Application.dataPath, "Journeep_Data$", "");
#endif
            string logsFolder = $"{projectFolder}/RLogs";

            if (!Directory.Exists(logsFolder))
            {
                Directory.CreateDirectory(logsFolder);
            }

            fileName = fileName == default ? DateTime.Now : fileName;
            string filePath = $"Logs {fileName.ToString("yyyy-MM-dd HH_mm_ss")}.log";
            string tmpPath = Path.Combine(logsFolder, filePath);

            using (StreamWriter sw = new StreamWriter(tmpPath, true))
            {
                sw.WriteLine(msg);
            }
        }

        static void WriteConsole(LogLevel level, string msg)
        {
            switch (level)
            {
                case LogLevel.DEBUG:
                case LogLevel.VERBOSE:
                case LogLevel.INFO:
                    //UnityEngine.Debug.Log(msg);
                    break;
                case LogLevel.WARN:
                    UnityEngine.Debug.LogWarning(msg);
                    break;
                case LogLevel.ERROR:
                    UnityEngine.Debug.LogError(msg);
                    break;
                case LogLevel.FATAL:
                    UnityEngine.Debug.LogError(msg);
                    UnityEngine.Debug.Break();
                    break;
                case LogLevel.ALL:
                    break;
                case LogLevel.NONE:
                    break;
                default:
                    break;
            }
        }
    }
}

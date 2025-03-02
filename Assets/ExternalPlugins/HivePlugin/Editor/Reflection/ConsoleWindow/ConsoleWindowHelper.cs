using Modules.Hive.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/LogEntries.bindings.cs
namespace Modules.Hive.Editor.Reflection
{
    public static class ConsoleWindowHelper
    {
        #region Fields

        private delegate void GetCountsByTypeDelegate(ref int errorCount, ref int warningCount, ref int logCount);

        private static readonly GetCountsByTypeDelegate GetCountsByType;
        private static readonly Func<int> StartGettingEntries;
        private static readonly Action EndGettingEntries;
        private static readonly Delegate GetEntryInternal;
        
        private static readonly Func<int> GetConsoleFlags;
        private static readonly Action<int> SetConsoleFlags;

        private static readonly Type LogEntryType;
        private static readonly FieldInfo LogEntryModeField;
        private static readonly FieldInfo LogEntryMessageField;
        
        #endregion
        
        
        
        #region Class lifecycle

        static ConsoleWindowHelper()
        {
            // Reflect UnityEditor.LogEntries
            Type logEntriesType = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            GetCountsByType = ReflectionHelper.CreateDelegateToMethod<GetCountsByTypeDelegate>(
                logEntriesType,
                "GetCountsByType",
                BindingFlags.Static | BindingFlags.Public,
                true);
            StartGettingEntries = ReflectionHelper.CreateDelegateToMethod<Func<int>>(
                logEntriesType,
                "StartGettingEntries",
                BindingFlags.Static | BindingFlags.Public,
                true);
            EndGettingEntries = ReflectionHelper.CreateDelegateToMethod<Action>(
                logEntriesType,
                "EndGettingEntries",
                BindingFlags.Static | BindingFlags.Public,
                true);
            GetEntryInternal = ReflectionHelper.CreateDelegateToMethod(
                logEntriesType,
                "GetEntryInternal",
                BindingFlags.Static | BindingFlags.Public);
            GetConsoleFlags = ReflectionHelper.CreateDelegateToPropertyGet<Func<int>>(
                logEntriesType,
                null,
                "consoleFlags",
                true);
            SetConsoleFlags = ReflectionHelper.CreateDelegateToPropertySet<Action<int>>(
                logEntriesType,
                null,
                "consoleFlags",
                true);

            // Reflect UnityEditor.LogEntry
            LogEntryType = Type.GetType("UnityEditor.LogEntry, UnityEditor.dll");
            LogEntryModeField = LogEntryType.GetField("mode", BindingFlags.Instance | BindingFlags.Public);
            LogEntryMessageField = LogEntryType.GetField("message", BindingFlags.Instance | BindingFlags.Public);
        }
        
        #endregion
        
        
        
        #region Logs count

        public static void GetLogsCountByType(ref int errorCount, ref int warningCount, ref int logCount) => 
            GetCountsByType(ref errorCount, ref warningCount, ref logCount);


        public static int GetScriptCompileErrorsCount()
        {
            int errorCount = 0;
            int warningCount = 0;
            int logCount = 0;
            GetLogsCountByType(ref errorCount, ref warningCount, ref logCount);

            if (errorCount != 0)
            {
                errorCount = EnumerateLogEntries().Count(IsScriptCompileErrorEntry);
            }

            return errorCount;
            
            
            bool IsScriptCompileErrorEntry(object entry) =>
                (GetLogEntryMode(entry) & ConsoleLogEntryMode.ScriptCompileError) != 0;
        }
        
        #endregion
        


        #region Console window flags

        public static void SetFlags(ConsoleFlag flags) => SetConsoleFlags((int)flags);
        public static ConsoleFlag GetFlags() => (ConsoleFlag)GetConsoleFlags();

        #endregion



        #region Log entry info

        public static ConsoleLogEntryMode GetLogEntryMode(object entry) =>
            (ConsoleLogEntryMode)(int)LogEntryModeField.GetValue(entry);
        
        public static string GetLogEntryMessage(object entry) =>
            (string)LogEntryMessageField.GetValue(entry);

        #endregion



        #region Log entries enumeration

        public static IEnumerable<object> EnumerateLogEntries()
        {
            int count = StartGettingEntries();
            try
            {
                object entry = null;
                for (int i = 0; i < count; i++)
                {
                    bool success = GetEntry(i, ref entry);
                    if (!success)
                    {
                        break;
                    }

                    yield return entry;
                }
            }
            finally
            {
                EndGettingEntries();
            }
            
            
            bool GetEntry(int row, ref object outputEntry)
            {
                if (outputEntry == null)
                {
                    outputEntry = Activator.CreateInstance(LogEntryType);
                }

                return (bool)GetEntryInternal.DynamicInvoke(row, outputEntry);
            }
        }

        #endregion
    }
}

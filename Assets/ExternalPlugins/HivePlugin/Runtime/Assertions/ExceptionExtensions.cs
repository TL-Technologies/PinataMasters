using System;
using System.Diagnostics;
using System.Linq;


namespace Modules.Hive.Assertions
{
    public static class ExceptionExtensions
    {
        public static string GetStackTraceWithoutHiddenMethods(this Exception e)
        {
            return string.Concat(
               new StackTrace(e, true)
                   .GetFrames()
                   .Where(frame => !frame.GetMethod().IsDefined(typeof(DebuggerHiddenAttribute), true))
                   .Select(frame => $"{new StackTrace(frame)}\n")
                   .ToArray());
        }
    }
}

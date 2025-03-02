using System;


namespace Modules.Hive.Assertions
{
    public class AssertionException : Exception
    {
        private string stacktrace = null;

        
        public AssertionException(string message) : base(message) { }

        
        public override string StackTrace => stacktrace ?? (stacktrace = this.GetStackTraceWithoutHiddenMethods());
    }
}

using System.Diagnostics;


namespace Modules.Hive.Assertions
{
    public static class Assert
    {
        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws an Hive.Assertions.AssertionException.
        /// </summary>
        /// <param name="condition">The evaluated condition.</param>
        /// <param name="message">The message to display in case of failure.</param>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void IsTrue(bool condition, string message = null)
        {
            if (!condition)
            {
                throw new AssertionException(message ?? "Assertion exception.\nExpected: 'true'\nBut was: 'false'");
            }
        }

        
        /// <summary>
        /// Asserts that an object reference is null. If it's not null the method throws an Hive.Assertions.AssertionException.
        /// </summary>
        /// <param name="obj">The target reference.</param>
        /// <param name="message">The message to display in case of failure.</param>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void IsNull(object obj, string message = null)
        {
            if (obj != null)
            {
                throw new AssertionException(message ?? $"Assertion exception.\nExpected: 'null'\nBut was: '{obj}'");
            }
        }

        
        /// <summary>
        /// Asserts that an object reference is not null. If it's null the method throws an Hive.Assertions.AssertionException.
        /// </summary>
        /// <param name="obj">The target reference.</param>
        /// <param name="message">The message to display in case of failure.</param>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void IsNotNull(object obj, string message = null)
        {
            if (obj == null)
            {
                throw new AssertionException(message ?? "Assertion exception.\nExpected: 'not null'\nBut was: 'null'");
            }
        }

        
        /// <summary>
        /// Asserts that an actual value is equal to expected value, otherwise the method throws an Hive.Assertions.AssertionException.
        /// </summary>
        /// <param name="actual">Actual value.</param>
        /// <param name="expected">Expected value.</param>
        /// <param name="message">The message to display in case of failure.</param>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void AreEquals(int actual, int expected, string message = null)
        {
            if (actual == expected)
            {
                throw new AssertionException(message ?? $"Assertion exception.\nExpected: '{expected}'\nBut was: '{actual}'");
            }
        }

        
        /// <summary>
        /// Throws an Hive.Assertions.AssertionException with specified message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void Throw(string message)
        {
            throw new AssertionException(message ?? "Assertion exception was thrown explicitly.");
        }
    }
}

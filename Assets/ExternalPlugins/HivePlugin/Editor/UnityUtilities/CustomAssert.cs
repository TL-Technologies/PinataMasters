using NUnit.Framework;
using UnityEngine;


namespace Modules.Hive.Editor
{
    // This class is workaround for various bugs and strange behaviors in Unity and Jenkins
    public static class CustomAssert
    {
        // Due to bug in Jenkins NUnit plugin Assert.Pass is interpreted by Jenkins as skipped test,
        // but not as passed one.
        public static void Pass(string message)
        {
            if (Application.isBatchMode)
            {
                Assert.True(true);
            }
            else
            {
                Assert.Pass(message);
            }
        }
        
        
        // https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/changelog/CHANGELOG.html
        // Inconclusive tests, which has been executed from command line, have caused exit code 2
        // since Unity Framework version 1.1.19. Non-zero exit code leads to crash during Jenkins build, so
        // inconclusiveness is replaced by pass.
        public static void Inconclusive(string message)
        {
            if (Application.isBatchMode)
            {
                Assert.True(true);
            }
            else
            {
                Assert.Inconclusive(message);
            }
        }
    }
}

namespace Modules.Hive.Pipeline
{
    // Get summary from here: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskstatus?view=netframework-4.8#System_Threading_Tasks_TaskStatus_Faulted

    public enum PipelineStatus
    {
        None = 0,
        WaitingToRun,
        Running,
        Succeeded,
        Failed,
        Canceled
    }
}

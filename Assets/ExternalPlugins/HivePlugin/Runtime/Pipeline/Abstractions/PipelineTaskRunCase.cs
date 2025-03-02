using System;


namespace Modules.Hive.Pipeline
{
    [Flags]
    public enum PipelineTaskRunCase
    {
        None        = 0,
        OnSuccess   = 1,
        OnFailure   = 2,
        OnCancel    = 4,

        Always      = OnSuccess | OnFailure | OnCancel
    }
}
